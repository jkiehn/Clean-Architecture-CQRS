namespace CleanArchitectureCQRS.Maui.Shared.Services;

public class CustomerService : ApiServiceBase
{
    public CustomerService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<IReadOnlyList<CustomerModel>> GetAllAsync(string searchPhrase = "")
    {
        var query = string.IsNullOrWhiteSpace(searchPhrase)
            ? string.Empty
            : $"?searchPhrase={Uri.EscapeDataString(searchPhrase)}";

        return await GetListAsync<CustomerModel>($"api/Customer{query}");
    }

    public Task<CustomerModel?> GetByIdAsync(Guid id)
        => GetAsync<CustomerModel>($"api/Customer/{id}");

    public Task CreateAsync(CreateCustomerCommand command)
        => PostAsync("api/Customer", command);

    public Task UpdateAsync(UpdateCustomerCommand command)
        => PutAsync($"api/Customer/{command.Id}", command);

    public Task DeleteAsync(Guid id)
        => DeleteAsync($"api/Customer/{id}");
}
