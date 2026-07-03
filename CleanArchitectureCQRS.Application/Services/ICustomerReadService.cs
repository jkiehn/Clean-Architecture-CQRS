namespace CleanArchitectureCQRS.Application.Services;

public interface ICustomerReadService
{
    Task<bool> ExistsByEmailAsync(string email, Guid? excludeCustomerId = null);
}
