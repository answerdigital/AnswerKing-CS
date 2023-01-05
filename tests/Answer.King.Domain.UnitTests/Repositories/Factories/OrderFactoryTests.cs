using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Orders.Models;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Product = Answer.King.Domain.Orders.Models.Product;

namespace Answer.King.Domain.UnitTests.Repositories.Factories;

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
        var OrderFactoryConstructorFieldInfo =
        typeof(OrderFactory).GetField($"<OrderConstructor>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic);

        var constructor = OrderFactoryConstructorFieldInfo?.GetValue(null);

        var wrongConstructor = typeof(Domain.Inventory.Category).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        OrderFactoryConstructorFieldInfo?.SetValue(null, wrongConstructor);

        // Act // Assert
        Assert.Throws<TargetParameterCountException>(() =>
            OrderFactory.CreateOrder(1, DateTime.UtcNow, DateTime.UtcNow, OrderStatus.Created, new List<LineItem>()));

        //Reset static constructor to correct value
        OrderFactoryConstructorFieldInfo?.SetValue(null, constructor);
    }


}
