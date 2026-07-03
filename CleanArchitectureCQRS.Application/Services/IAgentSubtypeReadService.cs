namespace CleanArchitectureCQRS.Application.Services;

public interface IAgentSubtypeReadService
{
    Task<bool> ExistsByEmailAsync(string email, Guid? excludeAgentId = null);
}
