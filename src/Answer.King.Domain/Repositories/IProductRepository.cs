using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> Get(long id);

    Task<IEnumerable<Product>> Get();

    Task<IEnumerable<Product>> Get(IEnumerable<long> ids);

    Task AddOrUpdate(Product product);
}
