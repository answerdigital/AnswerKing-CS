using Answer.King.Domain;

namespace Answer.King.Api.IntegrationTests.Common.Models;

public class Product
{
    public Product(long id, string name, string description, double price, IList<long> categories, bool retired)
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.Categories = categories ?? new List<long>();
        this.Retired = retired;
    }
    public long Id { get; }

    public string Name { get; set; }

    public string Description { get; set; }

    public double Price { get; set; }

    public IList<long> Categories { get; set; }

    public bool Retired { get; private set; }

    public void Retire()
    {
        this.Retired = true;
    }
}
