using System.Collections.Generic;
using System.Threading.Tasks;
using AnswerKing.Core.Entities;
using AnswerKing.Services.DTOs;

namespace AnswerKing.Services.Interfaces
{
    public interface IOrderService
    {
        public Task<List<OrderDto>> GetAll();
        public Task<OrderDto?> GetById(int orderId);
        public Task<OrderDto?> Create(OrderCreateDto createDto);
        Task<bool> Exists(int orderId);
        Task<bool> AddItem(int orderId, int itemId);
        Task<bool> RemoveItem(int orderId, int itemId);
        Task<bool> UpdateOrderItem(int orderId, int itemId, OrderItemUpdateDto updateDto);
        decimal CalculateTotal(List<OrderItemEntity> orderItems);
    }
}