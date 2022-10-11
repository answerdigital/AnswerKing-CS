using System.Collections.Generic;
using System.Threading.Tasks;
using AnswerKing.Core.Entities;

namespace AnswerKing.Repositories.Interfaces
{
    public interface IItemRepository : IGenericRepository<ItemEntity>
    {
        Task<int?> Create(ItemEntity itemEntity);
        Task<List<ItemEntity>> GetByCategory(int categoryId);
        Task<bool> AddCategory(int itemId, int categoryId);
        Task<bool> RemoveCategory(int itemId, int categoryId);
    }
}