﻿using System.Runtime.Serialization;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Orders.Models;

namespace Answer.King.Domain.Inventory;

// Todo: look at custom deserialisation: https://stackoverflow.com/questions/42336751/custom-deserialization
public class Category : IAggregateRoot
{
    public Category(string name, string description, IList<ProductId> products)
    {
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNullArgument(nameof(products), products);

        this.Id = 0;
        this.Name = name;
        this.Description = description;
        this.LastUpdated = this.CreatedOn = DateTime.UtcNow;
        this._products = new HashSet<ProductId>(products);
        this.Retired = false;
    }

    // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
    private Category(
        long id,
        string name,
        string description,
        DateTime createdOn,
        DateTime lastUpdated,
        IList<ProductId> products,
        bool retired)
    {
#pragma warning restore IDE0051 // Remove unused private members
        Guard.AgainstDefaultValue(nameof(id), id);
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstDefaultValue(nameof(createdOn), createdOn);
        Guard.AgainstDefaultValue(nameof(lastUpdated), lastUpdated);
        Guard.AgainstNullArgument(nameof(products), products);

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.CreatedOn = createdOn;
        this.LastUpdated = lastUpdated;
        this._products = new HashSet<ProductId>(products);
        this.Retired = retired;
    }

    public long Id { get; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public DateTime CreatedOn { get; }

    public DateTime LastUpdated { get; private set; }

    private HashSet<ProductId> _products { get; }

    public IReadOnlyCollection<ProductId> Products => this._products;

    public bool Retired { get; private set; }

    public void Rename(string name, string description)
    {
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);

        this.Name = name;
        this.Description = description;
        this.LastUpdated = DateTime.UtcNow;
    }

    public void AddProduct(ProductId productId)
    {
        if (this.Retired)
        {
            throw new CategoryLifecycleException("Cannot add product to retired catgory.");
        }

        if (this._products.Add(productId))
        {
            this.LastUpdated = DateTime.UtcNow;
        }
    }

    public void RemoveProduct(ProductId productId)
    {
        if (this.Retired)
        {
            throw new CategoryLifecycleException("Cannot remove product from retired catgory.");
        }

        if (this._products.Remove(productId))
        {
            this.LastUpdated = DateTime.UtcNow;
        }
    }

    public void RetireCategory()
    {
        if (this.Retired)
        {
            throw new CategoryLifecycleException("The category is already retired.");
        }

        if (this._products.Count > 0)
        {
            throw new CategoryLifecycleException(
                $"Cannot retire category whilst there are still products assigned. {string.Join(',', this.Products.Select(p => p.Value))}");
        }

        this.Retired = true;

        this.LastUpdated = DateTime.UtcNow;
    }
}

[Serializable]
public class CategoryLifecycleException : Exception
{
    public CategoryLifecycleException(string message) : base(message)
    {
    }

    public CategoryLifecycleException() : base()
    {
    }

    public CategoryLifecycleException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected CategoryLifecycleException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
