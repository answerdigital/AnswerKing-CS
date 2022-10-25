using Answer.King.Domain.Orders;

namespace Answer.King.Infrastructure.SeedData;

public class OrderDataSeeder : ISeedData
{
    public void SeedData(ILiteDbConnectionFactory connections)
    {
        var db = connections.GetConnection();
        var collection = db.GetCollection<Order>();

        if (DataSeeded)
        {
            return;
        }

        var none = collection.Count() < 1;
        if (none)
        {
            collection.InsertBulk(OrderData.Orders);
        }

        DataSeeded = true;
    }

    private static bool DataSeeded { get; set; }
}
