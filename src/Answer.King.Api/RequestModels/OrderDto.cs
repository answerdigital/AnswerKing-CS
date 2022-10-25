namespace Answer.King.Api.RequestModels;

public record OrderDto
{
    public IList<LineItemDto> LineItems { get; init; } = null!;
}
