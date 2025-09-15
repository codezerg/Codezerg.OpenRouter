using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class ModelEndpointsResponse
{
    [JsonProperty("data")]
    public ModelEndpoints Data { get; set; } = new ModelEndpoints();
}

public class ModelEndpoints
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("created")]
    public double Created { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("architecture")]
    public ModelArchitecture? Architecture { get; set; }

    [JsonProperty("endpoints")]
    public List<EndpointInfo> Endpoints { get; set; } = new List<EndpointInfo>();
}

public class EndpointInfo
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("context_length")]
    public double? ContextLength { get; set; }

    [JsonProperty("pricing")]
    public EndpointPricing? Pricing { get; set; }

    [JsonProperty("provider_name")]
    public string? ProviderName { get; set; }

    [JsonProperty("supported_parameters")]
    public List<string>? SupportedParameters { get; set; }

    [JsonProperty("quantization")]
    public string? Quantization { get; set; }

    [JsonProperty("max_completion_tokens")]
    public double? MaxCompletionTokens { get; set; }

    [JsonProperty("max_prompt_tokens")]
    public double? MaxPromptTokens { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; }

    [JsonProperty("uptime_last_30m")]
    public double? UptimeLast30m { get; set; }
}

public class EndpointPricing
{
    [JsonProperty("request")]
    public string? Request { get; set; }

    [JsonProperty("image")]
    public string? Image { get; set; }

    [JsonProperty("prompt")]
    public string? Prompt { get; set; }

    [JsonProperty("completion")]
    public string? Completion { get; set; }
}