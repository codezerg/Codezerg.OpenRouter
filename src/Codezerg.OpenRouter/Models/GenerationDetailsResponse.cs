using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class GenerationDetailsResponse
{
    [JsonProperty("data")]
    public GenerationDetails Data { get; set; } = new GenerationDetails();
}

public class GenerationDetails
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("total_cost")]
    public double TotalCost { get; set; }

    [JsonProperty("created_at")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonProperty("model")]
    public string Model { get; set; } = string.Empty;

    [JsonProperty("origin")]
    public string? Origin { get; set; }

    [JsonProperty("usage")]
    public double Usage { get; set; }

    [JsonProperty("is_byok")]
    public bool IsByok { get; set; }

    [JsonProperty("upstream_id")]
    public string? UpstreamId { get; set; }

    [JsonProperty("cache_discount")]
    public double? CacheDiscount { get; set; }

    [JsonProperty("upstream_inference_cost")]
    public double UpstreamInferenceCost { get; set; }

    [JsonProperty("app_id")]
    public int? AppId { get; set; }

    [JsonProperty("streamed")]
    public bool Streamed { get; set; }

    [JsonProperty("cancelled")]
    public bool Cancelled { get; set; }

    [JsonProperty("provider_name")]
    public string? ProviderName { get; set; }

    [JsonProperty("latency")]
    public int Latency { get; set; }

    [JsonProperty("moderation_latency")]
    public int? ModerationLatency { get; set; }

    [JsonProperty("generation_time")]
    public int GenerationTime { get; set; }

    [JsonProperty("finish_reason")]
    public string? FinishReason { get; set; }

    [JsonProperty("native_finish_reason")]
    public string? NativeFinishReason { get; set; }

    [JsonProperty("tokens_prompt")]
    public int TokensPrompt { get; set; }

    [JsonProperty("tokens_completion")]
    public int TokensCompletion { get; set; }

    [JsonProperty("native_tokens_prompt")]
    public int NativeTokensPrompt { get; set; }

    [JsonProperty("native_tokens_completion")]
    public int NativeTokensCompletion { get; set; }

    [JsonProperty("native_tokens_reasoning")]
    public int NativeTokensReasoning { get; set; }

    [JsonProperty("num_media_prompt")]
    public int? NumMediaPrompt { get; set; }

    [JsonProperty("num_media_completion")]
    public int NumMediaCompletion { get; set; }

    [JsonProperty("num_search_results")]
    public int? NumSearchResults { get; set; }

    [JsonProperty("native_tokens_cached")]
    public int? NativeTokensCached { get; set; }

    [JsonProperty("native_tokens_completion_images")]
    public int? NativeTokensCompletionImages { get; set; }

    [JsonProperty("external_user")]
    public string? ExternalUser { get; set; }

    [JsonProperty("api_type")]
    public string? ApiType { get; set; }
}