using Answer.King.Domain.Orders;
using Answer.King.Domain.Orders.Models;
using Product = Answer.King.Domain.Repositories.Models.Product;

namespace Answer.King.Api.Services.Extensions;

public static class OrderExtensions
{
    public static void AddOrRemoveLineItems(this Order order, RequestModels.OrderDto orderChanges, IList<Product> domainProducts)
    {
        var actions = new List<AddRemoveAction>();
        actions.AddRange(GetLineItemsToAdd(order, orderChanges));
        actions.AddRange(GetLineItemsToRemove(order, orderChanges));
        actions.AddRange(GetLineItemQuantityActions(order, orderChanges));

        foreach (var action in actions)
        {
            if (action.IsIncrease)
            {
                var product = domainProducts.Single(p => p.Id == action.ProductId);
                var category = new Category(product.Category.Id, product.Category.Name, product.Category.Description);

                order.AddLineItem(
                    product.Id,
                    product.Name,
                    product.Description,
                    product.Price,
                    category,
                    action.QuantityDifference);
            }
            else
            {
                order.RemoveLineItem(action.ProductId, action.QuantityDifference);
            }
        }
    }

    private static IEnumerable<AddRemoveAction> GetLineItemsToRemove(Order order, RequestModels.OrderDto orderChanges)
    {
        var newProductIds = orderChanges.LineItems.Select(li => li.Product.Id);

        var lineItemsToRemove =
            order.LineItems
                .Where(x => !newProductIds.Contains(x.Product.Id))
                .ToList();

        var removeActions =
            lineItemsToRemove.Select(lineItem =>
                new AddRemoveAction
                {
                    ProductId = lineItem.Product.Id,
                    QuantityDifference = lineItem.Quantity,
                    IsIncrease = false
                });

        return removeActions;
    }

    private static IEnumerable<AddRemoveAction> GetLineItemsToAdd(Order order, RequestModels.OrderDto orderChanges)
    {
        var oldProductIds = order.LineItems.Select(li => li.Product.Id);

        var lineItemsToAdd =
            orderChanges.LineItems
                .Where(x => !oldProductIds.Contains(x.Product.Id) && x.Quantity > 0)
                .ToList();

        var addActions = 
            lineItemsToAdd.Select(lineItem => 
                new AddRemoveAction
                {
                    ProductId = lineItem.Product.Id,
                    QuantityDifference = lineItem.Quantity,
                    IsIncrease = true
                });

        return addActions;
    }

    private static IEnumerable<AddRemoveAction> GetLineItemQuantityActions(Order order, RequestModels.OrderDto orderChanges)
    {
        var quantityAddRemoveActions = order.LineItems.Select(lineItem =>
        {
            var updatedLineItem =
                orderChanges.LineItems
                    .First(li => li.Product.Id == lineItem.Product.Id);

            return new AddRemoveAction
            {
                ProductId = lineItem.Product.Id,
                QuantityDifference = Math.Abs(lineItem.Quantity - updatedLineItem.Quantity),
                IsIncrease = lineItem.Quantity < updatedLineItem.Quantity
            };
        });

        return quantityAddRemoveActions;
    }

    private class AddRemoveAction
    {
        public Guid ProductId { get; set; }

        public int QuantityDifference { get; set; }

        public bool IsIncrease { get; set; }
    }
}
