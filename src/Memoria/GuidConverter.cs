using System.Text.Json;
using System.Text.Json.Serialization;

namespace Memoria;

internal class GuidConverter : JsonConverter<Guid>
{
    /// <inheritdoc />
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return string.IsNullOrEmpty(value) ? Guid.Empty : Guid.ParseExact(value, "N");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("N"));
    }
}