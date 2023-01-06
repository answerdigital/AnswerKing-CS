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
public class TagFactoryTests
{
    private static TagFactory TagFactory = new();

    [Fact]
    public Task CreateTag_ConstructorExists_ReturnsTag()
    {
        // Arrange / Act
        var result = TagFactory.CreateTag(1, "NAME", "DESC", DateTime.UtcNow, DateTime.UtcNow, new List<ProductId>(), false);

        // Assert
        Assert.IsType<Tag>(result);
        return Verify(result);
    }

    [Fact]
    public void CreateTag_ConstructorNotFound_ReturnsException()
    {
        // Arrange
        var tagFactoryConstructorPropertyInfo =
        typeof(TagFactory).GetProperty("TagConstructor", BindingFlags.Instance | BindingFlags.NonPublic);

        var constructor = tagFactoryConstructorPropertyInfo?.GetValue(TagFactory);

        var wrongConstructor = typeof(Product).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        tagFactoryConstructorPropertyInfo?.SetValue(TagFactory, wrongConstructor);

        // Act // Assert
        Assert.Throws<ArgumentException>(() =>
            TagFactory.CreateTag(1, "NAME", "DESC", DateTime.UtcNow, DateTime.UtcNow, new List<ProductId>(), false));

        tagFactoryConstructorPropertyInfo?.SetValue(TagFactory, constructor);
    }
}
