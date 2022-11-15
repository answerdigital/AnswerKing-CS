using Swashbuckle.AspNetCore.SwaggerGen;

namespace Answer.King.Api.Extensions.DependencyInjection;

public static class SwaggerGenOptionsExtensions
{
    private const string AnswerKingPrefix = "AnswerKing";

    public static void UseCustomSchemaIdSelectorStrategy(this SwaggerGenOptions options)
    {
        options.CustomSchemaIds(CustomSchemaIdSelector);
    }

    private static string CustomSchemaIdSelector(Type modelType)
    {
        if (!modelType.IsConstructedGenericType)
        {
            var schemaId = modelType.Name.Replace("[]", "Array");
            if (modelType.Namespace!.EndsWith("RequestModels", StringComparison.OrdinalIgnoreCase))
            {
                schemaId = $"RequestModels.{schemaId}";
            }

            if (modelType.Namespace!.Contains("Domain.Orders", StringComparison.OrdinalIgnoreCase))
            {
                schemaId = $"Orders.{schemaId}";
            }

            if (modelType.Namespace!.Contains("Domain.Inventory", StringComparison.OrdinalIgnoreCase))
            {
                schemaId = $"Inventory.{schemaId}";
            }

            if (modelType.Namespace!.Contains("Domain.Repositories", StringComparison.OrdinalIgnoreCase))
            {
                schemaId = $"Entity.{schemaId}";
            }

            return $"{AnswerKingPrefix}.{schemaId}";
        }

        var prefix = modelType.GetGenericArguments()
            .Select(genericArg => CustomSchemaIdSelector(genericArg))
            .Aggregate((previous, current) => previous + current);

        var genericSchemaId = modelType.Name.Split('`').First();
        if (modelType.Namespace!.EndsWith("RequestModels", StringComparison.OrdinalIgnoreCase))
        {
            genericSchemaId = $"RequestModels.{genericSchemaId}";
        }

        if (modelType.Namespace!.Contains("Domain.Orders", StringComparison.OrdinalIgnoreCase))
        {
            genericSchemaId = $"Orders.{genericSchemaId}";
        }

        if (modelType.Namespace!.Contains("Domain.Inventory", StringComparison.OrdinalIgnoreCase))
        {
            genericSchemaId = $"Inventory.{genericSchemaId}";
        }

        if (modelType.Namespace!.Contains("Domain.Repositories", StringComparison.OrdinalIgnoreCase))
        {
            genericSchemaId = $"Entity.{genericSchemaId}";
        }

        return $"{AnswerKingPrefix}.{prefix}.{genericSchemaId}";
    }
}
