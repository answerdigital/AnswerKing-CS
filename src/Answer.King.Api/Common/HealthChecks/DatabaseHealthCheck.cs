using System.Diagnostics;
using Answer.King.Domain.Inventory;
using Answer.King.Infrastructure;
using LiteDB;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Answer.King.Api.Common.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly LiteDatabase liteDB;

    public DatabaseHealthCheck(ILiteDbConnectionFactory connections)
    {
        this.liteDB = connections.GetConnection();
    }

    private IStopwatch Stopwatch { get; } = new MyStopwatch();

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        this.Stopwatch.Start();
        await this.QueryDB();
        this.Stopwatch.Stop();

        var responseTime = this.Stopwatch.ElapsedMilliseconds;

        if (responseTime < 100)
        {
            return await Task.FromResult(HealthCheckResult.Healthy("Healthy result from DatabaseHealthCheck"));
        }
        else if (responseTime < 200)
        {
            return await Task.FromResult(HealthCheckResult.Degraded("Degraded result from DatabaseHealthCheck"));
        }

        return await Task.FromResult(HealthCheckResult.Unhealthy("Unhealthy result from DatabaseHealthCheck"));
    }

    public Task<Category> QueryDB()
    {
        var collection = this.liteDB.GetCollection<Category>();
        collection.EnsureIndex("products");

        return Task.FromResult(collection.FindOne(c => true));
    }
}

public class MyStopwatch : Stopwatch, IStopwatch
{
}
