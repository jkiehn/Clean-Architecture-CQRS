namespace CleanArchitectureCQRS.Maui.Shared.Services;

public record CustomerModel(Guid Id, string Name, string Email);
public record CreateCustomerCommand(Guid Id, string Name, string Email);
public record UpdateCustomerCommand(Guid Id, string Name, string Email);
