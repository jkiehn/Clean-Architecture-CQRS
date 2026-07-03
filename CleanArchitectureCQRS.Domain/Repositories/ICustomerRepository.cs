using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetAsync(AgentId id);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(Customer customer);
}
