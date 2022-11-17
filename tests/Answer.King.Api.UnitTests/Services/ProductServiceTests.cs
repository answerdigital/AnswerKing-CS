using Answer.King.Api.RequestModels;
using Answer.King.Api.Services;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using NSubstitute;
using Xunit;
using Category = Answer.King.Domain.Inventory.Category;

namespace Answer.King.Api.UnitTests.Services;

[TestCategory(TestType.Unit)]
public class ProductServiceTests
{
    #region Create

    [Fact]
    public async void CreateProduct_InvalidCategoryIdInProduct_ThrowsException()
    {
        // Arrange
        var productRequest = new ProductDto
        {
            Name = "Laptop",
            Description = "desc",
            Price = 1500.00,
            Categories = new List<CategoryId>
            {
                new CategoryId
                {
                    Id = 1
                }
            }
        };

        this.CategoryRepository.Get(Arg.Any<long>()).Returns(null as Category);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<ProductServiceException>(() => sut.CreateProduct(productRequest));
    }

    #endregion

    #region Retire

    [Fact]
    public async void RetireProduct_InvalidProductId_ReturnsNull()
    {
        // Arrange
        this.ProductRepository.Get(Arg.Any<long>()).Returns(null as Domain.Repositories.Models.Product);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        Assert.Null(await sut.RetireProduct(1));
    }

    [Fact]
    public async void RetireProduct_ValidProductId_ReturnsProductAsRetired()
    {
        // Arrange
        var product = ProductFactory.CreateProduct(1,
            "product", "desc", 12.00, DateTime.Now, DateTime.Now, new List<Domain.Repositories.Models.Category>
            {
                new Domain.Repositories.Models.Category(1, "category", "desc")
            }, false);

        this.ProductRepository.Get(product.Id).Returns(product);

        var category = new Category("category", "desc",
            new List<Domain.Inventory.Models.ProductId>
            {
                new Domain.Inventory.Models.ProductId(1)
            });
        var categories = new Category[] { category };

        this.CategoryRepository.GetByProductId(product.Id)
            .Returns(categories);

        // Act
        var sut = this.GetServiceUnderTest();
        var retiredProduct = await sut.RetireProduct(product.Id);

        // Assert
        Assert.True(retiredProduct!.Retired);
        Assert.Equal(product.Id, retiredProduct.Id);

        await this.CategoryRepository.Received().GetByProductId(product.Id);
        await this.CategoryRepository.Save(category);
        await this.ProductRepository.AddOrUpdate(product);
    }

    #endregion

    #region Update

    [Fact]
    public async void UpdateProduct_InvalidProductId_ReturnsNull()
    {
        // Arrange
        this.ProductRepository.Get(Arg.Any<long>()).Returns(null as Domain.Repositories.Models.Product);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        Assert.Null(await sut.UpdateProduct(1, new ProductDto()));
    }

    [Fact]
    public async void UpdateProduct_InvalidProductNotAssociatedWithCategory_ThrowsException()
    {
        // Arrange
        var categories = new List<Domain.Repositories.Models.Category>
        {
            new Domain.Repositories.Models.Category(1, "category", "desc")
        };
        var product = new Domain.Repositories.Models.Product("product", "desc", 10.00, categories);

        this.ProductRepository.Get(Arg.Any<long>()).Returns(product);
        //this.CategoryRepository.GetByProductId(product.Id).Returns(null as Category[]);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<ProductServiceException>(() =>
            sut.UpdateProduct(product.Id, new ProductDto()));
    }

    [Fact]
    public async void UpdateProduct_InvalidUpdatedCategory_ThrowsException()
    {
        // Arrange
        var oldCategory = this.CreateCategory(1, "category", "desc");
        var categories = new Category[]
        {
            oldCategory
        };
        var product = new Product(
            "product",
            "desc",
            10.00,
            new List<Domain.Repositories.Models.Category>
            {
                new Domain.Repositories.Models.Category(oldCategory.Id, oldCategory.Name, oldCategory.Description)
            });

        var updatedCategory = this.CreateCategory(2, "updated category", "desc");

        this.ProductRepository.Get(Arg.Any<long>()).Returns(product);
        this.CategoryRepository.GetByProductId(product.Id).Returns(categories);
        this.CategoryRepository.Get(updatedCategory.Id).Returns(null as Category);

        var updatedProduct = new ProductDto
        {
            Categories = new List<CategoryId>
            {
                new CategoryId
                {
                    Id = updatedCategory.Id
                }
            }
        };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<ProductServiceException>(() =>
            sut.UpdateProduct(product.Id, updatedProduct));
    }

    #endregion

    #region Get

    [Fact]
    public async void GetProducts_ReturnsAllProducts()
    {
        // Arrange
        var categories = new List<Domain.Repositories.Models.Category>
        {
            new Domain.Repositories.Models.Category(1, "category", "desc")
        };
        var products = new[]
        {
            new Domain.Repositories.Models.Product("product 1", "desc", 10.00, categories),
            new Domain.Repositories.Models.Product("product 2", "desc", 5.00, categories)
        };

        this.ProductRepository.Get().Returns(products);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualProducts = await sut.GetProducts();

        // Assert
        Assert.Equal(products, actualProducts);
        await this.ProductRepository.Received().Get();
    }

    [Fact]
    public async void GetProduct_ValidProductId_ReturnsProduct()
    {
        // Arrange
        var categories = new List<Domain.Repositories.Models.Category>
        {
            new Domain.Repositories.Models.Category(1, "category", "desc")
        };
        var product = new Domain.Repositories.Models.Product("product 1", "desc", 10.00, categories);

        this.ProductRepository.Get(product.Id).Returns(product);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualProduct = await sut.GetProduct(product.Id);

        // Assert
        Assert.Equal(product, actualProduct);
        await this.ProductRepository.Received().Get(product.Id);
    }

    #endregion

    #region Helpers

    public Category CreateCategory(long id, string name, string description)
    {
        return CategoryFactory.CreateCategory(id, name, description, DateTime.UtcNow, DateTime.UtcNow, new List<Answer.King.Domain.Inventory.Models.ProductId>(), false);
    }

    #endregion

    #region Setup

    private readonly ICategoryRepository CategoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IProductRepository ProductRepository = Substitute.For<IProductRepository>();

    private IProductService GetServiceUnderTest()
    {
        return new ProductService(this.ProductRepository, this.CategoryRepository);
    }

    #endregion
}
