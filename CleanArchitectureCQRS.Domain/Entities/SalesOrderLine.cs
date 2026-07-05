using CleanArchitectureCQRS.Domain.Exceptions;
using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public sealed class SalesOrderLine : Give
{
    private CommitmentId _salesOrderId = default!;
    private ResourceId _itemId = default!;
    private decimal _unitPrice;
    private decimal _quantity;

    public SalesOrderLine()
    {
    }

    public SalesOrderLine(StockflowId id, CommitmentId salesOrderId, ResourceId itemId, decimal unitPrice, decimal quantity)
        : base(id, new StockflowEndId(Guid.NewGuid()), new StockflowEndId(Guid.NewGuid()))
    {
        AssignToSalesOrder(salesOrderId, itemId);
        UpdateAmounts(unitPrice, quantity);
    }

    public decimal LineTotal => _unitPrice * _quantity;

    public SalesOrderLineCommitmentEnd GetCommitmentEnd()
        => new(GetFieldValue<StockflowEndId>("_occurrentEndId"), Id, _salesOrderId);

    public SalesOrderLineResourceEnd GetResourceEnd()
        => new(GetFieldValue<StockflowEndId>("_resourceEndId"), Id, _itemId);

    public void UpdateDetails(ResourceId itemId, decimal unitPrice, decimal quantity)
    {
        _itemId = itemId;
        UpdateAmounts(unitPrice, quantity);
    }

    private void AssignToSalesOrder(CommitmentId salesOrderId, ResourceId itemId)
    {
        _salesOrderId = salesOrderId;
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
