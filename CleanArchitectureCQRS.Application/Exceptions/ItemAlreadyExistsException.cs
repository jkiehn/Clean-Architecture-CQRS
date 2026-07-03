using CleanArchitectureCQRS.Shared.Abstractions.Exceptions;

namespace CleanArchitectureCQRS.Application.Exceptions;

public class ItemAlreadyExistsException : PublicException
{
    public string Name { get; }

    public ItemAlreadyExistsException(string name) : base($"Item with name '{name}' already exists.")
        => Name = name;
}
