﻿using Answer.King.Api.RequestModels;
using Answer.King.Api.Services;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;
using CategoryId = Answer.King.Domain.Repositories.Models.CategoryId;
using TagId = Answer.King.Domain.Repositories.Models.TagId;
using Order = Answer.King.Domain.Orders.Order;
using Product = Answer.King.Domain.Repositories.Models.Product;

namespace Answer.King.Api.UnitTests.Services;

[TestCategory(TestType.Unit)]
public class OrderServiceTests
{
    private static CategoryFactory CategoryFactory { get; } = new();

    private static ProductFactory ProductFactory { get; } = new();

    #region Create

    [Fact]
    public async Task CreateOrder_InvalidProductsSubmitted_ThrowsException()
    {
        // Arrange
        var lineItem1 = new LineItem
        {
            ProductId = 1,
            Quantity = 1
        };

        var lineItem2 = new LineItem
        {
            ProductId = 1,
            Quantity = 1
        };

        var orderRequest = new RequestModels.Order
        {
            LineItems = new List<LineItem>(new[] { lineItem1, lineItem2 })
        };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<ProductInvalidException>(
            () => sut.CreateOrder(orderRequest));
    }


    [Fact]
    public async Task CreateOrder_ValidOrderRequestRecieved_ReturnsOrder()
    {
        // Arrange
        var categoryIds = new List<CategoryId> { new(1) };
        var tagIds = new List<TagId> { new(1) };
        var products = new[]
        {
            ProductFactory.CreateProduct(1, "product 1", "desc", 2.0, categoryIds, tagIds, false),
            ProductFactory.CreateProduct(2, "product 2", "desc", 4.0, categoryIds, tagIds, false)
        };

        var orderRequest = new RequestModels.Order
        {
            LineItems = new List<LineItem>(new[]
            {
                new LineItem { ProductId = products[0].Id, Quantity = 4 },
                new LineItem { ProductId = products[1].Id, Quantity = 1 }
            })
        };

        var now = DateTime.UtcNow;
        var categories = new[]
        {
            CategoryFactory.CreateCategory(
                1,
                "category 1",
                "desc",
                now,
                now,
                new[] { new ProductId(1) },
                false)
        };

        this.ProductRepository.Get(Arg.Any<IList<long>>()).Returns(products);
        this.CategoryRepository.GetByProductId(Arg.Any<long[]>()).Returns(categories);

        // Act
        var sut = this.GetServiceUnderTest();
        var createdOrder = await sut.CreateOrder(orderRequest);

        // Assert
        Assert.Equal(2, createdOrder.LineItems.Count);
        Assert.Equal(12.0, createdOrder.OrderTotal);
    }

    #endregion

    #region Update

    [Fact]
    public async Task UpdateOrder_InvalidOrderIdReceived_ReturnsNull()
    {
        // Arrange
        this.OrderRepository.Get(Arg.Any<long>()).ReturnsNull();

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        Assert.Null(await sut.UpdateOrder(1, new RequestModels.Order()));
    }

    [Fact]
    public async Task UpdateOrder_ValidOrderRequestReceived_ReturnsUpdatedOrder()
    {
        // Arrange
        var order = new Order();
        this.OrderRepository.Get(Arg.Any<long>()).Returns(order);

        var categoryIds = new List<CategoryId> { new(1) };
        var tagIds = new List<TagId> { new(1) };
        var products = new[]
        {
            ProductFactory.CreateProduct(1, "product 1", "desc", 2.0, categoryIds, tagIds, false),
            ProductFactory.CreateProduct(2, "product 2", "desc", 4.0, categoryIds, tagIds, false)
        };

        var orderRequest = new RequestModels.Order
        {
            LineItems = new List<LineItem>(new[]
            {
                new LineItem {ProductId = products[0].Id , Quantity = 4},
            })
        };

        var now = DateTime.UtcNow;
        var categories = new[]
        {
            CategoryFactory.CreateCategory(
                1,
                "category 1",
                "desc",
                now,
                now,
                new[] { new ProductId(1) },
                false)
        };

        this.ProductRepository.Get(Arg.Any<IList<long>>()).Returns(products);
        this.CategoryRepository.GetByProductId(Arg.Any<long[]>()).Returns(categories);

        // Act
        var sut = this.GetServiceUnderTest();
        var updatedOrder = await sut.UpdateOrder(1, orderRequest);

        // Assert
        await this.OrderRepository.Received().Save(Arg.Any<Order>());

        Assert.Equal(1, updatedOrder!.LineItems.Count);
        Assert.Equal(8.0, updatedOrder.OrderTotal);
    }

    [Fact]
    public async Task UpdateOrder_InvalidProductReceivedInOrder_ThrowsException()
    {
        // Arrange
        var order = new Order();
        this.OrderRepository.Get(Arg.Any<long>()).Returns(order);

        var products = new[]
        {
            new Product("product 1", "desc", 2.0),
            new Product("product 2", "desc", 4.0)
        };

        var orderRequest = new RequestModels.Order
        {
            LineItems = new List<LineItem>(new[]
            {
                new LineItem { ProductId = 1, Quantity = 4 }
            })
        };

        this.ProductRepository.Get(Arg.Any<IList<long>>()).Returns(products);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<ProductInvalidException>(() =>
            sut.UpdateOrder(1, orderRequest));
    }

    #endregion

    #region Get

    [Fact]
    public async Task GetOrder_ValidOrderId_ReturnsOrder()
    {
        // Arrange
        var order = new Order();
        this.OrderRepository.Get(order.Id).Returns(order);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualOrder = await sut.GetOrder(order.Id);

        // Assert
        Assert.Equal(order, actualOrder);
        await this.OrderRepository.Received().Get(order.Id);
    }

    [Fact]
    public async Task GetOrders_ReturnsAllOrders()
    {
        // Arrange
        var orders = new[]
        {
            new Order(), new Order(),
        };

        this.OrderRepository.Get().Returns(orders);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualOrders = await sut.GetOrders();

        // Assert
        Assert.Equal(orders, actualOrders);
        await this.OrderRepository.Received().Get();
    }

    #endregion

    #region Cancel

    [Fact]
    public async Task CancelOrder_InvalidOrderIdReceived_ReturnsNull()
    {
        // Arrange
        this.OrderRepository.Get(Arg.Any<long>()).ReturnsNull();

        // Act
        var sut = this.GetServiceUnderTest();
        var cancelOrder = await sut.CancelOrder(1);

        // Assert
        Assert.Null(cancelOrder);
    }

    #endregion

    #region Setup

    private readonly IOrderRepository OrderRepository = Substitute.For<IOrderRepository>();
    private readonly IProductRepository ProductRepository = Substitute.For<IProductRepository>();
    private readonly ICategoryRepository CategoryRepository = Substitute.For<ICategoryRepository>();

    private IOrderService GetServiceUnderTest()
    {
        return new OrderService(this.OrderRepository, this.ProductRepository);
    }

    #endregion
}
