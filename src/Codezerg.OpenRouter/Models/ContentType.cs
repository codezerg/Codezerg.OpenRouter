using System;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

/// <summary>
/// Represents the type of content in a chat message part.
/// </summary>
[JsonConverter(typeof(ContentTypeConverter))]
public record struct ContentType(string Value)
{
    /// <summary>
    /// Text content type for regular text messages.
    /// </summary>
    public static readonly ContentType Text = new ContentType("text");
    
    /// <summary>
    /// Image URL content type for images (either URLs or base64 encoded).
    /// </summary>
    public static readonly ContentType ImageUrl = new ContentType("image_url");
    
    /// <summary>
    /// Input audio content type for audio data.
    /// </summary>
    public static readonly ContentType InputAudio = new ContentType("input_audio");

    /// <summary>
    /// Returns the string representation of the content type.
    /// </summary>
    public override string ToString() => Value;
}

/// <summary>
/// JSON converter for the ContentType type.
/// </summary>
public class ContentTypeConverter : JsonConverter<ContentType>
{
    public override ContentType ReadJson(JsonReader reader, Type objectType, ContentType existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return new ContentType(reader.Value?.ToString() ?? string.Empty);
    }

    public override void WriteJson(JsonWriter writer, ContentType value, JsonSerializer serializer)
    {
        writer.WriteValue(value.Value);
    }
}