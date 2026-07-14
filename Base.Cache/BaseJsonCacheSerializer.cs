using System.Text.Json;
using Base.Contracts.Cache;

namespace Base.Cache;

/// <summary>
/// Provides a System.Text.Json serializer for typed cache values.
/// </summary>
public class BaseJsonCacheSerializer : IBaseCacheSerializer
{
    private readonly JsonSerializerOptions? _serializerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseJsonCacheSerializer"/> class.
    /// </summary>
    /// <param name="serializerOptions">The optional serializer options used for cache payloads.</param>
    public BaseJsonCacheSerializer(JsonSerializerOptions? serializerOptions = default)
    {
        _serializerOptions = serializerOptions;
    }

    /// <summary>
    /// Serializes a typed cache value to UTF-8 JSON bytes.
    /// </summary>
    public virtual byte[] Serialize<TValue>(TValue? value)
    {
        return JsonSerializer.SerializeToUtf8Bytes(value, _serializerOptions);
    }

    /// <summary>
    /// Deserializes UTF-8 JSON bytes into a typed cache value.
    /// </summary>
    public virtual TValue? Deserialize<TValue>(byte[] value)
    {
        return JsonSerializer.Deserialize<TValue>(value, _serializerOptions);
    }
}
