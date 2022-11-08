namespace Answer.King.Domain.Inventory.Models;

public class ProductId
{
    public ProductId(long id)
    {
        Guard.AgainstDefaultValue(nameof(id), id);

        this.Id = id;
    }

    public long Id { get; }
}
