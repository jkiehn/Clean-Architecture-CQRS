using CleanArchitectureCQRS.Shared.Abstractions.Exceptions;

namespace CleanArchitectureCQRS.Application.Exceptions;

public class CustomerAlreadyExistsException : PublicException
{
    public string Email { get; }

    public CustomerAlreadyExistsException(string email) : base($"Customer with email '{email}' already exists.")
        => Email = email;
}
