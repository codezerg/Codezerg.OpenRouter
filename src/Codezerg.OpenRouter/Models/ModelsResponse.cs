using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class ModelsResponse
{
    [JsonProperty("data")]
    public List<ModelInfo> Data { get; set; } = new List<ModelInfo>();
}

public class ModelInfo
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("created")]
    public long Created { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("architecture")]
    public ModelArchitecture? Architecture { get; set; }

    [JsonProperty("top_provider")]
    public TopProviderInfo? TopProvider { get; set; }

    [JsonProperty("pricing")]
    public ModelPricing? Pricing { get; set; }

    [JsonProperty("canonical_slug")]
    public string? CanonicalSlug { get; set; }

    [JsonProperty("context_length")]
    public int? ContextLength { get; set; }

    [JsonProperty("hugging_face_id")]
    public string? HuggingFaceId { get; set; }

    [JsonProperty("per_request_limits")]
    public Dictionary<string, object>? PerRequestLimits { get; set; }

    [JsonProperty("supported_parameters")]
    public List<string>? SupportedParameters { get; set; }
}

public class ModelArchitecture
{
    [JsonProperty("input_modalities")]
    public List<string> InputModalities { get; set; } = new List<string>();

    [JsonProperty("output_modalities")]
    public List<string> OutputModalities { get; set; } = new List<string>();

    [JsonProperty("tokenizer")]
    public string? Tokenizer { get; set; }

    [JsonProperty("instruct_type")]
    public string? InstructType { get; set; }
}

public class TopProviderInfo
{
    [JsonProperty("is_moderated")]
    public bool IsModerated { get; set; }

    [JsonProperty("context_length")]
    public int? ContextLength { get; set; }

    [JsonProperty("max_completion_tokens")]
    public int? MaxCompletionTokens { get; set; }
}

public class ModelPricing
{
    [JsonProperty("prompt")]
    public string? Prompt { get; set; }

    [JsonProperty("completion")]
    public string? Completion { get; set; }

    [JsonProperty("image")]
    public string? Image { get; set; }

    [JsonProperty("request")]
    public string? Request { get; set; }

    [JsonProperty("web_search")]
    public string? WebSearch { get; set; }

    [JsonProperty("internal_reasoning")]
    public string? InternalReasoning { get; set; }

    [JsonProperty("input_cache_read")]
    public string? InputCacheRead { get; set; }

    [JsonProperty("input_cache_write")]
    public string? InputCacheWrite { get; set; }
}