using System.Reflection;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Infrastructure.UnitTests.Repositories.Factories;

[UsesVerify]
[TestCategory(TestType.Unit)]
public class CategoryFactoryTests
{
    private static CategoryFactory CategoryFactory { get; } = new();

    [Fact]
    public Task CreateCategory_ConstructorExists_ReturnsCategory()
    {
        // Arrange / Act
        var result = CategoryFactory.CreateCategory(1, "NAME", "DESC", DateTime.UtcNow, DateTime.UtcNow, new List<ProductId>(), false);

        // Assert
        Assert.IsType<Category>(result);
        return Verify(result);
    }

    [Fact]
    public void CreateCategory_ConstructorNotFound_ReturnsException()
    {
        // Arrange
        var categoryFactoryConstructorPropertyInfo =
        typeof(CategoryFactory).GetProperty("CategoryConstructor", BindingFlags.Instance | BindingFlags.NonPublic);

        var constructor = categoryFactoryConstructorPropertyInfo?.GetValue(CategoryFactory);

        var wrongConstructor = typeof(Product).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        categoryFactoryConstructorPropertyInfo?.SetValue(CategoryFactory, wrongConstructor);

        // Act // Assert
        Assert.Throws<ArgumentException>(() =>
            CategoryFactory.CreateCategory(1, "NAME", "DESC", DateTime.UtcNow, DateTime.UtcNow, new List<ProductId>(), false));

        //Reset static constructor to correct value
        categoryFactoryConstructorPropertyInfo?.SetValue(CategoryFactory, constructor);
    }
}
