using Answer.King.Domain.Inventory;

namespace Answer.King.Domain.Repositories;

public interface ICategoryRepository : IAggregateRepository<Category>
{
    public Task<Category?> GetByProductId (long productId);
}
