using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.Sales;

public sealed class SalesOrder : Commitment
{
    private ParticipationId _internalParticipationId = default!;
    private AgentId _employeeId = default!;
    private ParticipationId _externalParticipationId = default!;
    private AgentId _customerId = default!;
    private readonly List<SalesOrderLine> _salesOrderLines = [];

    public SalesOrder()
    {
    }

    public SalesOrder(CommitmentId id, DateTimeOffset when, AgentId employeeId, AgentId customerId, DateTimeOffset? endWhen = null, decimal? amount = null)
        : base(id, when, endWhen, amount)
    {
        _internalParticipationId = new ParticipationId(Guid.NewGuid());
        _externalParticipationId = new ParticipationId(Guid.NewGuid());
        UpdateCounterparties(employeeId, customerId);
    }

    public void UpdateDetails(DateTimeOffset when, AgentId employeeId, AgentId customerId, DateTimeOffset? endWhen = null)
    {
        UpdateTemporalDetails(when, endWhen, _salesOrderLines.Count == 0 ? GetFieldValue<decimal?>("_amount") : _salesOrderLines.Sum(line => line.LineTotal));
        UpdateCounterparties(employeeId, customerId);
    }

    public IReadOnlyList<SalesOrderLine> GetSalesOrderLines()
        => _salesOrderLines.AsReadOnly();

    public SalesOrderInternalParticipation GetInternalParticipation()
        => new(_internalParticipationId, _employeeId, Id);

    public SalesOrderExternalParticipation GetExternalParticipation()
        => new(_externalParticipationId, _customerId, Id);

    public SalesOrderLine AddSalesOrderLine(ResourceId itemId, decimal unitPrice, decimal quantity)
    {
        var line = new SalesOrderLine(new StockflowId(Guid.NewGuid()), Id, itemId, unitPrice, quantity);
        _salesOrderLines.Add(line);
        RecalculateAmount();
        return line;
    }

    public void RemoveSalesOrderLine(StockflowId salesOrderLineId)
    {
        var removed = _salesOrderLines.RemoveAll(line => line.Id == salesOrderLineId);

        if (removed == 0)
        {
            throw new InvalidOperationException($"Sales order line '{salesOrderLineId.Value}' was not found.");
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
            _salesOrderLines.Count == 0 ? null : _salesOrderLines.Sum(line => line.LineTotal));

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
