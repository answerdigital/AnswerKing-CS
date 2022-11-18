using Alba;
using Answer.King.Api.IntegrationTests.Common;
using Answer.King.Api.IntegrationTests.Common.Models;
using Answer.King.Api.RequestModels;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Answer.King.Api.IntegrationTests.Controllers;


[UsesVerify]
public class CategoryControllerTests : IAsyncLifetime
{
    private IAlbaHost _host = null!;

    private VerifySettings _errorLevelSettings;

    public CategoryControllerTests()
    {
        this._errorLevelSettings = new();
        this._errorLevelSettings.ScrubMember("traceId");
    }

    public async Task InitializeAsync()
    {
        this._host = await Alba.AlbaHost.For<Program>();
    }

    public async Task DisposeAsync()
    {
        await this._host.DisposeAsync();
        File.Delete(".\\Answer.King.db");
    }

    #region Get
    [Fact]
    public async Task<VerifyResult> GetCategories_ReturnsList()
    {
        var result = await this._host.Scenario(_ =>
        {
            _.Get.Url("/api/categories");
            _.StatusCodeShouldBeOk();
        });

        var products = result.ReadAsJson<IEnumerable<Category>>();
        return await Verify(products);
    }

    [Fact]
    public async Task<VerifyResult> GetCategory_CategoryExists_ReturnsCategory()
    {
        var category = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans"
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var result = await this._host.Scenario(_ =>
        {
            _.Get.Url("/api/categories/1");
            _.StatusCodeShouldBeOk();
        });

        var products = result.ReadAsJson<Category>();
        return await Verify(products);
    }

    [Fact]
    public async Task<VerifyResult> GetCategory_CategoryDoesNotExist_Returns404()
    {
        var result = await this._host.Scenario(_ =>
        {
            _.Get.Url("/api/categories/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this._errorLevelSettings);
    }
    #endregion

    #region Post
    [Fact]
    public async Task<VerifyResult> PostCategory_ValidModel_ReturnsNewCategory()
    {
        var result = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans"
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var products = result.ReadAsJson<Category>();
        return await Verify(products);
    }

    [Fact]
    public async Task<VerifyResult> PostCategory_InvalidDTO_Fails()
    {
        var result = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood"
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this._errorLevelSettings);
    }
    #endregion

    #region Put
    [Fact]
    public async Task<VerifyResult> PutCategory_ValidDTO_ReturnsModel()
    {
        var postResult = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans"
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var category = postResult.ReadAsJson<Category>();

        var putResult = await this._host.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans and the high seas and also the puddles maybe"
                })
                .ToUrl($"/api/categories/{category?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        var updatedCategory = putResult.ReadAsJson<Category>();
        return await Verify(updatedCategory);
    }

    [Fact]
    public async Task<VerifyResult> PutCategory_InvalidDTO_ReturnsBadRequest()
    {
        var putResult = await this._host.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "Seafood"
                })
                .ToUrl("/api/categories/1");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._errorLevelSettings);
    }

    [Fact]
    public async Task<VerifyResult> PutCategory_InvalidId_ReturnsNotFound()
    {
        var putResult = await this._host.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans"
                })
                .ToUrl("/api/categories/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._errorLevelSettings);
    }
    #endregion

    #region Retire
    [Fact]
    public async Task<VerifyResult> RetireCategory_InvalidId_ReturnsNotFound()
    {
        var putResult = await this._host.Scenario(_ =>
        {
            _.Delete
                .Url("/api/categories/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._errorLevelSettings);
    }

    [Fact]
    public async Task<VerifyResult> RetireCategory_ValidId_ReturnsOk()
    {
        var postResult = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans"
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var categories = postResult.ReadAsJson<Category>();

        var putResult = await this._host.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/categories/{categories?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._errorLevelSettings);
    }

    [Fact]
    public async Task<VerifyResult> RetireCategory_ValidId_IsRetired_ReturnsNotFound()
    {
        var postResult = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans"
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var categories = postResult.ReadAsJson<Category>();

        await this._host.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/categories/{categories?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        var secondDeleteResult = await this._host.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/categories/{categories?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Gone);
        });

        return await VerifyJson(secondDeleteResult.ReadAsTextAsync(), this._errorLevelSettings);
    }
    #endregion
}
