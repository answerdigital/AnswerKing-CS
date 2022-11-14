using System.Text;
using Answer.King.Api.RequestModels;
using Answer.King.Api.IntegrationTests.Common.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using Assert = Xunit.Assert;

namespace Answer.King.Api.IntegrationTests.Controllers;
[TestClass]
public class ProductControllerTests 
{
    private readonly HttpClient _httpClient;

    public ProductControllerTests()
    {
        var webApplicationFactory = new WebApplicationFactory<Program>();
        this._httpClient = webApplicationFactory.CreateDefaultClient();
    }

    #region Get
    [Fact]
    public async Task GetProducts_ReturnsList()
    {

        var response = await this._httpClient.GetAsync("/api/products");
        var result = await response.Content.ReadAsStringAsync();
        var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(result);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(products);
    }

    [Fact]
    public async Task GetProduct_ProductExists_ReturnsProduct()
    {
        var response = await this._httpClient.GetAsync("/api/products/1");
        var result = await response.Content.ReadAsStringAsync();
        var products = JsonConvert.DeserializeObject<Product>(result);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProduct_ProductDoesNotExist_Returns404()
    {
        var response = await this._httpClient.GetAsync("/api/products/5000");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion

    #region Post
    [Fact]
    public async Task PostProduct_ValidModel_ReturnsNewProduct()
    {
        var body = new
        {
            Name = "Burger",
            Description = "Juicy",
            Price = 1.50,
            Category = new CategoryId { Id = 1 }
        };

        var response = await this._httpClient.PostAsync(
            "/api/products",
            new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_InValidMedia_Fails()
    {
        var body = new
        {
            Actor = "Burger",
            Description = "Juicy",
            Price = 1.50,
            Category = new CategoryId { Id = 1 }
        };

        var response = await this._httpClient.PostAsync(
            "/api/products",
            new StringContent(JsonConvert.SerializeObject(body)));

        Assert.Equal(System.Net.HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_InValidDTO_Fails()
    {
        var body = new
        {
            Name = "Burger",
            Description = "Juicy",
            Price = 1.50
        };

        var response = await this._httpClient.PostAsync(
            "/api/products",
            new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
    #endregion

    #region Put
    [Fact]
    public async Task PutProduct_ValidDTO_ReturnsModel()
    {
        var body = new
        {
            Name = "Test",
            Description = "Juicy",
            Price = 1.50,
            Category = new CategoryId { Id = 1 }
        };

        var postResponse = await this._httpClient.PostAsync(
            "/api/products",
            new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

        var result = await postResponse.Content.ReadAsStringAsync();
        var product = JsonConvert.DeserializeObject<Product>(result);

        var putBody = new
        {
            Name = "BBQ Burger",
            Description = "Juicy",
            Price = 1.50,
            Category = new CategoryId { Id = 1 }
        };

        var response = await this._httpClient.PutAsync(
            $"/api/products/{product.Id}",
            new StringContent(JsonConvert.SerializeObject(putBody), Encoding.UTF8, "application/json"));

        var putResult = await response.Content.ReadAsStringAsync();
        var putProduct = JsonConvert.DeserializeObject<Product>(putResult);

        Assert.Equal(putBody.Name, putProduct.Name);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PutProduct_InvalidDTO_ReturnsModel()
    {
        var body = new
        {
            Name = "BBQ Burger",
            Description = "Juicy",
            Price = 1.50,
            Category = new CategoryId { Id = 300000 }
        };

        var response = await this._httpClient.PutAsync(
            "/api/products/1",
            new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutProduct_InvalidId_ReturnsNotFound()
    {
        var body = new
        {
            Name = "BBQ Burger",
            Description = "Juicy",
            Price = 1.50,
            Category = new CategoryId { Id = 1 }
        };

        var response = await this._httpClient.PutAsync(
            "/api/products/100000",
            new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion

    #region Retire
    [Fact]
    public async Task RetireProduct_InvalidId_ReturnsNotFound()
    {
        var response = await this._httpClient.DeleteAsync("/api/products/100000");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RetireProduct_ValidId_ReturnsOk()
    {
        var body = new
        {
            Name = "Test",
            Description = "Juicy",
            Price = 1.50,
            Category = new CategoryId { Id = 1 }
        };

        var postResponse = await this._httpClient.PostAsync(
            "/api/products",
            new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

        var result = await postResponse.Content.ReadAsStringAsync();
        var product = JsonConvert.DeserializeObject<Product>(result);

        var response = await this._httpClient.DeleteAsync($"/api/products/{product.Id}");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task RetireProduct_ValidId_IsRetired_ReturnsNotFound()
    {
        var response = await this._httpClient.DeleteAsync("/api/products/3");

        Assert.Equal(System.Net.HttpStatusCode.Gone, response.StatusCode);
    }
    #endregion
}
