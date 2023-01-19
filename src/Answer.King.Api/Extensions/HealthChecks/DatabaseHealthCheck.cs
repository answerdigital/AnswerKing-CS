using System.Diagnostics;
using Answer.King.Domain.Inventory;
using Answer.King.Infrastructure;
using LiteDB;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Answer.King.Api.Extensions.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    public DatabaseHealthCheck(ILiteDbConnectionFactory connections)
    {
        var db = connections.GetConnection();

        this.Collection = db.GetCollection<Category>();
        this.Collection.EnsureIndex("products");
    }

    private ILiteCollection<Category> Collection { get; }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var watch = Stopwatch.StartNew();
        await this.QueryDB();
        watch.Stop();

        var responseTime = watch.ElapsedMilliseconds;

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

    private Task<Category> QueryDB()
    {
        return Task.FromResult(this.Collection.FindOne(c => true));
    }
}
