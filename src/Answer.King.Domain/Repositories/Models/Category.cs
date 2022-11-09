namespace Answer.King.Domain.Repositories.Models;

public class Category
{
    public Category (string name, string description)
    {
        Guard.AgainstNullOrWhitespaceArgument(nameof(name), name);
        Guard.AgainstNullOrWhitespaceArgument(nameof(description), description);

        this.Id = 0;
        this.Name = name;
        this.Description = description;
    }

    public Category(long id, string name, string description)
    {
        Guard.AgainstDefaultValue(nameof(id), id);
        Guard.AgainstNullOrWhitespaceArgument(nameof(name), name);
        Guard.AgainstNullOrWhitespaceArgument(nameof(description), description);

        this.Id = id;
        this.Name = name;
        this.Description = description;
    }

    public long Id { get; }

    public string Name { get; }

    public string Description { get; }
}
