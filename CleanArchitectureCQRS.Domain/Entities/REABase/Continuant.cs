using CleanArchitectureCQRS.Shared.Abstractions.Domains;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public abstract class Continuant<TId, TName> : AggregateRoot<TId>
{
    protected TName _name = default!;

    protected Continuant()
    {
    }

    protected Continuant(TId id, TName name)
    {
        Id = id;
        _name = name;
    }

    protected void UpdateName(TName name)
    {
        _name = name;
    }
}
