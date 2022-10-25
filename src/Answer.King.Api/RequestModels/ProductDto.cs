namespace Answer.King.Api.RequestModels;

public record ProductDto
{
    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;

    public double Price { get; init; }

    public CategoryId Category { get; init; } = null!;
}
