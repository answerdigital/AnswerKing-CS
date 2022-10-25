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
                    ["Category"] = new BsonDocument
                    {
                        ["_id"] = product.Category.Id,
                        ["Name"] = product.Category.Name,
                        ["Description"] = product.Category.Description
                    },
                    ["Retired"] = product.Retired
                };

                return doc;
            },
            deserialize: bson =>
            {
                var doc = bson.AsDocument;
                var cat = doc["Category"].AsDocument;
                var category = new Category(
                    cat["_id"].AsGuid,
                    cat["Name"].AsString,
                    cat["Description"].AsString);

                return ProductFactory.CreateProduct(
                    doc["_id"].AsGuid,
                    doc["Name"].AsString,
                    doc["Description"].AsString,
                    doc["Price"].AsDouble,
                    category, 
                    doc["Retired"].AsBoolean);
            }
        );
    }
}
