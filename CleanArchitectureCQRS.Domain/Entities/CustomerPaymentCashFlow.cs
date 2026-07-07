using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public sealed class CustomerPaymentCashFlow : Take
{
    private EventId _customerPaymentId = default!;
    private ResourceId _cashResourceId = default!;

    public CustomerPaymentCashFlow()
    {
    }

    public CustomerPaymentCashFlow(StockflowId id, EventId customerPaymentId, ResourceId cashResourceId)
        : base(id, new StockflowEndId(Guid.NewGuid()), new StockflowEndId(Guid.NewGuid()))
    {
        _customerPaymentId = customerPaymentId;
        _cashResourceId = cashResourceId;
    }

    public CustomerPaymentEventEnd GetEventEnd()
        => new(GetFieldValue<StockflowEndId>("_occurrentEndId"), Id, _customerPaymentId);

    public CustomerPaymentCashEnd GetCashEnd()
        => new(GetFieldValue<StockflowEndId>("_resourceEndId"), Id, _cashResourceId);

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
