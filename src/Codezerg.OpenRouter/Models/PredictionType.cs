using System;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

/// <summary>
/// Represents the type of prediction for latency optimization.
/// </summary>
[JsonConverter(typeof(PredictionTypeConverter))]
public record struct PredictionType(string Value)
{
    /// <summary>
    /// Content prediction type - provides the model with predicted output to reduce latency.
    /// </summary>
    public static readonly PredictionType Content = new PredictionType("content");

    /// <summary>
    /// Returns the string representation of the prediction type.
    /// </summary>
    public override string ToString() => Value;
}

/// <summary>
/// JSON converter for the PredictionType type.
/// </summary>
public class PredictionTypeConverter : JsonConverter<PredictionType>
{
    public override PredictionType ReadJson(JsonReader reader, Type objectType, PredictionType existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return new PredictionType(reader.Value?.ToString() ?? string.Empty);
    }

    public override void WriteJson(JsonWriter writer, PredictionType value, JsonSerializer serializer)
    {
        writer.WriteValue(value.Value);
    }
}