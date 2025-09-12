using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class ProviderOptions
{
    [JsonProperty("allow_fallbacks")]
    public bool? AllowFallbacks { get; set; }

    [JsonProperty("require_parameters")]
    public bool? RequireParameters { get; set; }

    [JsonProperty("data_collection")]
    public DataCollectionOption? DataCollection { get; set; }

    [JsonProperty("order")]
    public List<string>? Order { get; set; }

    [JsonProperty("ignore")]
    public List<string>? Ignore { get; set; }

    [JsonProperty("allow")]
    public List<string>? Allow { get; set; }

    [JsonProperty("block")]
    public List<string>? Block { get; set; }
}

public class ResponseFormatOptions
{
    [JsonProperty("type")]
    public ResponseFormatType Type { get; set; } = ResponseFormatType.JsonObject;

    [JsonProperty("json_schema")]
    public object? JsonSchema { get; set; }
}

public class PredictionOptions
{
    [JsonProperty("type")]
    public PredictionType Type { get; set; } = PredictionType.Content;

    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;
}