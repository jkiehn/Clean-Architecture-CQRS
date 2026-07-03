using CleanArchitectureCQRS.Domain.Events;
using CleanArchitectureCQRS.Domain.Exceptions;
using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Shared.Abstractions.Domains;

namespace CleanArchitectureCQRS.Domain.Entities;

public class SampleEntity : AggregateRoot<SampleEntityId>
{
    private SampleEntityName _name = default!;
    private SampleEntityDestination _destination = default!;
    private readonly LinkedList<SampleEntityItem> _items = new();

    internal SampleEntity(
        SampleEntityId id,
        SampleEntityName name,
        SampleEntityDestination destination)
    {
        Id = id;
        _name = name;
        _destination = destination;
    }

    public SampleEntity()
    {
    }

    public void AddItem(SampleEntityItem item)
    {
        var alreadyExists = _items.Any(i => i.Name == item.Name);

        if (alreadyExists)
        {
            throw new SampleDuplicateException(_name, item.Name);
        }

        _items.AddLast(item);
        AddEvent(new SampleEntityItemAdded(this, item));
    }

    public void AddItems(IEnumerable<SampleEntityItem> items)
    {
        foreach (var item in items)
        {
            AddItem(item);
        }
    }

    public void TakeItem(string itemName)
    {
        var item = GetItem(itemName);
        var sampleEntityItem = item with { IsTaken = true };

        var itemNode = _items.Find(item);

        if (itemNode is null)
        {
            throw new SampleInvalidException();
        }

        itemNode.Value = sampleEntityItem;
        AddEvent(new SampleEntityItemTaken(this, item));
    }

    public void RemoveItem(string itemName)
    {
        var item = GetItem(itemName);
        _items.Remove(item);
        AddEvent(new SampleEntityItemRemoved(this, item));
    }

    private SampleEntityItem GetItem(string itemName)
    {
        var item = _items.SingleOrDefault(i => i.Name == itemName);

        if (item is null)
        {
            throw new SampleInvalidException();
        }

        return item;
    }
}
