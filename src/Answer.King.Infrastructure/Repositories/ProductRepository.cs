using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Repositories.Models;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    public ProductRepository(ILiteDbConnectionFactory connections)
    {
        var db = connections.GetConnection();

        this.Collection = db.GetCollection<Product>();
    }

    private ILiteCollection<Product> Collection { get; }

    public Task<Product?> Get(long id)
    {
        return Task.FromResult(this.Collection.FindOne(c => c.Id == id))!;
    }

    public Task<IEnumerable<Product>> Get()
    {
        return Task.FromResult(this.Collection.FindAll());
    }

    public Task<IEnumerable<Product>> Get(IEnumerable<long> ids)
    {
        return Task.FromResult(this.Collection.Find(p => ids.Contains(p.Id)));
    }

    public Task AddOrUpdate(Product product)
    {
        return Task.FromResult(this.Collection.Upsert(product));
    }
}
