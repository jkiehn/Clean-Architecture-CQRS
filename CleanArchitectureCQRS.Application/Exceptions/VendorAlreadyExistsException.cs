using CleanArchitectureCQRS.Shared.Abstractions.Exceptions;

namespace CleanArchitectureCQRS.Application.Exceptions;

public class VendorAlreadyExistsException : PublicException
{
    public string Email { get; }

    public VendorAlreadyExistsException(string email) : base($"Vendor with email '{email}' already exists.")
        => Email = email;
}
