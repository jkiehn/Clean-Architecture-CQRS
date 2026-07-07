using CleanArchitectureCQRS.Domain.Exceptions;
using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.Sales;

public sealed class SalesLine : Give
{
    private EventId _saleId = default!;
    private ResourceId _itemId = default!;
    private decimal _unitPrice;
    private decimal _quantity;

    public SalesLine()
    {
    }

    public SalesLine(StockflowId id, EventId saleId, ResourceId itemId, decimal unitPrice, decimal quantity)
        : base(id, new StockflowEndId(Guid.NewGuid()), new StockflowEndId(Guid.NewGuid()))
    {
        AssignToSale(saleId, itemId);
        UpdateAmounts(unitPrice, quantity);
    }

    public decimal LineTotal => _unitPrice * _quantity;

    public SalesLineEventEnd GetEventEnd()
        => new(GetFieldValue<StockflowEndId>("_occurrentEndId"), Id, _saleId);

    public SalesLineResourceEnd GetResourceEnd()
        => new(GetFieldValue<StockflowEndId>("_resourceEndId"), Id, _itemId);

    public void UpdateDetails(ResourceId itemId, decimal unitPrice, decimal quantity)
    {
        _itemId = itemId;
        UpdateAmounts(unitPrice, quantity);
    }

    private void AssignToSale(EventId saleId, ResourceId itemId)
    {
        _saleId = saleId;
        _itemId = itemId;
    }

    private void UpdateAmounts(decimal unitPrice, decimal quantity)
    {
        if (unitPrice < 0 || quantity <= 0)
        {
            throw new StockflowInvalidException();
        }

        _unitPrice = unitPrice;
        _quantity = quantity;
    }

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
