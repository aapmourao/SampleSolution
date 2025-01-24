using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SharedKernel.Infrastructure.Persistence.Converters;

public class ObjectToJsonConverter<T> : ValueConverter<T, string>
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ObjectToJsonConverter() : base(
        v => JsonSerializer.Serialize(v, _jsonSerializerOptions),
        v => JsonSerializer.Deserialize<T>(v, _jsonSerializerOptions)!)
    {
    }
}

public class ObjectToJsonComparer<T> : ValueComparer<T>
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ObjectToJsonComparer() : base(
        (c1, c2) => JsonSerializer.Serialize(c1, (JsonSerializerOptions)null!) == JsonSerializer.Serialize(c2, _jsonSerializerOptions),
        c => c == null ? 0 : JsonSerializer.Serialize(c, (JsonSerializerOptions)null!).GetHashCode(),
        c => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(c, _jsonSerializerOptions), _jsonSerializerOptions)!)
    {
    }
}