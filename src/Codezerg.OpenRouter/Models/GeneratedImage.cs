using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class GeneratedImage
{
    [JsonProperty("type")]
    public string Type { get; set; } = "image_url";

    [JsonProperty("image_url")]
    public GeneratedImageUrl ImageUrl { get; set; } = new GeneratedImageUrl();
}

public class GeneratedImageUrl
{
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}