namespace Answer.King.Domain.Repositories.Models;

public class CategoryId
{
    public CategoryId(long id)
    {
        Guard.AgainstDefaultValue(nameof(id), id);

        this.Id = id;
    }

    public long Id { get; }
}
