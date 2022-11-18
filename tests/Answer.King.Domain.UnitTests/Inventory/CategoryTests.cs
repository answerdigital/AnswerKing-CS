using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Domain.UnitTests.Inventory;

[TestCategory(TestType.Unit)]
public class CategoryTests
{
    [Fact]
    public void RenameCategory_WithValidNameAndDescription_ReturnsExpectedResult()
    {
        var products = new List<ProductId> { new ProductId(1) };
        var category = new Category("Phones", "Electronics", products);

        category.Rename("Lemon", "Squash");

        Assert.Equal("Lemon", category.Name);
        Assert.Equal("Squash", category.Description);
    }

    [Fact]
    public void RenameCategory_WithInvalidName_ThrowsException()
    {
        var products = new List<ProductId> { new ProductId(1) };
        var category = new Category("Phones", "Electronics", products);
        Assert.Throws<ArgumentNullException>(() => category.Rename(null!, "Electronics"));
    }

    [Fact]
    public void RenameCategory_WithBlankName_ThrowsException()
    {
        var products = new List<ProductId> { new ProductId(1) };
        var category = new Category("Phones", "Electronics", products);
        Assert.Throws<Guard.EmptyStringException>(() => category.Rename("", "Electronics"));
    }

    [Fact]
    public void RenameCategory_WithInvalidDescription_ThrowsException()
    {
        var products = new List<ProductId> { new ProductId(1) };
        var category = new Category("Phones", "Electronics", products);
        Assert.Throws<ArgumentNullException>(() => category.Rename("Phones", null!));
    }

    [Fact]
    public void RenameCategory_WithBlankDescription_ThrowsException()
    {
        var products = new List<ProductId> { new ProductId(1) };
        var category = new Category("Phones", "Electronics", products);
        Assert.Throws<Guard.EmptyStringException>(() => category.Rename("Phones", ""));
    }

    [Fact]
    public void RetireCategory_WithProductsContainedWithinCategory_ThrowsException()
    {
        var products = new List<ProductId> { new ProductId(1) };
        var category = new Category("Phones", "Electronics", products);
        category.AddProduct(new ProductId(1));

        Assert.Throws<CategoryLifecycleException>(() => category.RetireCategory());
    }
}
