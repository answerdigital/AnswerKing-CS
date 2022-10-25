using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> Get(Guid id);

    Task<IEnumerable<Product>> Get();

    Task<IEnumerable<Product>> Get(IEnumerable<Guid> ids);

    Task AddOrUpdate(Product product);
}
