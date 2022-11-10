using Answer.King.Domain.Inventory;

namespace Answer.King.Domain.Repositories;

public interface ICategoryRepository : IAggregateRepository<Category>
{
    Task<Category?> GetByProductId (long productId);
}
