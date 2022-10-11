using System.Collections.Generic;
using System.Threading.Tasks;
using AnswerKing.API.Controllers;
using AnswerKing.Services;
using AnswerKing.Services.DTOs;
using AnswerKing.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace AnswerKing.Tests.Controllers
{
    public class OrderControllerTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IOrderService _orderService = Substitute.For<IOrderService>();
        private readonly IItemService _itemService = Substitute.For<IItemService>();
        private OrderController _sut { get; }
        
        public OrderControllerTests(ITestOutputHelper output)
        {
            this._output = output;
            this._sut = new OrderController(this._orderService, this._itemService);
        }
        
        [Fact]
        public async Task GetAll_ShouldReturnOkObject_WhenListIsEmpty()
        {
            // Arrange
            this._orderService.GetAll().Returns(new List<OrderDto>());
            
            // Act
            var result = await this._sut.GetAll() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<List<OrderDto>>(result.Value);
            Assert.Empty(result.Value as List<OrderDto>);
        }
        
        [Fact]
        public async Task GetAll_ShouldReturnOkObject_WhenListIsNotEmpty()
        {
            // Arrange
            this._orderService.GetAll().Returns(new List<OrderDto>
            {
                new OrderDto()
                {
                    Id = 0,
                    Status = "Pending",
                    Address = "Test",
                    Items = new List<OrderItemDto>(),
                    Total = 0
                }
            });
            
            // Act
            var result = await this._sut.GetAll() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<List<OrderDto>>(result.Value);
            Assert.NotEmpty(result.Value as List<OrderDto>);
        }
        
        [Fact]
        public async Task GetById_ShouldReturnOkObject_WhenCategoryExists()
        {
            // Arrange
            var testOrderId = 0;
            var testOrderDto = new OrderDto()
            {
                Id = testOrderId,
                Status = "Pending",
                Address = "Test",
                Items = new List<OrderItemDto>(),
                Total = 0
            };
            this._orderService.GetById(testOrderId).Returns(testOrderDto);
            
            // Act
            var result = await this._sut.GetById(testOrderId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<OrderDto>(result.Value);
            Assert.NotNull(result.Value as OrderDto);
        }
        
        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var testOrderId = 0;
            this._orderService.GetById(testOrderId).Returns(null as OrderDto);
            
            // Act
            var result = await this._sut.GetById(testOrderId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
        
        [Fact]
        public async Task Create_ShouldReturnOkObject_WhenValidationPasses()
        {
            // Arrange
            var testOrderCreateDto = new OrderCreateDto
            {
                Address = "Test"
            };
            var testOrderDto = new OrderDto
            {
                Id = 0,
                Status = "Pending",
                Address = "Test",
                Items = new List<OrderItemDto>(),
                Total = 0
            };
            this._orderService.Create(testOrderCreateDto).Returns(testOrderDto);
            
            // Act
            var result = await this._sut.Create(testOrderCreateDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<OrderDto>(result.Value);
            Assert.NotNull(result.Value as OrderDto);
        }
        
        [Fact]
        public async Task Create_ShouldReturnBadRequestObject_WhenValidationFails()
        {
            // Arrange
            var testOrderCreateDto = new OrderCreateDto
            {
                Address = null
            };
            this._sut.ModelState.AddModelError("Address", "The Name field is required.");

            // Act
            var result = await this._sut.Create(testOrderCreateDto) as BadRequestObjectResult;
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task AddItem_ShouldReturnNoContent_WhenItemIsSuccessfullyAdded()
        {
            // Arrange
            var testOrderId = 0;
            var testItemId = 0;
            this._orderService.Exists(testOrderId).Returns(true);
            this._itemService.Exists(testItemId).Returns(true);
            this._orderService.AddItem(testOrderId, testItemId).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.AddItemToOrder(testOrderId, testItemId) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
        }
        
        [Fact]
        public async Task AddItem_ShouldReturnBadRequest_WhenItemIsAlreadyAdded()
        {
            // Arrange
            var testOrderId = 0;
            var testItemId = 0;
            this._orderService.Exists(testOrderId).Returns(true);
            this._itemService.Exists(testItemId).Returns(true);
            this._orderService.AddItem(testOrderId, testItemId).Returns(Task.FromResult(false));

            // Act
            var result = await this._sut.AddItemToOrder(testOrderId, testItemId) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task AddItem_ShouldReturnNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var testOrderId = 0;
            var testItemId = 0;
            this._orderService.Exists(testOrderId).Returns(false);
            this._itemService.Exists(testItemId).Returns(true);
            this._orderService.AddItem(testOrderId, testItemId).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.AddItemToOrder(testOrderId, testItemId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);        
        }
        
        [Fact]
        public async Task AddItem_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var testOrderId = 0;
            var testItemId = 0;
            this._orderService.Exists(testOrderId).Returns(true);
            this._itemService.Exists(testItemId).Returns(false);
            this._orderService.AddItem(testOrderId, testItemId).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.AddItemToOrder(testOrderId, testItemId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);        
        }
        
        [Fact]
        public async Task RemoveItem_ShouldReturnNoContent_WhenItemIsSuccessfullyAdded()
        {
            // Arrange
            var testOrderId = 0;
            var testItemId = 0;
            this._orderService.Exists(testOrderId).Returns(true);
            this._itemService.Exists(testItemId).Returns(true);
            this._orderService.RemoveItem(testOrderId, testItemId).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.RemoveItemFromOrder(testOrderId, testItemId) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
        }
        
        [Fact]
        public async Task RemoveItem_ShouldReturnBadRequest_WhenItemIsAlreadyAdded()
        {
            // Arrange
            var testOrderId = 0;
            var testItemId = 0;
            this._orderService.Exists(testOrderId).Returns(true);
            this._itemService.Exists(testItemId).Returns(true);
            this._orderService.RemoveItem(testOrderId, testItemId).Returns(Task.FromResult(false));

            // Act
            var result = await this._sut.RemoveItemFromOrder(testOrderId, testItemId) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task RemoveItem_ShouldReturnNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var testOrderId = 0;
            var testItemId = 0;
            this._orderService.Exists(testOrderId).Returns(false);
            this._itemService.Exists(testItemId).Returns(true);
            this._orderService.RemoveItem(testOrderId, testItemId).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.RemoveItemFromOrder(testOrderId, testItemId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);    
        }
        
        [Fact]
        public async Task RemoveItem_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var testOrderId = 0;
            var testItemId = 0;
            this._orderService.Exists(testOrderId).Returns(true);
            this._itemService.Exists(testItemId).Returns(false);
            this._orderService.RemoveItem(testOrderId, testItemId).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.RemoveItemFromOrder(testOrderId, testItemId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
        
                [Fact]
        public async Task UpdateItemQuantity_ShouldReturnNoContent_WhenQuantityIsSuccessfullyUpdated()
        {
            // Arrange
            var testOrderId = 0;
            var testItemId = 0;
            var testOrderItemDto = new OrderItemUpdateDto
            {
                Quantity = 0
            };
            this._orderService.Exists(testOrderId).Returns(true);
            this._itemService.Exists(testItemId).Returns(true);
            this._orderService.UpdateOrderItem(testOrderId, testItemId, testOrderItemDto).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.UpdateOrderItem(testOrderId, testItemId, testOrderItemDto) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
        }
        
        [Fact]
        public async Task UpdateItemQuantity_ShouldReturnBadRequest_WhenItemIsNotInOrder()
        {
            // Arrange
            var testOrderId = 0;
            var testItemId = 0;
            var testOrderItemDto = new OrderItemUpdateDto
            {
                Quantity = 0
            };
            this._orderService.Exists(testOrderId).Returns(true);
            this._itemService.Exists(testItemId).Returns(true);
            this._orderService.UpdateOrderItem(testOrderId, testItemId, testOrderItemDto).Returns(Task.FromResult(false));

            // Act
            var result = await this._sut.UpdateOrderItem(testOrderId, testItemId, testOrderItemDto) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task UpdateItemQuantity_ShouldReturnNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var testOrderId = 0;
            var testItemId = 0;
            var testOrderItemDto = new OrderItemUpdateDto
            {
                Quantity = 0
            };
            this._orderService.Exists(testOrderId).Returns(false);
            this._itemService.Exists(testItemId).Returns(true);
            this._orderService.UpdateOrderItem(testOrderId, testItemId, testOrderItemDto).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.UpdateOrderItem(testOrderId, testItemId, testOrderItemDto) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);    
        }
        
        [Fact]
        public async Task UpdateItemQuantity_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var testOrderId = 0;
            var testItemId = 0;
            var testOrderItemDto = new OrderItemUpdateDto
            {
                Quantity = 0
            };
            this._orderService.Exists(testOrderId).Returns(true);
            this._itemService.Exists(testItemId).Returns(false);
            this._orderService.UpdateOrderItem(testOrderId, testItemId, testOrderItemDto).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.UpdateOrderItem(testOrderId, testItemId, testOrderItemDto) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
    }
}