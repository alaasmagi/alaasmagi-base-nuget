namespace Base.Contracts.Cache;

/// <summary>
/// Defines serialization for typed cache values.
/// </summary>
public interface IBaseCacheSerializer
{
    /// <summary>
    /// Serializes a typed value into a byte payload suitable for a key-value cache backend.
    /// </summary>
    /// <param name="value">The typed value to serialize.</param>
    /// <typeparam name="TValue">The value type being serialized.</typeparam>
    /// <returns>The serialized value bytes.</returns>
    public byte[] Serialize<TValue>(TValue? value);

    /// <summary>
    /// Deserializes a byte payload from a key-value cache backend into a typed value.
    /// </summary>
    /// <param name="value">The serialized value bytes.</param>
    /// <typeparam name="TValue">The expected value type.</typeparam>
    /// <returns>The deserialized value.</returns>
    public TValue? Deserialize<TValue>(byte[] value);
}
