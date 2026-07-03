namespace CleanArchitectureCQRS.Maui.Shared.Services;

public enum Gender
{
    Male = 0,
    Female = 1
}

public record DestinationDto(string? City, string? Country);

public record SampleEntityItemModel(string Name, uint Quantity, bool IsTaken);

public record SampleEntityModel(Guid Id, string Name, DestinationDto? Destination, IEnumerable<SampleEntityItemModel>? Items)
{
    public IEnumerable<SampleEntityItemModel> SafeItems => Items ?? Enumerable.Empty<SampleEntityItemModel>();

    public string DestinationLabel =>
        string.Join(", ", new[] { Destination?.City, Destination?.Country }.Where(static x => !string.IsNullOrWhiteSpace(x)));
}

public record DestinationWriteModel(string City, string Country);

public record CreateSampleEntityCommand(Guid Id, string Name, Gender Gender, DestinationWriteModel Destionation);
