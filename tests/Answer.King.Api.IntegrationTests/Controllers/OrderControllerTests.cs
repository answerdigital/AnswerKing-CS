using Alba;
using Answer.King.Api.IntegrationTests.Common;
using Answer.King.Api.RequestModels;
using Product = Answer.King.Api.IntegrationTests.Common.Models.Product;

namespace Answer.King.Api.IntegrationTests.Controllers;

[UsesVerify]
public class OrderControllerTests : IClassFixture<WebFixtures>
{
    private readonly IAlbaHost _host;

    private VerifySettings _errorLevelSettings;

    public OrderControllerTests(WebFixtures app)
    {
        this._host = app.AlbaHost;

        this._errorLevelSettings = new();
        this._errorLevelSettings.ScrubMember("traceId");
    }

    [Fact]
    public async Task<VerifyResult> GetOrders_ReturnsList()
    {
        var result = await this._host.Scenario(_ =>
        {
            _.Get.Url("/api/orders");
            _.StatusCodeShouldBeOk();
        });

        var orders = result.ReadAsJson<IEnumerable<Order>>();
        return await Verify(orders);
    }
}
