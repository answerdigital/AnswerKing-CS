using System.Reflection;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using Category = Answer.King.Domain.Repositories.Models.Category;

namespace Answer.King.Infrastructure.UnitTests.Repositories.Factories;

[UsesVerify]
[TestCategory(TestType.Unit)]
public class ProductFactoryTests
{
    private static readonly ProductFactory ProductFactory = new();

    [Fact]
    public Task CreateProduct_ConstructorExists_ReturnsProduct()
    {
        // Arrange / Act
        var result = ProductFactory.CreateProduct(1, "NAME", "DESC", 1, new Category(1, "name", "desc"), new List<TagId>(), false);

        // Assert
        Assert.IsType<Product>(result);
        return Verify(result);
    }

    [Fact]
    public void CreateProduct_ConstructorNotFound_ReturnsException()
    {
        // Arrange
        var productFactoryConstructorPropertyInfo =
        typeof(ProductFactory).GetField("<ProductConstructor>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

        var constructor = productFactoryConstructorPropertyInfo?.GetValue(ProductFactory);

        var wrongConstructor = typeof(Order).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        productFactoryConstructorPropertyInfo?.SetValue(ProductFactory, wrongConstructor);

        // Act // Assert
        Assert.Throws<TargetParameterCountException>(() =>
            ProductFactory.CreateProduct(1, "NAME", "DESC", 1, new Category(1, "name", "desc"), new List<TagId>(), false));

        productFactoryConstructorPropertyInfo?.SetValue(ProductFactory, constructor);
    }
}
