using Answer.King.Api.Services.Extensions;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Repositories;


namespace Answer.King.Api.Services;

public class OrderService : IOrderService
{
    public OrderService(
        IOrderRepository orders,
        IProductRepository products)
    {
        this.Orders = orders;
        this.Products = products;
    }

    private IOrderRepository Orders { get; }

    private IProductRepository Products { get; }

    public async Task<Order?> GetOrder(Guid orderId)
    {
        return await this.Orders.Get(orderId);
    }

    public async Task<IEnumerable<Order>> GetOrders()
    {
        return await this.Orders.Get();
    }

    public async Task<Order> CreateOrder(RequestModels.OrderDto createOrder)
    {
        var submittedProductIds = createOrder.LineItems.Select(l => l.Product.Id).ToList();

        var matchingProducts =
            (await this.Products.Get(submittedProductIds)).ToList();

        var invalidProducts =
            submittedProductIds.Except(matchingProducts.Select(p => p.Id))
                .ToList();

        if (invalidProducts.Any())
        {
            throw new ProductInvalidException(
                $"Product id{(invalidProducts.Count > 1 ? "s" : "")} does not exist: {string.Join(',', invalidProducts)}");
        }

        var order = new Order();
        order.AddOrRemoveLineItems(createOrder, matchingProducts);

        await this.Orders.Save(order);

        return order;
    }

    public async Task<Order?> UpdateOrder(Guid orderId, RequestModels.OrderDto updateOrder)
    {
        var order = await this.Orders.Get(orderId);

        if (order == null)
        {
            return null;
        }

        var submittedProductIds = updateOrder.LineItems.Select(l => l.Product.Id).ToList();

        var matchingProducts =
            (await this.Products.Get(submittedProductIds)).ToList();

        var invalidProducts =
            submittedProductIds.Except(matchingProducts.Select(p => p.Id))
                .ToList();

        if (invalidProducts.Any())
        {
            throw new ProductInvalidException(
                $"Product id{(invalidProducts.Count > 1 ? "s" : "")} does not exist: {string.Join(',', invalidProducts)}");
        }

        order.AddOrRemoveLineItems(updateOrder, matchingProducts);

        await this.Orders.Save(order);

        return order;
    }

    public async Task<Order?> CancelOrder(Guid orderId)
    {
        var order = await this.Orders.Get(orderId);

        if (order == null)
        {
            return null;
        }

        try
        {
            order.CancelOrder();
            await this.Orders.Save(order);
        }
#pragma warning disable RCS1075
        catch (Exception)
        {
            // ignored
        }
#pragma warning restore RCS1075
        return order;
    }
}

[Serializable]
internal class ProductInvalidException : Exception
{
    public ProductInvalidException(string message) : base(message)
    {
    }

    public ProductInvalidException () : base()
    {
    }

    public ProductInvalidException (string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
