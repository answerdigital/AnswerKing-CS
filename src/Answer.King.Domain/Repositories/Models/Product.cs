using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Orders.Models;

namespace Answer.King.Domain.Repositories.Models;

public class Product
{
    public Product(string name, string description, double price, IList<CategoryId>? categories)
    {
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNegativeValue(nameof(price), price);

        this.Id = 0;
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.LastUpdated = this.CreatedOn = DateTime.UtcNow;
        this._Categories = categories ?? new List<CategoryId>();
    }

    private Product(long id,
        string name,
        string description,
        double price,
        DateTime createdOn,
        DateTime lastUpdated,
        IList<CategoryId>? categories,
        bool retired)
    {
        Guard.AgainstDefaultValue(nameof(id), id);
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNegativeValue(nameof(price), price);
        Guard.AgainstDefaultValue(nameof(createdOn), createdOn);
        Guard.AgainstDefaultValue(nameof(lastUpdated), lastUpdated);

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.CreatedOn = createdOn;
        this.LastUpdated = lastUpdated;
        this._Categories = categories ?? new List<CategoryId>();
        this.Retired = retired;
    }

    public long Id { get; }

    public string Name { get; set; }

    public string Description { get; set; }

    public double Price { get; set; }

    public DateTime CreatedOn { get; }

    public DateTime LastUpdated { get; private set; }

    private IList<CategoryId> _Categories { get; }

    public IReadOnlyCollection<CategoryId> Categories => (this._Categories as List<CategoryId>)!;

    public bool Retired { get; private set; }

    public void AddCategory(CategoryId categoryId)
    {
        if (this.Retired)
        {
            throw new ProductLifecycleException("Cannot add category to retired product.");
        }

        var exists = this._Categories.Any(p => p.Id == categoryId.Id);

        if (exists)
        {
            return;
        }

        this._Categories.Add(categoryId);

        this.LastUpdated = DateTime.UtcNow;
    }

    public void RemoveCategory(CategoryId categoryId)
    {
        if (this.Retired)
        {
            throw new ProductLifecycleException("Cannot remove category from retired product.");
        }

        var existing = this._Categories.SingleOrDefault(p => p.Id == categoryId.Id);

        if (existing == null)
        {
            return;
        }

        this._Categories.Remove(existing);

        this.LastUpdated = DateTime.UtcNow;
    }

    public void Retire()
    {
        this.Retired = true;
    }
}

[Serializable]
public class ProductLifecycleException : Exception
{
    public ProductLifecycleException(string message) : base(message)
    {
    }

    public ProductLifecycleException() : base()
    {
    }

    public ProductLifecycleException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
