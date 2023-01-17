namespace Answer.King.Api.RequestModels;

public record CategoryId
{
    // public CategoryId(long value)
    // {
    //     this.Id = value;
    // }
    public long Id { get; init; }

    public static implicit operator long(CategoryId id) => id.Id;
}
