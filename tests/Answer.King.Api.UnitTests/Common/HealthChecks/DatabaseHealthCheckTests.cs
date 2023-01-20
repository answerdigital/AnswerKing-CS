using System.Reflection;
using Answer.King.Api.Common.HealthChecks;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Infrastructure;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using LiteDB;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;
using Xunit;

namespace Answer.King.Api.UnitTests.Common.HealthChecks;

[TestCategory(TestType.Unit)]
public class DatabaseHealthCheckTests
{
    private readonly string testDbName = $"Answer.King.{Guid.NewGuid()}.db";
    private readonly ILiteDbConnectionFactory dbConnectionFactory = Substitute.For<ILiteDbConnectionFactory>();
    private readonly IStopwatch stopwatch = Substitute.For<IStopwatch>();

    private FieldInfo? DatabaseHealthCheckStopwatch { get; }
        = typeof(DatabaseHealthCheck).GetField("<Stopwatch>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

    [Fact]
    public async void CheckHealthAsync_DelayUnder100ms_ReturnsHealthCheckResultHealthy()
    {
        var liteDb = new LiteDatabase($"filename={this.testDbName};Connection=Shared;", new BsonMapper());

        // Arrange
        this.dbConnectionFactory.GetConnection().Returns(liteDb);

        var dbHealthCheck = new DatabaseHealthCheck(this.dbConnectionFactory);
        var healthCheckContext = new HealthCheckContext();

        // Act
        var result = await dbHealthCheck.CheckHealthAsync(healthCheckContext);

        // Assert
        Assert.IsType<HealthCheckResult>(result);
        Assert.Equal(result, HealthCheckResult.Healthy("Healthy result from DatabaseHealthCheck"));
    }

    [Fact]
    public async void CheckHealthAsync_DelayOver100ms_ReturnsHealthCheckResultDegraded()
    {
        var liteDb = new LiteDatabase($"filename={this.testDbName};Connection=Shared;", new BsonMapper());

        // Arrange
        this.dbConnectionFactory.GetConnection().Returns(liteDb);

        this.stopwatch.GetElapsedTime(Arg.Any<long>()).Returns(TimeSpan.FromMilliseconds(101));

        var dbHealthCheck = new DatabaseHealthCheck(this.dbConnectionFactory);
        var healthCheckContext = new HealthCheckContext();

        this.DatabaseHealthCheckStopwatch?.SetValue(dbHealthCheck, this.stopwatch);

        // Act
        var result = await dbHealthCheck.CheckHealthAsync(healthCheckContext);

        // Assert
        Assert.IsType<HealthCheckResult>(result);
        Assert.Equal(result, HealthCheckResult.Degraded("Degraded result from DatabaseHealthCheck"));
    }

    [Fact]
    public async void CheckHealthAsync_DelayOver200ms_ReturnsHealthCheckResultUnhealthy()
    {
        var liteDb = new LiteDatabase($"filename={this.testDbName};Connection=Shared;", new BsonMapper());

        // Arrange
        this.dbConnectionFactory.GetConnection().Returns(liteDb);

        this.stopwatch.GetElapsedTime(Arg.Any<long>()).Returns(TimeSpan.FromMilliseconds(201));

        var dbHealthCheck = new DatabaseHealthCheck(this.dbConnectionFactory);
        var healthCheckContext = new HealthCheckContext();

        this.DatabaseHealthCheckStopwatch?.SetValue(dbHealthCheck, this.stopwatch);

        // Act
        var result = await dbHealthCheck.CheckHealthAsync(healthCheckContext);

        // Assert
        Assert.IsType<HealthCheckResult>(result);
        Assert.Equal(result, HealthCheckResult.Unhealthy("Unhealthy result from DatabaseHealthCheck"));
    }
}
