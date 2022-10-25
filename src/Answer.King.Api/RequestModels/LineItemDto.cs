namespace Answer.King.Api.RequestModels;

public record LineItemDto
{
    public ProductId Product { get; init; } = null!;

    public int Quantity { get; init; }
}
