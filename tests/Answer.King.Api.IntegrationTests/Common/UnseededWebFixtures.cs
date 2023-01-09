using Alba;
using Answer.King.Infrastructure.SeedData;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Answer.King.Api.IntegrationTests.Common;

public class UnseededWebFixtures : IAsyncLifetime
{
    public IAlbaHost AlbaHost = null!;

    private readonly string TestDbName = $"Answer.King.{Guid.NewGuid()}.db";

    public async Task InitializeAsync()
    {
        this.AlbaHost = await Alba.AlbaHost.For<Program>(hostBuilder =>
        {
            hostBuilder.UseSetting("ConnectionStrings:AnswerKing", $"filename={this.TestDbName};Connection=Shared;");
            hostBuilder.ConfigureServices(services =>
            {
                var seeds = services.Where(s => s.ServiceType == typeof(ISeedData)).ToList();
                seeds.ForEach(seeds => services.Remove(seeds));
            });
        });
    }

    public async Task DisposeAsync()
    {
        await this.AlbaHost.DisposeAsync();
        File.Delete($".\\{this.TestDbName}");
    }
}


