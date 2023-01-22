using Answer.King.Api.Common.HealthChecks;
using Answer.King.Infrastructure;
using Answer.King.Test.Common.CustomTraits;
using LiteDB;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Answer.King.Api.UnitTests.Common.HealthChecks;

[TestCategory(TestType.Unit)]
public class DatabaseHealthCheckTests
{
    private readonly string testDbName = $"Answer.King.{Guid.NewGuid()}.db";
    private readonly ILiteDbConnectionFactory dbConnectionFactory = Substitute.For<ILiteDbConnectionFactory>();

    [Fact]
    public async void CheckHealthAsync_DelayUnderDegradedThreshold_ReturnsHealthCheckResultHealthy()
    {
        // Arrange
        var liteDb = new LiteDatabase($"filename={this.testDbName};Connection=Shared;", new BsonMapper());
        var options = Options.Create(new HealthCheckOptions());

        this.dbConnectionFactory.GetConnection().Returns(liteDb);

        var dbHealthCheck = new DatabaseHealthCheck(this.dbConnectionFactory, options);

        // Act
        var result = await dbHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.IsType<HealthCheckResult>(result);
        Assert.Equal(result, HealthCheckResult.Healthy("Healthy result from DatabaseHealthCheck"));
    }

    [Fact]
    public async void CheckHealthAsync_DelayOverDegradedThreshold_ReturnsHealthCheckResultDegraded()
    {
        // Arrange
        var liteDb = new LiteDatabase($"filename={this.testDbName};Connection=Shared;", new BsonMapper());
        var options = Options.Create(new HealthCheckOptions { DegradedThresholdMs = 0 });

        this.dbConnectionFactory.GetConnection().Returns(liteDb);

        var dbHealthCheck = new DatabaseHealthCheck(this.dbConnectionFactory, options);

        // Act
        var result = await dbHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.IsType<HealthCheckResult>(result);
        Assert.Equal(result, HealthCheckResult.Degraded("Degraded result from DatabaseHealthCheck"));
    }

    [Fact]
    public async void CheckHealthAsync_DelayOverUnhealthyThreshold_ReturnsHealthCheckResultUnhealthy()
    {
        // Arrange
        var liteDb = new LiteDatabase($"filename={this.testDbName};Connection=Shared;", new BsonMapper());
        var options = Options.Create(new HealthCheckOptions { UnhealthyThresholdMs = 0 });

        this.dbConnectionFactory.GetConnection().Returns(liteDb);

        var dbHealthCheck = new DatabaseHealthCheck(this.dbConnectionFactory, options);

        // Act
        var result = await dbHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.IsType<HealthCheckResult>(result);
        Assert.Equal(result, HealthCheckResult.Unhealthy("Unhealthy result from DatabaseHealthCheck"));
    }
}
