using System;
using System.Collections.Generic;
using System.Linq;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Orders.Models;
using Answer.King.Infrastructure.Repositories.Mappings;

namespace Answer.King.Infrastructure.SeedData;

internal static class OrderData
{
    public static IList<Order> Orders { get; } = GetOrders();

    private static IList<Order> GetOrders()
    {
        return new List<Order>
        {
            OrderWithLineItems(),
            CancelledOrder(),
            new Order()
        };
    }

    private static Order OrderWithLineItems()
    {
        var fish = ProductData.Products
            .Where(p => p.Id == 1)
            .Select(x => new Product(x.Id, x.Name, x.Description, x.Price, new Category(x.Category.Id, x.Category.Name, x.Category.Description)))
            .SingleOrDefault();

        var lineItem1 = new LineItem(fish!);
        lineItem1.AddQuantity(1);

        var chips = ProductData.Products
            .Where(p => p.Id == 2)
            .Select(x => new Product(x.Id, x.Name, x.Description, x.Price, new Category(x.Category.Id, x.Category.Name, x.Category.Description)))
            .SingleOrDefault(); ;

        var lineItem2 = new LineItem(chips!);
        lineItem2.AddQuantity(2);

        var lineItems = new List<LineItem>
        {
            lineItem1,
            lineItem2
        };

        return OrderFactory.CreateOrder(
            1,
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddMinutes(-10),
            OrderStatus.Created,
            lineItems
        );
    }

    private static Order CancelledOrder()
    {
        var lineItems = new List<LineItem>();

        return OrderFactory.CreateOrder(
            2,
            DateTime.UtcNow.AddHours(-3),
            DateTime.UtcNow.AddMinutes(-50),
            OrderStatus.Cancelled,
            lineItems
        );
    }
}
