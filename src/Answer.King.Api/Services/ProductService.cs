using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Api.Services;

using System.Runtime.Serialization;

public class ProductService : IProductService
{
    public ProductService(
        IProductRepository products,
        ICategoryRepository categories)
    {
        this.Products = products;
        this.Categories = categories;
    }

    private IProductRepository Products { get; }

    private ICategoryRepository Categories { get; }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await this.Products.GetAll();
    }

    public async Task<IEnumerable<Product>> GetProducts(IEnumerable<long> productIds)
    {
        return await this.Products.GetMany(productIds);
    }

    public async Task<Product?> GetProduct(long productId)
    {
        return await this.Products.GetOne(productId);
    }

    public async Task<Product> CreateProduct(RequestModels.Product createProduct)
    {
        var category = await this.Categories.Get(createProduct.Category.Id);

        if (category == null)
        {
            throw new ProductServiceException("The provided category is not valid.");
        }

        var product = new Product(
            createProduct.Name,
            createProduct.Description,
            createProduct.Price,
            new Category(category.Id, category.Name, category.Description));
        await this.Products.AddOrUpdate(product);
        category.AddProduct(new ProductId(product.Id));

        await this.Categories.Save(category);

        return product;
    }

    public async Task<Product?> UpdateProduct(long productId, RequestModels.Product updateProduct)
    {
        var product = await this.Products.GetOne(productId);

        if (product == null)
        {
            return null;
        }

        if (product.Retired)
        {
            throw new ProductServiceException("The product is retired.");
        }

        var category = await this.Categories.Get(updateProduct.Category.Id);

        if (category == null)
        {
            throw new ProductServiceException("The provided category is not valid.");
        }

        product.Name = updateProduct.Name;
        product.Description = updateProduct.Description;
        product.Price = updateProduct.Price;

        if (product.Category.Id != updateProduct.Category.Id)
        {
            var currentCategory = await this.Categories.Get(product.Category.Id);

            if (currentCategory == null)
            {
                throw new ProductServiceException("The current category is not valid");
            }

            currentCategory.RemoveProduct(new ProductId(product.Id));
            product.SetCategory(new Category(category.Id, category.Name, category.Description));
        }

        await this.Products.AddOrUpdate(product);

        return product;
    }

    public async Task<Product?> RetireProduct(long productId)
    {
        var product = await this.Products.GetOne(productId);

        if (product == null)
        {
            return null;
        }

        if (product.Retired)
        {
            throw new ProductServiceException("The product is already retired.");
        }

        var categories = await this.Categories.GetByProductId(productId);
        foreach (var category in categories.ToList())
        {
            category.RemoveProduct(new ProductId(productId));
            await this.Categories.Save(category);
        }

        product.Retire();

        await this.Products.AddOrUpdate(product);

        return product;
    }
}

[Serializable]
public class ProductServiceException : Exception
{
    public ProductServiceException(string message)
        : base(message)
    {
    }

    public ProductServiceException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected ProductServiceException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext)
    {
    }
}
