using System;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

/// <summary>
/// Represents the type of content in a chat message part.
/// </summary>
[JsonConverter(typeof(MessageContentTypeConverter))]
public record struct MessageContentType(string Value)
{
    /// <summary>
    /// Text content type for regular text messages.
    /// </summary>
    public static readonly MessageContentType Text = new MessageContentType("text");
    
    /// <summary>
    /// Image URL content type for images (either URLs or base64 encoded).
    /// </summary>
    public static readonly MessageContentType ImageUrl = new MessageContentType("image_url");
    
    /// <summary>
    /// Input audio content type for audio data.
    /// </summary>
    public static readonly MessageContentType InputAudio = new MessageContentType("input_audio");

    /// <summary>
    /// Returns the string representation of the content type.
    /// </summary>
    public override string ToString() => Value;
}

/// <summary>
/// JSON converter for the MessageContentType type.
/// </summary>
public class MessageContentTypeConverter : JsonConverter<MessageContentType>
{
    public override MessageContentType ReadJson(JsonReader reader, Type objectType, MessageContentType existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return new MessageContentType(reader.Value?.ToString() ?? string.Empty);
    }

    public override void WriteJson(JsonWriter writer, MessageContentType value, JsonSerializer serializer)
    {
        writer.WriteValue(value.Value);
    }
}