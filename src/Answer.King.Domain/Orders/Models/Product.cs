namespace Answer.King.Domain.Orders.Models;

public class Product
{
    public Product(long id, string name, string description, double price, IList<Category> categories, IList<Tag> tags)
    {
        Guard.AgainstDefaultValue(nameof(id), id);
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNegativeValue(nameof(price), price);
        Guard.AgainstNullArgument(nameof(categories), categories);
        Guard.AgainstNullArgument(nameof(tags), tags);

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this._Categories = categories;
        this._Tags = tags;
    }

    public long Id { get; }

    public string Name { get; }

    public string Description { get; }

    public double Price { get; }

    private IList<Category> _Categories { get; }

    public IReadOnlyCollection<Category> Categories => (this._Categories as List<Category>)!;

    private IList<Tag> _Tags { get; }

    public IReadOnlyCollection<Tag> Tags => (this._Tags as List<Tag>)!;
}
