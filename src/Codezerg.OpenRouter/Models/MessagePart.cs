#nullable enable

using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class MessagePart
{
    [JsonProperty("type")]
    public MessageContentType Type { get; set; } = MessageContentType.Text;

    [JsonProperty("text")]
    public string? Text { get; set; }

    [JsonProperty("image_url")]
    public ImageReference? ImageUrl { get; set; }


    [JsonProperty("input_audio")]
    public AudioContent? InputAudio { get; set; }


    // Factory methods for creating different content types
    public static MessagePart CreateText(string text)
    {
        return new MessagePart
        {
            Type = MessageContentType.Text,
            Text = text
        };
    }

    public static MessagePart CreateImage(string url, string? detail = null)
    {
        return new MessagePart
        {
            Type = MessageContentType.ImageUrl,
            ImageUrl = new ImageReference { Url = url, Detail = detail }
        };
    }


    public static MessagePart CreateAudio(string data, string format = "wav")
    {
        return new MessagePart
        {
            Type = MessageContentType.InputAudio,
            InputAudio = new AudioContent { Data = data, Format = format }
        };
    }

    // Helper properties
    [JsonIgnore]
    public bool IsText => Type == MessageContentType.Text;

    [JsonIgnore]
    public bool IsImage => Type == MessageContentType.ImageUrl;


    [JsonIgnore]
    public bool IsAudio => Type == MessageContentType.InputAudio;

}

public class ImageReference
{
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonProperty("detail")]
    public string? Detail { get; set; }
}

public class AudioContent
{
    [JsonProperty("data")]
    public string Data { get; set; } = string.Empty;

    [JsonProperty("format")]
    public string Format { get; set; } = "wav";
}