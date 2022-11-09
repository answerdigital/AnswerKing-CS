using System;
using System.Reflection;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories.Mappings;

public interface IEntityMapping
{
    void RegisterMapping (BsonMapper mapper);

    void ResolveMember(Type type, MemberInfo memberInfo, MemberMapper memberMapper);
}
