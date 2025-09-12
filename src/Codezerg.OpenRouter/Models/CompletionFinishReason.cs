using System;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

/// <summary>
/// Represents the reason why the model stopped generating tokens.
/// OpenRouter normalizes finish reasons across different providers.
/// </summary>
[JsonConverter(typeof(CompletionFinishReasonConverter))]
public record struct CompletionFinishReason(string Value)
{
    /// <summary>
    /// The model hit a natural stop point or a provided stop sequence.
    /// </summary>
    public static readonly CompletionFinishReason Stop = new CompletionFinishReason("stop");
    
    /// <summary>
    /// The maximum number of tokens specified in the request was reached.
    /// </summary>
    public static readonly CompletionFinishReason Length = new CompletionFinishReason("length");
    
    /// <summary>
    /// The model called a tool/function.
    /// </summary>
    public static readonly CompletionFinishReason ToolCalls = new CompletionFinishReason("tool_calls");
    
    /// <summary>
    /// Content was omitted due to a content filter.
    /// </summary>
    public static readonly CompletionFinishReason ContentFilter = new CompletionFinishReason("content_filter");
    
    /// <summary>
    /// An error occurred during generation.
    /// </summary>
    public static readonly CompletionFinishReason Error = new CompletionFinishReason("error");

    /// <summary>
    /// Returns the string representation of the finish reason.
    /// </summary>
    public override string ToString() => Value;
}

/// <summary>
/// JSON converter for the CompletionFinishReason type.
/// </summary>
public class CompletionFinishReasonConverter : JsonConverter<CompletionFinishReason?>
{
    public override CompletionFinishReason? ReadJson(JsonReader reader, Type objectType, CompletionFinishReason? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
            return null;
        
        return new CompletionFinishReason(reader.Value.ToString() ?? string.Empty);
    }

    public override void WriteJson(JsonWriter writer, CompletionFinishReason? value, JsonSerializer serializer)
    {
        if (value == null)
            writer.WriteNull();
        else
            writer.WriteValue(value.Value.Value);
    }
}