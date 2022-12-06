using Alba;
using Answer.King.Api.IntegrationTests.Common;
using Xunit.Abstractions;
using Product = Answer.King.Api.IntegrationTests.Common.Models.Product;
using RMLineItems = Answer.King.Api.RequestModels.LineItem;

namespace Answer.King.Api.IntegrationTests.Controllers;

[UsesVerify]
public class ProductControllerUnseededTests : UnseededWebFixtures
{
    private readonly VerifySettings _verifySettings;

    public ProductControllerUnseededTests()
    {
        this._verifySettings = new();
        this._verifySettings.ScrubMembers("traceId");
        //this._verifySettings.UseStreamComparer("[]");
    }

    #region Get
    [Fact]
    public async Task<VerifyResult> GetProducts_ReturnsEmptyList()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/products");
            _.StatusCodeShouldBeOk();
        });

        var products = result.ReadAsJson<IEnumerable<Product>>();
        return await Verify(products, this._verifySettings);
    }
    #endregion
}
