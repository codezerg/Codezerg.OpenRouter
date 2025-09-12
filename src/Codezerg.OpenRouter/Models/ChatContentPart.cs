#nullable enable

using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class ChatContentPart
{
    [JsonProperty("type")]
    public string Type { get; set; } = "text";

    [JsonProperty("text")]
    public string? Text { get; set; }

    [JsonProperty("image_url")]
    public ImageUrl? ImageUrl { get; set; }


    [JsonProperty("input_audio")]
    public AudioData? InputAudio { get; set; }


    // Factory methods for creating different content types
    public static ChatContentPart CreateText(string text)
    {
        return new ChatContentPart
        {
            Type = "text",
            Text = text
        };
    }

    public static ChatContentPart CreateImage(string url, string? detail = null)
    {
        return new ChatContentPart
        {
            Type = "image_url",
            ImageUrl = new ImageUrl { Url = url, Detail = detail }
        };
    }


    public static ChatContentPart CreateAudio(string data, string format = "wav")
    {
        return new ChatContentPart
        {
            Type = "input_audio",
            InputAudio = new AudioData { Data = data, Format = format }
        };
    }

    // Helper properties
    [JsonIgnore]
    public bool IsText => Type == "text";

    [JsonIgnore]
    public bool IsImage => Type == "image_url";


    [JsonIgnore]
    public bool IsAudio => Type == "input_audio";

}

public class ImageUrl
{
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonProperty("detail")]
    public string? Detail { get; set; }
}

public class AudioData
{
    [JsonProperty("data")]
    public string Data { get; set; } = string.Empty;

    [JsonProperty("format")]
    public string Format { get; set; } = "wav";
}