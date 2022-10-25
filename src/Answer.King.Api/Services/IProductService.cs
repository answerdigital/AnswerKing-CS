using Answer.King.Domain.Repositories.Models;

using RequestProduct = Answer.King.Api.RequestModels.ProductDto;

namespace Answer.King.Api.Services;

public interface IProductService
{
    Task<Product> CreateProduct(RequestProduct createProduct);

    Task<Product?> GetProduct(Guid productId);

    Task<IEnumerable<Product>> GetProducts();

    Task<IEnumerable<Product>> GetProducts(IEnumerable<Guid> productIds);

    Task<Product?> RetireProduct(Guid productId);

    Task<Product?> UpdateProduct(Guid productId, RequestProduct updateProduct);
}