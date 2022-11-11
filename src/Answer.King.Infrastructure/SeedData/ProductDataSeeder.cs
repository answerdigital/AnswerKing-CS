using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Infrastructure.SeedData;

public class ProductDataSeeder : ISeedData
{
    public void SeedData(ILiteDbConnectionFactory connections)
    {
        var db = connections.GetConnection();
        var collection = db.GetCollection<Product>();

        if (DataSeeded)
        {
            return;
        }

        var none = collection.Count() < 1;
        if (none)
        {
            collection.Insert(ProductData.Products);
        }

        DataSeeded = true;
    }

    private static bool DataSeeded { get; set; }
}
