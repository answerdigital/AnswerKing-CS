namespace Answer.King.Domain.Inventory.Models;

public class ProductId
{
    public ProductId(Guid id)
    {
        Guard.AgainstDefaultValue(nameof(id), id);

        this.Id = id;
    }

    public Guid Id { get; }
}
