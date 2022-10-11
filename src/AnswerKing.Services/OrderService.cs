using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnswerKing.Core.Entities;
using AnswerKing.Repositories.Interfaces;
using AnswerKing.Services.DTOs;
using AnswerKing.Services.Interfaces;

namespace AnswerKing.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository;
        }

        public async Task<List<OrderDto>> GetAll()
        {
            var orderEntities = await this._orderRepository.GetAll();

            var orderDtos = orderEntities
                .Select(orderEntity => new OrderDto
                {
                    Id = orderEntity.Id,
                    Status = orderEntity.Status,
                    Address = orderEntity.Address,
                    Total = CalculateTotal(orderEntity.Items),
                    Items = orderEntity.Items
                        .Select(orderItemEntity => new OrderItemDto
                            {
                                Id = orderItemEntity.Id,
                                Name = orderItemEntity.Name,
                                Price = orderItemEntity.Price,
                                Description = orderItemEntity.Description,
                                Quantity = orderItemEntity.Quantity
                            })
                            .ToList()
                })
                .ToList();

            return orderDtos;
        }

        public async Task<OrderDto?> GetById(int orderId)
        {
            var orderEntity = await this._orderRepository.GetById(orderId);
            if (orderEntity is null)
            {
                return null;
            }

            var orderDto = new OrderDto
            {
                Id = orderEntity.Id,
                Status = orderEntity.Status,
                Address = orderEntity.Address,
                Total = CalculateTotal(orderEntity.Items),
                Items = orderEntity.Items
                    .Select(orderItemEntity => new OrderItemDto
                    {
                        Id = orderItemEntity.Id,
                        Name = orderItemEntity.Name,
                        Price = orderItemEntity.Price,
                        Description = orderItemEntity.Description,
                        Quantity = orderItemEntity.Quantity
                    })
                    .ToList()
            };

            return orderDto;
        }

        public async Task<OrderDto?> Create(OrderCreateDto createDto)
        {
            var orderId = await this._orderRepository.Create(new OrderEntity
            {
                Address = createDto.Address,
            });

            var orderEntity = await this._orderRepository.GetById(orderId);
            
            return new OrderDto
            {
                Id = orderEntity.Id,
                Status = orderEntity.Status,
                Address = orderEntity.Address,
                Total = CalculateTotal(orderEntity.Items),
                Items =new List<OrderItemDto>()
            };
        }
        
        public async Task<bool> Exists(int orderId)
        {
            var itemEntity = await this._orderRepository.GetById(orderId);

            return itemEntity is not null;
        }

        public async Task<bool> AddItem(int orderId, int itemId)
        {
            var orderEntity = await this._orderRepository.GetById(orderId);

            if (orderEntity?.Items.FirstOrDefault(item => item.Id == itemId) is not null)
            {
                return false;
            }

            return await this._orderRepository.AddItem(orderId, itemId);
        }

        public async Task<bool> RemoveItem(int orderId, int itemId)
        {
            var orderEntity = await this._orderRepository.GetById(orderId);
            
            if (orderEntity?.Items.FirstOrDefault(item => item.Id == itemId) is null)
            {
                return false;
            }
            
            return await this._orderRepository.RemoveItem(orderId, itemId);
        }

        public async Task<bool> UpdateOrderItem(int orderId, int itemId, OrderItemUpdateDto updateDto)
        {
            var orderEntity = await this._orderRepository.GetById(orderId);
            
            if (orderEntity?.Items.FirstOrDefault(item => item.Id == itemId) is null)
            {
                return false;
            }

            if (updateDto.Quantity < 1)
            {
                return await RemoveItem(orderId, itemId);
            }
            
            return await this._orderRepository.UpdateItemQuantity(orderId, itemId, updateDto.Quantity);
        }

        public decimal CalculateTotal(List<OrderItemEntity> orderItems)
        {
            return orderItems.Select(item => item.Price * item.Quantity).Sum();
        }
    }
}