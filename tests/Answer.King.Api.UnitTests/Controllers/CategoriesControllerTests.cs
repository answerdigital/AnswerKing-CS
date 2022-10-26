using Answer.King.Api.Controllers;
using Answer.King.Api.Services;
using Answer.King.Domain.Inventory;
using Answer.King.Test.Common.CustomAsserts;
using Answer.King.Test.Common.CustomTraits;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Answer.King.Api.UnitTests.Controllers;

[TestCategory(TestType.Unit)]
public class CategoriesControllerTests
{
    #region GenericControllerTests

    [Fact]
    public void Controller_RouteAttribute_IsPresentWithCorrectRoute()
    {
        // Assert
        AssertController.HasRouteAttribute<CategoriesController>("api/[controller]");
        Assert.Equal(nameof(CategoriesController), "CategoriesController");
    }

    #endregion GenericControllerTests

    #region GetAll

    [Fact]
    public void GetAll_Method_HasCorrectVerbAttribute()
    {
        // Assert
        AssertController.MethodHasVerb<CategoriesController, HttpGetAttribute>(
            nameof(CategoriesController.GetAll));
    }

    [Fact]
    public async void GetAll_ValidRequest_ReturnsOkObjectResult()
    {
        // Arrange
        var data = new List<Category>();
        CategoryService.GetCategories().Returns(data);

        // Act
        var result = await GetSubjectUnderTest.GetAll();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    #endregion GetAll

    #region GetOne

    [Fact]
    public void GetOne_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<CategoriesController, HttpGetAttribute>(
            nameof(CategoriesController.GetOne), "{id}");
    }

    [Fact]
    public async Task GetOne_ValidRequestWithNullResult_ReturnsNotFoundResult()
    {
        // Arrange
        Category data = null!;
        CategoryService.GetCategory(Arg.Any<Guid>()).Returns(data);

        // Act
        var result = await GetSubjectUnderTest.GetOne(Arg.Any<Guid>());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact(Skip = "Solve access issue for private Category - WIP")]
    public void GetOne_ValidRequestWithResult_ReturnsOkObjectResult()
    {
        // Arrange
        //var data = new Category();
        //CategoryService.GetCategory(Arg.Any<Guid>()).Returns(data);

        // Act
        //var result = await GetSubjectUnderTest.GetOne((Arg.Any<Guid>()));

        // Assert
        //Assert.IsType<OkObjectResult>(result);
    }

    #endregion GetOne

    #region Post

    [Fact]
    public void Post_Method_HasCorrectVerbAttribute()
    {
        // Assert
        AssertController.MethodHasVerb<CategoriesController, HttpPostAttribute>(
            nameof(CategoriesController.Post));
    }

    [Fact(Skip = "This test needs to be written, but not sure due best way due to Domain.Inventory.Category protection level")]
    public void Post_ValidRequestCallsGetAction_ReturnsNewCategory()
    {
        // Arrange

        // Act

        // Assert
    }

    #endregion Post

    #region Put

    [Fact]
    public void Put_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<CategoriesController, HttpPutAttribute>(
            nameof(CategoriesController.Put), "{id}");
    }

    [Fact]
    public async void Put_NullCategory_ReturnsNotFoundResult()
    {
        // Arrange
        var id = Guid.Parse("20C72D1B-DE64-4262-B657-4280747CE358");

        // Act
        var result = await GetSubjectUnderTest.Put(id, null!);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async void Put_ValidRequest_ReturnsOkObjectResult()
    {
        // Arrange
        var id = Guid.Parse("E5853F4F-FCE7-4A81-B02D-729AF333328C");
        var categoryRequestModel = new RequestModels.CategoryDto
        {
            Name = "CATEGORY_NAME",
            Description = "CATEGORY_DESCRIPTION"
        };

        var category = new Category("CATEGORY_NAME", "CATEGORY_DESCRIPTION");

        CategoryService.UpdateCategory(id, categoryRequestModel).Returns(category);

        // Act
        var result = await GetSubjectUnderTest.Put(id, categoryRequestModel);

        // Assert
        Assert.Equal(categoryRequestModel.Name, category.Name);
        Assert.Equal(categoryRequestModel.Description, category.Description);
        Assert.IsType<OkObjectResult>(result);
    }

    #endregion Put

    #region Retire

    [Fact]
    public void Delete_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<CategoriesController, HttpDeleteAttribute>(
            nameof(CategoriesController.Retire), "{id}");
    }

    [Fact]
    public async void Retire_NullCategory_ReturnsNotFound()
    {
        // Arrange
        var category = null as RequestModels.CategoryDto;

        // Act
        var result = await GetSubjectUnderTest.Retire(Arg.Any<Guid>());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async void Retire_ValidRequest_ReturnsOkObjectResult()
    {
        // Arrange
        var id = Guid.Parse("E5853F4F-FCE7-4A81-B02D-729AF333328C");
        var category = new Category("CATEGORY_NAME", "CATEGORY_DESCRIPTION");

        CategoryService.RetireCategory(id).Returns(category);

        // Act
        var result = await GetSubjectUnderTest.Retire(id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    #endregion Retire

    #region GetProducts

    [Fact]
    public void GetProducts_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<CategoriesController, HttpGetAttribute>(
            nameof(CategoriesController.GetProducts), "{id}/products");
    }

    #endregion GetProducts

    #region Setup

    private static readonly ICategoryService CategoryService = Substitute.For<ICategoryService>();

    private static readonly IProductService ProductService = Substitute.For<IProductService>();

    private static readonly CategoriesController GetSubjectUnderTest =
        new CategoriesController(CategoryService, ProductService);

    #endregion Setup
}
