using System;
using System.Linq;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Orders.Models;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories.Mappings;

public class OrderEntityMappings : IEntityMapping
{
    public void RegisterMapping(BsonMapper mapper)
    {
        mapper.RegisterType
        (
            serialize: order =>
            {

                var lineItems = order.LineItems.Select(li => new BsonDocument
                {
                    ["Product"] = new BsonDocument
                    {
                        ["_id"] = li.Product.Id,
                        ["Name"] = li.Product.Name,
                        ["Description"] = li.Product.Description,
                        ["Categories"] = new BsonArray(li.Product.Categories.Select(ca => new BsonDocument
                        {
                            ["_id"] = ca.Id,
                            ["Name"] = ca.Name,
                            ["Description"] = ca.Description
                        })),
                        ["Price"] = li.Product.Price
                    },
                    ["Quantity"] = li.Quantity
                });

                var doc = new BsonDocument
                {
                    ["_id"] = order.Id,
                    ["CreatedOn"] = order.CreatedOn,
                    ["LastUpdated"] = order.LastUpdated,
                    ["OrderStatus"] = $"{order.OrderStatus}",
                    ["LineItems"] = new BsonArray(lineItems)
                };

                return doc;
            },
            deserialize: bson =>
            {
                var doc = bson.AsDocument;

                var lineItems = 
                    doc["LineItems"].AsArray.Select(this.ToLineItem)
                        .ToList();

                return OrderFactory.CreateOrder(
                    doc["_id"].AsGuid,
                    doc["CreatedOn"].AsDateTime,
                    doc["LastUpdated"].AsDateTime,
                    (OrderStatus)Enum.Parse(typeof(OrderStatus), doc["OrderStatus"]),
                    lineItems);
            }
        );
    }

    private LineItem ToLineItem(BsonValue item)
    {
        var lineItem = item.AsDocument;
        var product = lineItem["Product"].AsDocument;
        var category = product["Category"].AsDocument;

        var result = new LineItem(
            new Product(
                product["_id"].AsGuid,
                product["Name"].AsString,
                product["Description"].AsString,
                product["Price"].AsDouble,
                new Category(
                    category["_id"].AsGuid,
                    category["Name"].AsString,
                    category["Description"].AsString)
            )
        );

        result.AddQuantity(lineItem["Quantity"].AsInt32);

        return result;
    }
}
