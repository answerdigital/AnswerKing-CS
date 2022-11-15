using System.Linq;
using Answer.King.Domain.Repositories.Models;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories.Mappings;

public class ProductEntityMappings : IEntityMapping
{
    public void RegisterMapping(BsonMapper mapper)
    {
        mapper.RegisterType
        (
            serialize: product =>
            {
                var doc = new BsonDocument
                {
                    ["_id"] = product.Id,
                    ["Name"] = product.Name,
                    ["Description"] = product.Description,
                    ["Price"] = product.Price,
                    ["Categories"] = new BsonArray(product.Categories.Select(ca => new BsonDocument
                    {
                        ["_id"] = ca.Id,
                        ["Name"] = ca.Name,
                        ["Description"] = ca.Description
                    })),
                    ["Retired"] = product.Retired
                };

                return doc;
            },
            deserialize: bson =>
            {
                var doc = bson.AsDocument;
                var cat = doc["Category"].AsDocument;
                var categories = new 
                Category(
                    cat["_id"].AsGuid,
                    cat["Name"].AsString,
                    cat["Description"].AsString);

                return ProductFactory.CreateProduct(
                    doc["_id"].AsGuid,
                    doc["Name"].AsString,
                    doc["Description"].AsString,
                    doc["Price"].AsDouble,
                    categories, 
                    doc["Retired"].AsBoolean);
            }
        );
    }
}
