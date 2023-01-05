using System.Reflection;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Infrastructure.UnitTests.Repositories.Factories;

[TestCategory(TestType.Unit)]
public class ProductFactoryTests
{
    [Fact]
    public void CreateProduct_ConstructorExists_ReturnsProduct()
    {
        // Arrange / Act
        var result = ProductFactory.CreateProduct(1, "NAME", "DESC", 1, new List<CategoryId>(), new List<TagId>(), false);

        // Assert
        Assert.IsType<Product>(result);
    }

    [Fact]
    public void CreateProduct_ConstructorNotFound_ReturnsException()
    {
        // Arrange
        var productFactoryConstructorPropertyInfo =
        typeof(ProductFactory).GetProperty("ProductConstructor", BindingFlags.Static | BindingFlags.NonPublic);

        var constructor = productFactoryConstructorPropertyInfo?.GetValue(null);

        var wrongConstructor = typeof(Category).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        productFactoryConstructorPropertyInfo?.SetValue(null, wrongConstructor);

        // Act // Assert
        Assert.Throws<ArgumentException>(() =>
            ProductFactory.CreateProduct(1, "NAME", "DESC", 1, new List<CategoryId>(), new List<TagId>(), false));

        productFactoryConstructorPropertyInfo?.SetValue(null, constructor);
    }
}
