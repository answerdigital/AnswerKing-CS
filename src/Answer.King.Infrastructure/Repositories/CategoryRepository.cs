using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Answer.King.Domain;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories;
using Answer.King.Infrastructure.SeedData;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    public CategoryRepository(ILiteDbConnectionFactory connections)
    {
        var db = connections.GetConnection();

        this.Collection = db.GetCollection<Category>();
        this.Collection.EnsureIndex("Products.Id");

        this.SeedData();
    }

    private ILiteCollection<Category> Collection { get; }


    public Task<IEnumerable<Category>> Get()
    {
        return Task.FromResult(this.Collection.FindAll());
    }

    public Task<Category?> Get(long id)
    {
        return Task.FromResult(this.Collection.FindOne(c => c.Id == id))!;
    }

    public Task Save(Category item)
    {
        return Task.FromResult(this.Collection.Upsert(item));
    }

    private void SeedData()
    {
        if (DataSeeded)
        {
            return;
        }

        var none = this.Collection.Count() < 1;
        if (none)
        {
            this.Collection.InsertBulk(CategoryData.Categories);
        }

        DataSeeded = true;
    }

    private static bool DataSeeded { get; set; }

    private class RepoCategory : IAggregateRoot
    {
        private RepoCategory (
        long id,
        string name,
        string description,
        DateTime createdOn,
        DateTime lastUpdated,
        IList<ProductId>? products,
        bool retired)
        {
            Guard.AgainstDefaultValue(nameof(id), id);
            Guard.AgainstNullOrEmptyArgument(nameof(name), name);
            Guard.AgainstNullOrEmptyArgument(nameof(description), description);
            Guard.AgainstDefaultValue(nameof(createdOn), createdOn);
            Guard.AgainstDefaultValue(nameof(lastUpdated), lastUpdated);

            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.CreatedOn = createdOn;
            this.LastUpdated = lastUpdated;
            this._Products = products ?? new List<ProductId>();
            this.Retired = retired;
        }

        public long Id { get; set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public DateTime CreatedOn { get; }

        public DateTime LastUpdated { get; private set; }

        private IList<ProductId> _Products { get; }

        public IReadOnlyCollection<ProductId> Products => (this._Products as List<ProductId>)!;

        public bool Retired { get; private set; }
    }
}
