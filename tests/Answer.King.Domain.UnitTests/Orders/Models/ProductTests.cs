using Answer.King.Test.Common.CustomTraits;
using Answer.King.Domain.Orders.Models;
using Xunit;
using Product = Answer.King.Domain.Orders.Models.Product;

namespace Answer.King.Domain.UnitTests.Orders.Models;

[TestCategory(TestType.Unit)]
public class ProductTests
{
    [Fact]
    public void Product_InitWithDefaultGuid_ThrowsDefaultValueException()
    {
        // Arrange
        var id = default(Guid);
        var name = "name";
        var description = "description";
        var price = 142;
        var categories = new List<Category>
        {
            new Category(Guid.NewGuid(), "name", "description")
        };

        // Act / Assert

        Assert.Throws<Guard.DefaultValueException>(() => new Product(
            id,
            name,
            description,
            price,
            categories)
        );
    }

    [Fact]
    public void Product_InitWithNegativePrice_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "name";
        var description = "description";
        var price = -1;
        var categories = new List<Category>
        {
            new Category(Guid.NewGuid(), "name", "description")
        };

        // Act Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Product(
            id,
            name,
            description,
            price,
            categories)
        );
    }
}
