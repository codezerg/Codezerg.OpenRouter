using System;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

/// <summary>
/// Represents the type of object returned in the response.
/// </summary>
[JsonConverter(typeof(ObjectTypeConverter))]
public record struct ObjectType(string Value)
{
    /// <summary>
    /// Represents a complete chat completion response.
    /// </summary>
    public static readonly ObjectType ChatCompletion = new ObjectType("chat.completion");
    
    /// <summary>
    /// Represents a streaming chunk of a chat completion response.
    /// </summary>
    public static readonly ObjectType ChatCompletionChunk = new ObjectType("chat.completion.chunk");

    /// <summary>
    /// Returns the string representation of the object type.
    /// </summary>
    public override string ToString() => Value;
}

/// <summary>
/// JSON converter for the ObjectType type.
/// </summary>
public class ObjectTypeConverter : JsonConverter<ObjectType>
{
    public override ObjectType ReadJson(JsonReader reader, Type objectType, ObjectType existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return new ObjectType(reader.Value?.ToString() ?? string.Empty);
    }

    public override void WriteJson(JsonWriter writer, ObjectType value, JsonSerializer serializer)
    {
        writer.WriteValue(value.Value);
    }
}