using System.Reflection;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Orders.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using Xunit;
using Product = Answer.King.Domain.Orders.Models.Product;

namespace Answer.King.Infrastructure.UnitTests.Repositories.Factories;

[TestCategory(TestType.Unit)]
public class OrderFactoryTests
{
    [Fact]
    public void CreateOrder_ConstructorExists_ReturnsOrder()
    {
        // Arrange / Act
        var result = OrderFactory.CreateOrder(1, DateTime.UtcNow, DateTime.UtcNow, OrderStatus.Created, new List<LineItem>());

        // Assert
        Assert.IsType<Order>(result);
    }

    [Fact]
    public void CreateOrder_ConstructorNotFound_ReturnsException()
    {
        // Arrange
        var orderFactoryConstructorPropertyInfo =
        typeof(OrderFactory).GetProperty("OrderConstructor", BindingFlags.Static | BindingFlags.NonPublic);

        var constructor = orderFactoryConstructorPropertyInfo?.GetValue(null);

        var wrongConstructor = typeof(Domain.Inventory.Category).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        orderFactoryConstructorPropertyInfo?.SetValue(null, wrongConstructor);

        // Act // Assert
        Assert.Throws<TargetParameterCountException>(() =>
            OrderFactory.CreateOrder(1, DateTime.UtcNow, DateTime.UtcNow, OrderStatus.Created, new List<LineItem>()));

        //Reset static constructor to correct value
        orderFactoryConstructorPropertyInfo?.SetValue(null, constructor);
    }
}
