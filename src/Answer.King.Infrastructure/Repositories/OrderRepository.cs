using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Repositories;
using Answer.King.Infrastructure.SeedData;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    public OrderRepository(ILiteDbConnectionFactory connections)
    {
        var db = connections.GetConnection();

        this.Collection = db.GetCollection<Order>();
        this.Collection.EnsureIndex("LineItems.Product.Id");

        this.SeedData();
    }

    private ILiteCollection<Order> Collection { get; }

    public Task<IEnumerable<Order>> Get()
    {
        return Task.FromResult(this.Collection.FindAll());
    }

    public Task<Order?> Get(Guid id)
    {
        return Task.FromResult(this.Collection.FindOne(c => c.Id == id))!;
    }

    public Task Save(Order item)
    {
        return Task.FromResult(this.Collection.Upsert(item));
    }

    private void SeedData()
    {
        if (DataSeeded)
        {
            return;
        }

        var none = this.Collection.Count() < 1;
        if (none)
        {
            this.Collection.InsertBulk(OrderData.Orders);
        }

        DataSeeded = true;
    }

    private static bool DataSeeded { get; set; }
}
