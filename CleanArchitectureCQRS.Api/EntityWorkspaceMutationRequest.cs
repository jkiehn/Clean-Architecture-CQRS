namespace CleanArchitectureCQRS.Api;

public record EntityWorkspaceMutationRequest(IReadOnlyDictionary<string, string?> Values);
