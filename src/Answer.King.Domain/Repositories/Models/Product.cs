namespace Answer.King.Domain.Repositories.Models;

public class Product
{
    public Product(string name, string description, double price, Category category)
    {
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNegativeValue(nameof(price), price);
        Guard.AgainstNullArgument(nameof(category), category);

        this.Id = Guid.NewGuid();
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.Category = category;
    }

    private Product(Guid id, string name, string description, double price, Category category, bool retired)
    {
        Guard.AgainstDefaultValue(nameof(id), id);
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNegativeValue(nameof(price), price);
        Guard.AgainstNullArgument(nameof(category), category);

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.Category = category;
        this.Retired = retired;
    }

    public Guid Id { get; }

    public string Name { get; set; }

    public string Description { get; set; }

    public double Price { get; set; }

    public Category Category { get; set; }

    public bool Retired { get; private set; }

    public void Retire()
    {
        this.Retired = true;
    }
}
