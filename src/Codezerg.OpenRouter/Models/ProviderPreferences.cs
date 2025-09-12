using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class ProviderPreferences
{
    [JsonProperty("allow_fallbacks")]
    public bool? AllowFallbacks { get; set; }

    [JsonProperty("require_parameters")]
    public bool? RequireParameters { get; set; }

    [JsonProperty("data_collection")]
    public string? DataCollection { get; set; }

    [JsonProperty("order")]
    public List<string>? Order { get; set; }

    [JsonProperty("ignore")]
    public List<string>? Ignore { get; set; }

    [JsonProperty("allow")]
    public List<string>? Allow { get; set; }

    [JsonProperty("block")]
    public List<string>? Block { get; set; }
}

public class ResponseFormat
{
    [JsonProperty("type")]
    public string Type { get; set; } = "json_object";

    [JsonProperty("json_schema")]
    public object? JsonSchema { get; set; }
}

public class Prediction
{
    [JsonProperty("type")]
    public string Type { get; set; } = "content";

    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;
}