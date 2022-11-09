using System;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Infrastructure.SeedData;
using LiteDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Answer.King.Infrastructure.Extensions.DependencyInjection;

public static class LiteDbServiceCollectionExtensions
{
    public static IServiceCollection ConfigureLiteDb(this IServiceCollection services, Action<LiteDbOptions>? setupAction = null)
    {
        var action = setupAction ?? delegate { };
        var options = new LiteDbOptions();

        action(options);

        options.EntityMappers.ForEach(type => services.Add(ServiceDescriptor.Transient(typeof(IEntityMapping), type)));
        options.DataSeeders.ForEach(type => services.Add(ServiceDescriptor.Transient(typeof(ISeedData), type)));

        return services;
    }

    public static IApplicationBuilder UseLiteDb(this IApplicationBuilder app)
    {
        var mappings = app.ApplicationServices.GetServices<IEntityMapping>();

        foreach (var entityMapping in mappings)
        {
            entityMapping.RegisterMapping(BsonMapper.Global);

            var originalResolver = BsonMapper.Global.ResolveMember;

            BsonMapper.Global.ResolveMember = (type, memberInfo, memberMapper) =>
            {
                originalResolver(type, memberInfo, memberMapper);
                entityMapping.ResolveMember(type, memberInfo, memberMapper);
            };
        }

        var connections = app.ApplicationServices.GetRequiredService<ILiteDbConnectionFactory>();

        var seeders = app.ApplicationServices.GetServices<ISeedData>();
        foreach (var seedData in seeders)
        {
            seedData.SeedData(connections);
        }

        return app;
    }
}
