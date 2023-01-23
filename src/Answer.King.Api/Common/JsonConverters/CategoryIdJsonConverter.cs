using System.Text.Json;
using System.Text.Json.Serialization;
using Answer.King.Api.RequestModels;

namespace Answer.King.Api.Common.JsonConverters;

public class CategoryIdJsonConverter : JsonConverter<CategoryId>
{
    public override CategoryId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetInt64(out var id))
        {
            return new CategoryId { Id = id };
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, CategoryId value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Id);
    }
}
