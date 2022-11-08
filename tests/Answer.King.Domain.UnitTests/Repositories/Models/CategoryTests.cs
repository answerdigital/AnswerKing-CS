using Answer.King.Domain.Repositories.Models;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Domain.UnitTests.Repositories.Models;

[TestCategory(TestType.Unit)]
public class CategoryTests
{
    [Fact]
    public void Category_InitWithWithDefaultId_ThrowsDefaultValueException ()
    {
        // Arrange
        var id = 0;
        var name = "name";
        var description = "description";

        // Act / Assert
        Assert.Throws<Guard.DefaultValueException>(() => new Category(id, name, description));
    }

    [Fact]
    public void Category_InitWithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var id = 1;
        var name = null as string;
        var description = "description";

        // Act / Assert
        Assert.Throws<ArgumentNullException>(() => new Category(id, name!, description));
    }

    [Fact]
    public void Category_InitWithEmptyStringName_ThrowsEmptyStringException()
    {
        // Arrange
        var id = 1;
        var name = "";
        var description = "description";

        // Act / Assert
        Assert.Throws<Guard.EmptyStringException>(() => new Category(id, name, description));
    }
}
