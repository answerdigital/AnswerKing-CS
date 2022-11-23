using Answer.King.Domain;
using Answer.King.Domain.Orders.Models;

namespace Answer.King.Api.IntegrationTests.Common.Models;

public class Order
{
    public Order()
    {
        this.Id = 0;
        this.LastUpdated = this.CreatedOn = DateTime.UtcNow;
        this.OrderStatus = OrderStatus.Created;
        this._LineItems = new List<LineItem>();
    }

    // ReSharper disable once UnusedMember.Local
    private Order(
        long id,
        DateTime createdOn,
        DateTime lastUpdated,
        OrderStatus status,
        IList<LineItem>? lineItems)
    {
        Guard.AgainstDefaultValue(nameof(createdOn), createdOn);
        Guard.AgainstDefaultValue(nameof(lastUpdated), lastUpdated);

        this.Id = id;
        this.CreatedOn = createdOn;
        this.LastUpdated = lastUpdated;
        this.OrderStatus = status;
        this._LineItems = lineItems ?? new List<LineItem>();
    }

    public long Id { get; }

    public DateTime CreatedOn { get; }

    public DateTime LastUpdated { get; private set; }

    public OrderStatus OrderStatus { get; private set; }

    public double OrderTotal => this.LineItems.Sum(li => li.SubTotal);

    private IList<LineItem> _LineItems { get; }

    public IReadOnlyCollection<LineItem> LineItems => (this._LineItems as List<LineItem>)!;
}

public enum OrderStatus
{
    Created = 0,
    Complete = 1,
    Cancelled = 2
}
