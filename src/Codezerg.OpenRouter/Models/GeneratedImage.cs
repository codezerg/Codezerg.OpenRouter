using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class GeneratedImage
{
    [JsonProperty("type")]
    public string Type { get; set; } = "image_url";

    [JsonProperty("image_url")]
    public GeneratedImageReference ImageUrl { get; set; } = new GeneratedImageReference();
}

public class GeneratedImageReference
{
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}