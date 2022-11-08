using Answer.King.Domain.Repositories.Models;

using RequestProduct = Answer.King.Api.RequestModels.ProductDto;

namespace Answer.King.Api.Services;

public interface IProductService
{
    Task<Product> CreateProduct(RequestProduct createProduct);

    Task<Product?> GetProduct(long productId);

    Task<IEnumerable<Product>> GetProducts();

    Task<IEnumerable<Product>> GetProducts(IEnumerable<long> productIds);

    Task<Product?> RetireProduct(long productId);

    Task<Product?> UpdateProduct(long productId, RequestProduct updateProduct);
}