using System;
using System.Collections.Generic;
using Answer.King.Api.RequestModels;
using Answer.King.Api.Services;
using Answer.King.Domain.Repositories;
using Answer.King.Test.Common.CustomTraits;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;
using Category = Answer.King.Domain.Repositories.Models.Category;
using Order = Answer.King.Domain.Orders.Order;
using Product = Answer.King.Domain.Repositories.Models.Product;

namespace Answer.King.Api.UnitTests.Services
{
    [TestCategory(TestType.Unit)]
    public class OrderServiceTests
    {
        #region Create

        [Fact]
        public async void CreateOrder_InvalidProductsSubmitted_ThrowsException()
        {
            // Arrange
            var lineItem1 = new LineItem
            {
                Product = new ProductId {Id = Guid.NewGuid()},
                Quantity = 1
            };

            var lineItem2 = new LineItem
            {
                Product = new ProductId {Id = Guid.NewGuid()},
                Quantity = 1
            };

            var orderRequest = new RequestModels.Order
            {
                LineItems = new List<LineItem>(new[] {lineItem1, lineItem2})
            };

            // Act / Assert
            await Assert.ThrowsAsync<ProductInvalidException>(
                () => this.GetServiceUnderTest().CreateOrder(orderRequest));
        }


        [Fact]
        public async void CreateOrder_ValidOrderRequestRecieved_ReturnsOrder()
        {
            // Arrange
            var category = new Category(Guid.NewGuid(), "Cat 1", "desc");
            var products = new[]
            {
                new Product("product 1", "desc", 2.0, category),
                new Product("product 2", "desc", 4.0, category)
            };

            var orderRequest = new RequestModels.Order
            {
                LineItems = new List<LineItem>(new[]
                {
                    new LineItem {Product = new ProductId {Id = products[0].Id}, Quantity = 4},
                    new LineItem {Product = new ProductId {Id = products[1].Id}, Quantity = 1}
                })
            };

            this.ProductRepository.Get(Arg.Any<IList<Guid>>()).Returns(products);

            // Act
            var createdOrder = await this.GetServiceUnderTest().CreateOrder(orderRequest);

            // Assert
            Assert.Equal(2, createdOrder.LineItems.Count);
            Assert.Equal(12.0, createdOrder.OrderTotal);
        }

        #endregion

        #region Update

        [Fact]
        public async void UpdateOrder_InvalidOrderIdReceived_ReturnsNull()
        {
            // Arrange
            this.OrderRepository.Get(Arg.Any<Guid>()).ReturnsNull();

            // Act / Assert
            Assert.Null(await this.GetServiceUnderTest().UpdateOrder(Guid.NewGuid(), new RequestModels.Order()));
        }

        [Fact]
        public async void UpdateOrder_ValidOrderRequestReceived_ReturnsUpdatedOrder()
        {
            // Arrange
            var order = new Order();
            this.OrderRepository.Get(Arg.Any<Guid>()).Returns(order);

            var category = new Category(Guid.NewGuid(), "Cat 1", "desc");
            var products = new[]
            {
                new Product("product 1", "desc", 2.0, category),
                new Product("product 2", "desc", 4.0, category)
            };

            var orderRequest = new RequestModels.Order
            {
                LineItems = new List<LineItem>(new[]
                {
                    new LineItem {Product = new ProductId {Id = products[0].Id}, Quantity = 4},
                })
            };

            this.ProductRepository.Get(Arg.Any<IList<Guid>>()).Returns(products);

            // Act
            var updatedOrder = await this.GetServiceUnderTest().UpdateOrder(Guid.NewGuid(), orderRequest);

            // Assert
            await this.OrderRepository.Received().Save(Arg.Any<Order>());

            Assert.Equal(1, updatedOrder.LineItems.Count);
            Assert.Equal(8.0, updatedOrder.OrderTotal);
        }

        [Fact]
        public async void UpdateOrder_InvalidProductReceivedInOrder_ThrowsException()
        {
            // Arrange
            var order = new Order();
            this.OrderRepository.Get(Arg.Any<Guid>()).Returns(order);

            var category = new Category(Guid.NewGuid(), "Cat 1", "desc");
            var products = new[]
            {
                new Product("product 1", "desc", 2.0, category),
                new Product("product 2", "desc", 4.0, category)
            };

            var orderRequest = new RequestModels.Order
            {
                LineItems = new List<LineItem>(new[]
                {
                    new LineItem {Product = new ProductId {Id = Guid.NewGuid()}, Quantity = 4}
                })
            };

            this.ProductRepository.Get(Arg.Any<IList<Guid>>()).Returns(products);

            // Act / Assert
            await Assert.ThrowsAsync<ProductInvalidException>(() =>
                this.GetServiceUnderTest().UpdateOrder(Guid.NewGuid(), orderRequest));
        }

        #endregion

        #region Get

        [Fact]
        public async void GetOrder_ValidOrderId_ReturnsOrder()
        {
            // Arrange
            var order = new Order();
            this.OrderRepository.Get(order.Id).Returns(order);

            // Act
            var actualOrder = await this.GetServiceUnderTest().GetOrder(order.Id);

            // Assert
            Assert.Equal(order, actualOrder);
            await this.OrderRepository.Received().Get(order.Id);
        }

        [Fact]
        public async void GetOrders_ReturnsAllOrders()
        {
            // Arrange
            var orders = new[]
            {
                new Order(), new Order(),
            };

            this.OrderRepository.Get().Returns(orders);

            // Act
            var actualOrders = await this.GetServiceUnderTest().GetOrders();

            // Assert
            Assert.Equal(orders, actualOrders);
            await this.OrderRepository.Received().Get();
        }

        #endregion

        #region Cancel

        [Fact]
        public async void CancelOrder_InvalidOrderIdReceived_ReturnsNull()
        {
            // Arrange
            this.OrderRepository.Get(Arg.Any<Guid>()).ReturnsNull();

            // Act
            var cancelOrder = await this.GetServiceUnderTest().CancelOrder(Guid.NewGuid());

            // Assert
            Assert.Null(cancelOrder);
        }

        #endregion

        #region Setup

        private readonly IOrderRepository OrderRepository = Substitute.For<IOrderRepository>();
        private readonly IProductRepository ProductRepository = Substitute.For<IProductRepository>();

        private IOrderService GetServiceUnderTest()
        {
            return new OrderService(this.OrderRepository, this.ProductRepository);
        }

        #endregion
    }
}