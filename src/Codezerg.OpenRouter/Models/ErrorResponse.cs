using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class ErrorResponse
{
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

public class ModerationErrorMetadata
{
    [JsonProperty("reasons")]
    public List<string> Reasons { get; set; } = new List<string>();

    [JsonProperty("flagged_input")]
    public string FlaggedInput { get; set; } = string.Empty;

    [JsonProperty("provider_name")]
    public string ProviderName { get; set; } = string.Empty;

    [JsonProperty("model_slug")]
    public string ModelSlug { get; set; } = string.Empty;
}

public class ProviderErrorMetadata
{
    [JsonProperty("provider_name")]
    public string ProviderName { get; set; } = string.Empty;

    [JsonProperty("raw")]
    public object? Raw { get; set; }
}