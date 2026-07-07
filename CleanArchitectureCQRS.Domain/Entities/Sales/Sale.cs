using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.Sales;

public class Sale : Event
{
    private ParticipationId _internalParticipationId = default!;
    private AgentId _employeeId = default!;
    private ParticipationId _externalParticipationId = default!;
    private AgentId _customerId = default!;
    private readonly List<SalesLine> _salesLines = [];

    public Sale()
    {
    }

    public Sale(EventId id, DateTimeOffset when, AgentId employeeId, AgentId customerId, DateTimeOffset? endWhen = null, decimal? amount = null)
        : base(id, when, endWhen, amount)
    {
        _internalParticipationId = new ParticipationId(Guid.NewGuid());
        _externalParticipationId = new ParticipationId(Guid.NewGuid());
        UpdateCounterparties(employeeId, customerId);
    }

    public void UpdateDetails(DateTimeOffset when, AgentId employeeId, AgentId customerId, DateTimeOffset? endWhen = null)
    {
        UpdateTemporalDetails(when, endWhen, _salesLines.Count == 0 ? GetFieldValue<decimal?>("_amount") : _salesLines.Sum(line => line.LineTotal));
        UpdateCounterparties(employeeId, customerId);
    }

    public IReadOnlyList<SalesLine> GetSalesLines()
        => _salesLines.AsReadOnly();

    public SaleInternalParticipation GetInternalParticipation()
        => new(_internalParticipationId, _employeeId, Id);

    public SaleExternalParticipation GetExternalParticipation()
        => new(_externalParticipationId, _customerId, Id);

    public SalesLine AddSalesLine(ResourceId itemId, decimal unitPrice, decimal quantity)
    {
        var line = new SalesLine(new StockflowId(Guid.NewGuid()), Id, itemId, unitPrice, quantity);
        _salesLines.Add(line);
        RecalculateAmount();
        return line;
    }

    public void RemoveSalesLine(StockflowId salesLineId)
    {
        var removed = _salesLines.RemoveAll(line => line.Id == salesLineId);

        if (removed == 0)
        {
            throw new InvalidOperationException($"Sales line '{salesLineId.Value}' was not found.");
        }

        RecalculateAmount();
    }

    private void UpdateCounterparties(AgentId employeeId, AgentId customerId)
    {
        _employeeId = employeeId;
        _customerId = customerId;
    }

    private void RecalculateAmount()
        => UpdateTemporalDetails(
            GetFieldValue<DateTimeOffset>("_when"),
            GetFieldValue<DateTimeOffset?>("_endWhen"),
            _salesLines.Count == 0 ? null : _salesLines.Sum(line => line.LineTotal));

    private T GetFieldValue<T>(string fieldName)
    {
        var currentType = GetType();
        System.Reflection.FieldInfo? field = null;

        while (currentType is not null && field is null)
        {
            field = currentType.GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            currentType = currentType.BaseType;
        }

        return (T)field!.GetValue(this)!;
    }
}
