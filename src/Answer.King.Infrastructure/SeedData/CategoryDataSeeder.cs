using Answer.King.Domain.Inventory;

namespace Answer.King.Infrastructure.SeedData;

public class CategoryDataSeeder : ISeedData
{
    public void SeedData(ILiteDbConnectionFactory connections)
    {
        var db = connections.GetConnection();
        var collection = db.GetCollection<Category>();

        if (DataSeeded)
        {
            return;
        }

        var none = collection.Count() < 1;
        if (none)
        {
            collection.Insert(CategoryData.Categories);
        }

        DataSeeded = true;
    }

    private static bool DataSeeded { get; set; }
}
