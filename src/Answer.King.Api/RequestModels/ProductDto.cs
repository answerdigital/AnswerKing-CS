namespace Answer.King.Api.RequestModels;

public record ProductDto
{
    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;

    public double Price { get; init; }

    public IList<CategoryId> Categories { get; init; } = null!;
}
