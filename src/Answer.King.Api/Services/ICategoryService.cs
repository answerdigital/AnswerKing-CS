using Answer.King.Domain.Inventory;
using RequestCategory = Answer.King.Api.RequestModels.CategoryDto;

namespace Answer.King.Api.Services;

public interface ICategoryService
{
    Task<Category> CreateCategory(RequestCategory createCategory);
    Task<IEnumerable<Category>> GetCategories();
    Task<Category?> GetCategory(long categoryId);
    Task<Category?> RetireCategory(long categoryId);
    Task<Category?> UpdateCategory(long categoryId, RequestCategory updateCategory);
}
