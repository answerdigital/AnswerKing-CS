using Answer.King.Domain.Inventory;

namespace Answer.King.Infrastructure.SeedData;

public class TagDataSeeder : ISeedData
{
    public void SeedData(ILiteDbConnectionFactory connections)
    {
        var db = connections.GetConnection();
        var collection = db.GetCollection<Tag>();

        if (this.DataSeeded)
        {
            return;
        }

        var none = collection.Count() < 1;
        if (none)
        {
            collection.InsertBulk(TagData.Tags);
        }

        this.DataSeeded = true;
    }

    private bool DataSeeded { get; set; }
}
