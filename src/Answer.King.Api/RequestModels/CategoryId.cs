using Answer.King.Domain;

namespace Answer.King.Api.RequestModels;

public record CategoryId
{
    public CategoryId(long value)
    {
        Guard.AgainstDefaultValue(nameof(value), value);
        this.Id = value;
    }

    public long Id { get; init; }

    public static implicit operator long(CategoryId id) => id.Id;
}
