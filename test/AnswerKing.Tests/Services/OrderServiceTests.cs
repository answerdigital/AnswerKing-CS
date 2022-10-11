using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnswerKing.Core.Entities;
using AnswerKing.Repositories.Interfaces;
using AnswerKing.Services;
using AnswerKing.Services.DTOs;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Xunit;
using Xunit.Abstractions;

namespace AnswerKing.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
        private OrderService _sut { get; }

        public OrderServiceTests(ITestOutputHelper output)
        {
            this._output = output;
            this._sut = new OrderService(this._orderRepository);
        }

        [Fact]
        public async Task GetAll_ShouldReturnItemDtoList_WhenListIsEmpty()
        {
            // Arrange
            var testOrderEntities = new List<OrderEntity>();
            this._orderRepository.GetAll().Returns(testOrderEntities);
            
            // Act
            var result = await this._sut.GetAll();
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<OrderDto>>(result);
            Assert.Empty(result);
        }
        
        [Fact]
        public async Task GetAll_ShouldReturnItemDtoList_WhenListIsNotEmpty()
        {
            // Arrange
            var testOrderEntities = new List<OrderEntity>()
            {
                new OrderEntity
                {
                    Id = 0,
                    Address = "test",
                    Status = "Pending",
                    Items = new List<OrderItemEntity>()
                }
            };
            this._orderRepository.GetAll().Returns(testOrderEntities);
            
            // Act
            var result = await this._sut.GetAll();
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<OrderDto>>(result);
            Assert.NotEmpty(result);
        }
        
        [Fact]
        public async Task GetById_ShouldReturnOrderDto_WhenOrderExists()
        {
            // Arrange
            var testOrderId = 0;
            var testOrderEntity = new OrderEntity
            {
                Id = testOrderId,
                Address = "test",
                Status = "Pending",
                Items = new List<OrderItemEntity>()
            };
            this._orderRepository.GetById(testOrderId).Returns(testOrderEntity);
            
            // Act
            var result = await this._sut.GetById(testOrderId);
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<OrderDto>(result);
        }
        
        [Fact]
        public async Task GetById_ShouldReturnNull_WhenItemDoesNotExist()
        {
            // Arrange
            var testOrderId = 0;
            this._orderRepository.GetById(testOrderId).Returns(null as OrderEntity);
            
            // Act
            var result = await this._sut.GetById(testOrderId);
            
            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task Create_ShouldReturnOrderDto_WhenOrderIsCreated()
        {
            // Arrange
            var testOrderId = 0;
            var testOrderCreateDto = new OrderCreateDto
            {
                Address = "test"
            };
            var testOrderEntity = new OrderEntity
            {
                Id = testOrderId,
                Address = testOrderCreateDto.Address,
                Status = "Pending",
                Items = new List<OrderItemEntity>()
            };
            this._orderRepository.Create(testOrderEntity).ReturnsForAnyArgs(testOrderId);
            this._orderRepository.GetById(testOrderId).Returns(testOrderEntity);

            // Act
            var result = await this._sut.Create(testOrderCreateDto);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OrderDto>(result);
            Assert.Equal(testOrderId, result.Id);
        }
        
        [Fact]
        public async Task Exists_ShouldReturnTrue_WhenOrderDoesExist()
        {
            // Arrange
            var testOrderId = 0;
            var testOrderEntity = new OrderEntity
            {
                Id = testOrderId,
                Address = "test",
                Status = "Pending",
                Items = new List<OrderItemEntity>()
            };
            this._orderRepository.GetById(testOrderId).Returns(testOrderEntity);

            // Act
            var result = await this._sut.Exists(testOrderId);

            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public async Task Exists_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            // Arrange
            var testOrderId = 0;
            this._orderRepository.GetById(testOrderId).Returns(null as OrderEntity);

            // Act
            var result = await this._sut.Exists(testOrderId);

            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public async Task AddItem_ShouldReturnTrue_WhenItemIsAdded()
        {
            // Arrange
            var testOrderId = 0;
            var testOrderItemId = 0;
            var testOrderEntity = new OrderEntity
            {
                Id = testOrderId,
                Address = "test",
                Status = "Pending",
                Items = new List<OrderItemEntity>()
            };
            this._orderRepository.GetById(testOrderId).Returns(testOrderEntity);
            this._orderRepository.AddItem(testOrderId, testOrderItemId).Returns(true);

            // Act
            var result = await this._sut.AddItem(testOrderId, testOrderItemId);

            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public async Task AddItem_ShouldReturnFalse_WhenItemIsAlreadyInOrder()
        {
            // Arrange
            var testOrderId = 0;
            var testItemId = 0;
            var testOrderItemEntity = new OrderItemEntity
            {
                Id = 0,
                Name = "test",
                Description = null,
                Price = 0,
            };
            var testOrderEntity = new OrderEntity
            {
                Id = testOrderId,
                Address = "test",
                Status = "Pending",
                Items = new List<OrderItemEntity> {testOrderItemEntity}
            };
            this._orderRepository.GetById(testOrderId).Returns(testOrderEntity);
            this._orderRepository.AddItem(testOrderId, testItemId).Returns(true);

            // Act
            var result = await this._sut.AddItem(testOrderId, testItemId);

            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public async Task RemoveItem_ShouldReturnTrue_WhenItemIsRemoved()
        {
            // Arrange
            var testOrderId = 0;
            var testOrderItemId = 0;
            var testOrderItemEntity = new OrderItemEntity
            {
                Id = 0,
                Name = "test",
                Description = null,
                Price = 0,
            };
            var testOrderEntity = new OrderEntity()
            {
                Id = testOrderId,
                Address = "test",
                Status = "Pending",
                Items = new List<OrderItemEntity> {testOrderItemEntity}
            };
            this._orderRepository.GetById(testOrderId).Returns(testOrderEntity);
            this._orderRepository.RemoveItem(testOrderId, testOrderItemId).Returns(true);

            // Act
            var result = await this._sut.RemoveItem(testOrderId, testOrderItemId);

            // Assert
            Assert.True(result);        }
        
        [Fact]
        public async Task RemoveItem_ShouldReturnFalse_WhenItemIsNotInOrder()
        {
            // Arrange
            var testOrderId = 0;
            var testOrderItemId = 0;
            var testOrderEntity = new OrderEntity
            {
                Id = testOrderId,
                Address = "test",
                Status = "Pending",
                Items = new List<OrderItemEntity>( )
            };
            this._orderRepository.GetById(testOrderId).Returns(testOrderEntity);
            this._orderRepository.RemoveItem(testOrderId, testOrderItemId).Returns(true);

            // Act
            var result = await this._sut.RemoveItem(testOrderId, testOrderItemId);

            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public async Task UpdateOrderItem_ShouldReturnTrue_WhenItemIsUpdated()
        {
            // Arrange
            var testOrderItemId = 0;
            var testOrderItemEntity = new OrderItemEntity
            {
                Id = testOrderItemId,
                Name = "Burger",
                Description = null,
                Price = 5,
                Quantity = 1
            };
            var testOrderId = 0;
            var testOrderEntity = new OrderEntity
            {
                Id = testOrderId,
                Address = "test",
                Status = "Pending",
                Items = new List<OrderItemEntity> {testOrderItemEntity}
            };
            var testOrderItemUpdateDto = new OrderItemUpdateDto
            {
                Quantity = 5
            };
            this._orderRepository.GetById(testOrderId).Returns(testOrderEntity);
            this._orderRepository.UpdateItemQuantity(testOrderId, testOrderItemId, testOrderItemUpdateDto.Quantity)
                .Returns(true);
            
            // Act
            var result = await this._sut.UpdateOrderItem(testOrderId, testOrderItemId, testOrderItemUpdateDto);
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public async Task UpdateOrderItem_ShouldReturnFalse_WhenItemIsNotInOrder()
        {
            // Arrange
            var testOrderItemId = 0;
            var testOrderId = 0;
            var testOrderEntity = new OrderEntity
            {
                Id = testOrderId,
                Address = "test",
                Status = "Pending",
                Items = new List<OrderItemEntity>()
            };
            var testOrderItemUpdateDto = new OrderItemUpdateDto
            {
                Quantity = 5
            };
            this._orderRepository.GetById(testOrderId).Returns(testOrderEntity);
            this._orderRepository.UpdateItemQuantity(testOrderId, testOrderItemId, testOrderItemUpdateDto.Quantity)
                .Returns(true);
            
            // Act
            var result = await this._sut.UpdateOrderItem(testOrderId, testOrderItemId, testOrderItemUpdateDto);
            
            // Assert
            Assert.False(result);
        }

        [Fact]
        public async void UpdateOrderItem_ShouldCallRemoveItemFromOrder_WhenQuantityIsZero()
        {
            // Arrange
            var testOrderItemId = 0;
            var testOrderItemEntity = new OrderItemEntity
            {
                Id = testOrderItemId,
                Name = "Burger",
                Description = null,
                Price = 5,
                Quantity = 1
            };
            var testOrderId = 0;
            var testOrderEntity = new OrderEntity
            {
                Id = testOrderId,
                Address = "test",
                Status = "Pending",
                Items = new List<OrderItemEntity> {testOrderItemEntity}
            };
            var testOrderItemUpdateDto = new OrderItemUpdateDto
            {
                Quantity = 0
            };
            this._orderRepository.GetById(testOrderId).Returns(testOrderEntity);
            this._orderRepository.RemoveItem(testOrderId, testOrderItemId).Returns(true);
            
            // Act
            var result = await this._sut.UpdateOrderItem(testOrderId, testOrderItemId, testOrderItemUpdateDto);
            
            // Assert
            await this._orderRepository.Received().RemoveItem(testOrderId, testOrderItemId);
        }
        
        [Fact]
        public void CalculateTotal_ShouldCalculateTotalPriceOfItemsInAnOrder_Always()
        {
            // Arrange
            var testOrderItems = new List<OrderItemEntity>
            {
                new OrderItemEntity
                {
                    Id = 1,
                    Name = "test1",
                    Description = null,
                    Price = 1.01M,
                    Quantity = 5
                },
                new OrderItemEntity
                {
                    Id = 2,
                    Name = "test2",
                    Description = null,
                    Price = 0.99M,
                    Quantity = 5
                },
                new OrderItemEntity
                {
                    Id = 3,
                    Name = "test3",
                    Description = null,
                    Price = 0,
                    Quantity = 1
                }
            };

            // Act
            var result = this._sut.CalculateTotal(testOrderItems);

            // Assert
            Assert.Equal(10M, result);
        }
    }
}