using System;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

/// <summary>
/// Represents the type of response format for structured outputs.
/// </summary>
[JsonConverter(typeof(ResponseFormatTypeConverter))]
public record struct ResponseFormatType(string Value)
{
    /// <summary>
    /// JSON object format - ensures the response is valid JSON.
    /// </summary>
    public static readonly ResponseFormatType JsonObject = new ResponseFormatType("json_object");
    
    /// <summary>
    /// JSON schema format - validates response against a provided schema.
    /// </summary>
    public static readonly ResponseFormatType JsonSchema = new ResponseFormatType("json_schema");
    
    /// <summary>
    /// Content format - used for predicted content responses.
    /// </summary>
    public static readonly ResponseFormatType Content = new ResponseFormatType("content");

    /// <summary>
    /// Returns the string representation of the response format type.
    /// </summary>
    public override string ToString() => Value;
}

/// <summary>
/// JSON converter for the ResponseFormatType type.
/// </summary>
public class ResponseFormatTypeConverter : JsonConverter<ResponseFormatType>
{
    public override ResponseFormatType ReadJson(JsonReader reader, Type objectType, ResponseFormatType existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return new ResponseFormatType(reader.Value?.ToString() ?? string.Empty);
    }

    public override void WriteJson(JsonWriter writer, ResponseFormatType value, JsonSerializer serializer)
    {
        writer.WriteValue(value.Value);
    }
}