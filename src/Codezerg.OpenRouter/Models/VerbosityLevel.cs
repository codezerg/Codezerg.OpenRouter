using System;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

/// <summary>
/// Represents the verbosity level for model responses.
/// Controls the length and detail of generated content.
/// </summary>
[JsonConverter(typeof(VerbosityLevelConverter))]
public record struct VerbosityLevel(string Value)
{
    /// <summary>
    /// Low verbosity - produces more concise responses.
    /// </summary>
    public static readonly VerbosityLevel Low = new VerbosityLevel("low");
    
    /// <summary>
    /// Medium verbosity - balanced response length (default).
    /// </summary>
    public static readonly VerbosityLevel Medium = new VerbosityLevel("medium");
    
    /// <summary>
    /// High verbosity - produces more detailed and comprehensive responses.
    /// </summary>
    public static readonly VerbosityLevel High = new VerbosityLevel("high");

    /// <summary>
    /// Returns the string representation of the verbosity level.
    /// </summary>
    public override string ToString() => Value;    
}

/// <summary>
/// JSON converter for the VerbosityLevel type.
/// </summary>
public class VerbosityLevelConverter : JsonConverter<VerbosityLevel?>
{
    public override VerbosityLevel? ReadJson(JsonReader reader, Type objectType, VerbosityLevel? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
            return null;
        
        return new VerbosityLevel(reader.Value.ToString() ?? string.Empty);
    }

    public override void WriteJson(JsonWriter writer, VerbosityLevel? value, JsonSerializer serializer)
    {
        if (value == null)
            writer.WriteNull();
        else
            writer.WriteValue(value.Value.Value);
    }
}