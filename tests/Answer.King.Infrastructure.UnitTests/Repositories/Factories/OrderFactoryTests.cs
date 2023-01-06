using System.Reflection;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Orders.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using Xunit;
using Product = Answer.King.Domain.Orders.Models.Product;

namespace Answer.King.Infrastructure.UnitTests.Repositories.Factories;

[UsesVerify]
[TestCategory(TestType.Unit)]
public class OrderFactoryTests
{
    private static OrderFactory OrderFactory = new();

    [Fact]
    public Task CreateOrder_ConstructorExists_ReturnsOrder()
    {
        // Arrange / Act
        var result = OrderFactory.CreateOrder(1, DateTime.UtcNow, DateTime.UtcNow, OrderStatus.Created, new List<LineItem>());

        // Assert
        Assert.IsType<Order>(result);
        return Verify(result);
    }

    [Fact]
    public void CreateOrder_ConstructorNotFound_ReturnsException()
    {
        // Arrange
        var orderFactoryConstructorPropertyInfo =
        typeof(OrderFactory).GetProperty("OrderConstructor", BindingFlags.Instance | BindingFlags.NonPublic);

        var constructor = orderFactoryConstructorPropertyInfo?.GetValue(OrderFactory);

        var wrongConstructor = typeof(Domain.Inventory.Category).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        orderFactoryConstructorPropertyInfo?.SetValue(OrderFactory, wrongConstructor);

        // Act // Assert
        Assert.Throws<TargetParameterCountException>(() =>
            OrderFactory.CreateOrder(1, DateTime.UtcNow, DateTime.UtcNow, OrderStatus.Created, new List<LineItem>()));

        //Reset static constructor to correct value
        orderFactoryConstructorPropertyInfo?.SetValue(OrderFactory, constructor);
    }
}
