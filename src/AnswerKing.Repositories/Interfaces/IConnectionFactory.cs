using System.Data;

namespace AnswerKing.Repositories.Interfaces
{
    public interface IConnectionFactory
    {
        public IDbConnection GetConnection();
    }
}