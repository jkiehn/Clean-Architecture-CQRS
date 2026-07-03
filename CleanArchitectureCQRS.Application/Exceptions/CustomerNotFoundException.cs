using CleanArchitectureCQRS.Shared.Abstractions.Exceptions;

namespace CleanArchitectureCQRS.Application.Exceptions;

public class CustomerNotFoundException : PublicException
{
    public Guid Id { get; }

    public CustomerNotFoundException(Guid id) : base($"Customer with ID '{id}' was not found.")
        => Id = id;
}
