using CleanArchitectureCQRS.Shared.Abstractions.Exceptions;

namespace CleanArchitectureCQRS.Application.Exceptions;

public class VendorNotFoundException : PublicException
{
    public Guid Id { get; }

    public VendorNotFoundException(Guid id) : base($"Vendor with ID '{id}' was not found.")
        => Id = id;
}
