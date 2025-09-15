using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class ActivityResponse
{
    [JsonProperty("data")]
    public List<Activity> Data { get; set; } = new List<Activity>();
}

public class Activity
{
    [JsonProperty("date")]
    public string Date { get; set; } = string.Empty;

    [JsonProperty("model")]
    public string Model { get; set; } = string.Empty;

    [JsonProperty("model_permaslug")]
    public string ModelPermaslug { get; set; } = string.Empty;

    [JsonProperty("endpoint_id")]
    public string EndpointId { get; set; } = string.Empty;

    [JsonProperty("provider_name")]
    public string ProviderName { get; set; } = string.Empty;

    [JsonProperty("usage")]
    public double Usage { get; set; }

    [JsonProperty("byok_usage_inference")]
    public double ByokUsageInference { get; set; }

    [JsonProperty("requests")]
    public int Requests { get; set; }

    [JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonProperty("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonProperty("reasoning_tokens")]
    public int ReasoningTokens { get; set; }
}