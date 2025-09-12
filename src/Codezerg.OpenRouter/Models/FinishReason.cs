using System;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

/// <summary>
/// Represents the reason why the model stopped generating tokens.
/// OpenRouter normalizes finish reasons across different providers.
/// </summary>
[JsonConverter(typeof(FinishReasonConverter))]
public record struct FinishReason(string Value)
{
    /// <summary>
    /// The model hit a natural stop point or a provided stop sequence.
    /// </summary>
    public static readonly FinishReason Stop = new FinishReason("stop");
    
    /// <summary>
    /// The maximum number of tokens specified in the request was reached.
    /// </summary>
    public static readonly FinishReason Length = new FinishReason("length");
    
    /// <summary>
    /// The model called a tool/function.
    /// </summary>
    public static readonly FinishReason ToolCalls = new FinishReason("tool_calls");
    
    /// <summary>
    /// Content was omitted due to a content filter.
    /// </summary>
    public static readonly FinishReason ContentFilter = new FinishReason("content_filter");
    
    /// <summary>
    /// An error occurred during generation.
    /// </summary>
    public static readonly FinishReason Error = new FinishReason("error");

    /// <summary>
    /// Returns the string representation of the finish reason.
    /// </summary>
    public override string ToString() => Value;
}

/// <summary>
/// JSON converter for the FinishReason type.
/// </summary>
public class FinishReasonConverter : JsonConverter<FinishReason?>
{
    public override FinishReason? ReadJson(JsonReader reader, Type objectType, FinishReason? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
            return null;
        
        return new FinishReason(reader.Value.ToString() ?? string.Empty);
    }

    public override void WriteJson(JsonWriter writer, FinishReason? value, JsonSerializer serializer)
    {
        if (value == null)
            writer.WriteNull();
        else
            writer.WriteValue(value.Value.Value);
    }
}