using Answer.King.Domain.Inventory;

namespace Answer.King.Domain.Repositories;

public interface ITagRepository : IAggregateRepository<Tag>
{
    Task<IEnumerable<Tag>> GetByProductId(long productId);

    Task<IEnumerable<Tag>> GetByProductId(params long[] productIds);
}
