namespace CleanArchitectureCQRS.Application.Services;

public interface IResourceSubtypeReadService
{
    Task<bool> ExistsByNameAsync(string name, Guid? excludeResourceId = null);
}
