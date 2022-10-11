using System.Threading.Tasks;
using AnswerKing.Core.Entities;

namespace AnswerKing.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<OrderEntity>
    {
        Task<int> Create(OrderEntity orderEntity);
        Task<bool> AddItem(int orderId, int itemId);
        Task<bool> RemoveItem(int orderId, int itemId);
        Task<bool> UpdateItemQuantity(int orderId, int itemId, int quantity);
    }
}