using Alba;
using Answer.King.Api.IntegrationTests.Common;
using Answer.King.Api.IntegrationTests.Common.Models;
using Answer.King.Api.RequestModels;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Answer.King.Api.IntegrationTests.Controllers;

[UsesVerify]
public class CategoryControllerTests : IClassFixture<WebFixtures>
{
    private readonly IAlbaHost _host;

    private VerifySettings _errorLevelSettings;

    public CategoryControllerTests(WebFixtures app)
    {
        this._host = app.AlbaHost;

        this._errorLevelSettings = new();
        this._errorLevelSettings.ScrubMember("traceId");
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
}
