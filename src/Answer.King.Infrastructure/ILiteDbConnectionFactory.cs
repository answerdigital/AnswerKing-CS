using LiteDB;

namespace Answer.King.Infrastructure;

public interface ILiteDbConnectionFactory
{
    LiteDatabase GetConnection();
}
