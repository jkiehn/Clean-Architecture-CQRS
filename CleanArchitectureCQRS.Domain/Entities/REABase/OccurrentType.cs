using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public class OccurrentType : Continuant<OccurrentTypeId, OccurrentTypeName>
{
    public OccurrentType()
    {
    }

    public OccurrentType(OccurrentTypeId id, OccurrentTypeName name)
        : base(id, name)
    {
    }

    public void UpdateDetails(OccurrentTypeName name)
    {
        UpdateName(name);
    }
}
