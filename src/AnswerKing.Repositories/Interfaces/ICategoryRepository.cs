using System.Threading.Tasks;
using AnswerKing.Core.Entities;

namespace AnswerKing.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<CategoryEntity>
    {
        Task<int?> Create(CategoryEntity categoryEntity);
    }
}