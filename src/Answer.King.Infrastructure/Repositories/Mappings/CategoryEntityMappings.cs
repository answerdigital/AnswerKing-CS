using System;
using System.Linq;
using System.Reflection;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories.Mappings;

public class CategoryEntityMappings : IEntityMapping
{
    private static readonly FieldInfo? CategoryIdFieldInfo =
        typeof(Category).GetField($"<{nameof(Category.Id)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

    public void RegisterMapping(BsonMapper mapper)
    {
        mapper.RegisterType
        (
            serialize: category =>
            {
                var products = category.Products.Select(p => new BsonDocument { ["_id"] = p.Id });

                var doc = new BsonDocument
                {
                    ["_id"] = category.Id,
                    ["Name"] = category.Name,
                    ["Description"] = category.Description,
                    ["CreatedOn"] = category.CreatedOn,
                    ["LastUpdated"] = category.LastUpdated,
                    ["Products"] = new BsonArray(products),
                    ["Retired"] = category.Retired
                };

                return doc;
            },
            deserialize: bson =>
            {
                var doc = bson.AsDocument;

                return CategoryFactory.CreateCategory(
                    doc["_id"].AsInt64,
                    doc["Name"].AsString,
                    doc["Description"].AsString,
                    doc["CreatedOn"].AsDateTime,
                    doc["LastUpdated"].AsDateTime,
                    doc["Products"].AsArray.Select(
                        p => new ProductId(p.AsDocument["_id"].AsInt64)).ToList(),
                    doc["Retired"].AsBoolean);
            }
        );
    }

    public void ResolveMember(Type type, MemberInfo memberInfo, MemberMapper memberMapper)
    {
        if (type == typeof(Category) && memberMapper.MemberName == "Id")
        {
            memberMapper.Setter =
                (obj, value) => CategoryIdFieldInfo?.SetValue(obj, value);
        }
    }
}
