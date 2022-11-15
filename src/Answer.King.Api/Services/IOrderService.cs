using Answer.King.Domain.Orders;

using RequestOrder = Answer.King.Api.RequestModels.OrderDto;

namespace Answer.King.Api.Services;

public interface IOrderService
{
    Task<Order> CreateOrder(RequestOrder createOrder);

    Task<Order?> GetOrder(long orderId);

    Task<IEnumerable<Order>> GetOrders();

    Task<Order?> UpdateOrder(long orderId, RequestOrder updateOrder);

    Task<Order?> CancelOrder(long orderId);
}