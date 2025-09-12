using System;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

/// <summary>
/// Represents the input/output modalities supported by a model.
/// </summary>
[JsonConverter(typeof(ModalityConverter))]
public record struct Modality(string Value)
{
    /// <summary>
    /// Text modality for text input/output.
    /// </summary>
    public static readonly Modality Text = new Modality("text");
    
    /// <summary>
    /// Image modality for image input/output (vision models or image generation).
    /// </summary>
    public static readonly Modality Image = new Modality("image");
    
    /// <summary>
    /// Audio modality for audio input/output.
    /// </summary>
    public static readonly Modality Audio = new Modality("audio");

    /// <summary>
    /// Returns the string representation of the modality.
    /// </summary>
    public override string ToString() => Value;
}

/// <summary>
/// JSON converter for the Modality type.
/// </summary>
public class ModalityConverter : JsonConverter<Modality>
{
    public override Modality ReadJson(JsonReader reader, Type objectType, Modality existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return new Modality(reader.Value?.ToString() ?? string.Empty);
    }

    public override void WriteJson(JsonWriter writer, Modality value, JsonSerializer serializer)
    {
        writer.WriteValue(value.Value);
    }
}