using LiteDB;

namespace Answer.King.Infrastructure.Repositories.Mappings;

public interface IEntityMapping
{
    void RegisterMapping(BsonMapper mapper);
}
