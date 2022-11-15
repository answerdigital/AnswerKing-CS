using Alba;
using Xunit;

namespace Answer.King.Api.IntegrationTests.Common;

public class WebFixtures : IAsyncLifetime
{
    public IAlbaHost AlbaHost = null!;

    public async Task InitializeAsync()
    {
        this.AlbaHost = await Alba.AlbaHost.For<Program>();
    }

    public async Task DisposeAsync()
    {
        await this.AlbaHost.DisposeAsync();
        File.Delete(".\\Answer.King.db");
    }
}
