using System;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

/// <summary>
/// Represents the type of tool that can be used in function calling.
/// </summary>
[JsonConverter(typeof(ToolTypeConverter))]
public record struct ToolType(string Value)
{
    /// <summary>
    /// Function tool type, used for function/tool calling.
    /// </summary>
    public static readonly ToolType Function = new ToolType("function");

    /// <summary>
    /// Returns the string representation of the tool type.
    /// </summary>
    public override string ToString() => Value;
}

/// <summary>
/// JSON converter for the ToolType type.
/// </summary>
public class ToolTypeConverter : JsonConverter<ToolType>
{
    public override ToolType ReadJson(JsonReader reader, Type objectType, ToolType existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return new ToolType(reader.Value?.ToString() ?? string.Empty);
    }

    public override void WriteJson(JsonWriter writer, ToolType value, JsonSerializer serializer)
    {
        writer.WriteValue(value.Value);
    }
}