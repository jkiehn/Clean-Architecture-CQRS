using CleanArchitectureCQRS.Shared.Abstractions.Exceptions;

namespace CleanArchitectureCQRS.Application.Exceptions;

public class ItemNotFoundException : PublicException
{
    public Guid Id { get; }

    public ItemNotFoundException(Guid id) : base($"Item with ID '{id}' was not found.")
        => Id = id;
}
