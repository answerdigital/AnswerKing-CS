using Answer.King.Domain.Inventory;
using RequestCategory = Answer.King.Api.RequestModels.CategoryDto;

namespace Answer.King.Api.Services;

public interface ICategoryService
{
    Task<Category> CreateCategory(RequestCategory createCategory);
    Task<IEnumerable<Category>> GetCategories();
    Task<Category?> GetCategory(Guid categoryId);
    Task<Category?> RetireCategory(Guid categoryId);
    Task<Category?> UpdateCategory(Guid categoryId, RequestCategory updateCategory);
}
