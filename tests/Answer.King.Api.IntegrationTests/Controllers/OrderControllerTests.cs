using Alba;
using Answer.King.Api.IntegrationTests.Common;
using Answer.King.Api.IntegrationTests.Common.Models;
using Answer.King.Api.RequestModels;
using Order = Answer.King.Api.IntegrationTests.Common.Models.Order;

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

    #region Get
    [Fact]
    public async Task<VerifyResult> GetOrders_ReturnsList()
    {
        var result = await this._host.Scenario(_ =>
        {
            _.Get.Url("/api/orders");
            _.StatusCodeShouldBeOk();
        });

        var orders = result.ReadAsJson<IEnumerable<ReturnOrder>>();
        return await Verify(orders);
    }

    [Fact]
    public async Task<VerifyResult> GetOrder_OrderExists_ReturnsOrder()
    {
        var result = await this._host.Scenario(_ =>
        {
            _.Get.Url("/api/orders/1");
            _.StatusCodeShouldBeOk();
        });

        var orders = result.ReadAsJson<ReturnOrder>();
        return await Verify(orders);
    }

    [Fact]
    public async Task<VerifyResult> GetOrder_OrderDoesNotExist_Returns404()
    {
        var result = await this._host.Scenario(_ =>
        {
            _.Get.Url("/api/orders/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this._errorLevelSettings);
    }
    #endregion

    #region Post
    [Fact]
    public async Task<VerifyResult> PostOrder_ValidModel_ReturnsNewOrder()
    {
        var result = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 1 }
                })
                .ToUrl("/api/orders");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var orders = result.ReadAsJson<ReturnOrder>();
        return await Verify(orders);
    }

    [Fact]
    public async Task<VerifyResult> PostOrder_InValidDTO_Fails()
    {
        var result = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",Product
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 4 }
                })
                .ToUrl("/api/orders");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this._errorLevelSettings);
    }
    #endregion

    #region Put
    [Fact]
    public async Task<VerifyResult> PutOrder_ValidDTO_ReturnsModel()
    {
        var postResult = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 1 }
                })
                .ToUrl("/api/orders");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var orders = postresult.ReadAsJson<ReturnOrder>();

        var putResult = await this._host.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "BBQ Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 1 }
                })
                .ToUrl($"/api/orders/{orders?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        var updatedOrder = putresult.ReadAsJson<ReturnOrder>();
        return await Verify(updatedOrder);
    }

    [Fact]
    public async Task<VerifyResult> PutOrder_InvalidDTO_ReturnsBadRequest()
    {
        var putResult = await this._host.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "BBQ Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 4 }
                })
                .ToUrl("/api/orders/1");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._errorLevelSettings);
    }

    [Fact]
    public async Task<VerifyResult> PutOrder_InvalidId_ReturnsNotFound()
    {
        var putResult = await this._host.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "BBQ Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 1 }
                })
                .ToUrl("/api/orders/5");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._errorLevelSettings);
    }
    #endregion

    #region Retire
    [Fact]
    public async Task<VerifyResult> RetireOrder_InvalidId_ReturnsNotFound()
    {
        var putResult = await this._host.Scenario(_ =>
        {
            _.Delete
                .Url("/api/orders/5");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._errorLevelSettings);
    }

    [Fact]
    public async Task<VerifyResult> RetireOrder_ValidId_ReturnsOk()
    {
        var postResult = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 1 }
                })
                .ToUrl("/api/orders");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var orders = postresult.ReadAsJson<ReturnOrder>();

        var putResult = await this._host.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/orders/{orders?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._errorLevelSettings);
    }

    [Fact]
    public async Task<VerifyResult> RetireOrder_ValidId_IsRetired_ReturnsNotFound()
    {
        var postResult = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 1 }
                })
                .ToUrl("/api/orders");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var orders = postresult.ReadAsJson<ReturnOrder>();

        await this._host.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/orders/{orders?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        var secondDeleteResult = await this._host.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/orders/{orders?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Gone);
        });

        return await VerifyJson(secondDeleteResult.ReadAsTextAsync(), this._errorLevelSettings);
    }
    #endregion
}
