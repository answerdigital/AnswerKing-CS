using Alba;
using Answer.King.Api.IntegrationTests.Common;
using Xunit.Abstractions;
using Order = Answer.King.Api.IntegrationTests.Common.Models.Order;
using RMLineItems = Answer.King.Api.RequestModels.LineItem;

namespace Answer.King.Api.IntegrationTests.Controllers;

[UsesVerify]
public class OrderControllerUnseededTests : UnseededWebFixtures
{
    private readonly VerifySettings _verifySettings;

    public OrderControllerUnseededTests()
    {
        this._verifySettings = new();
        this._verifySettings.ScrubMembers("traceId");
    }

    #region Get
    [Fact]
    public async Task<VerifyResult> GetOrders_ReturnsEmptyList()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/orders");
            _.StatusCodeShouldBeOk();
        });

        var orders = result.ReadAsJson<IEnumerable<Order>>();
        return await Verify(orders, this._verifySettings);
    }
    #endregion
}


