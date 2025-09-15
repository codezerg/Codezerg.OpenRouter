using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Frontend
{
    /// <summary>
    /// Represents a model from the OpenRouter frontend API
    /// </summary>
    public class FrontendModel
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("hf_slug")]
        public string HfSlug { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("context_length")]
        public int ContextLength { get; set; }

        [JsonProperty("input_modalities")]
        public List<string> InputModalities { get; set; }

        [JsonProperty("output_modalities")]
        public List<string> OutputModalities { get; set; }

        [JsonProperty("has_text_output")]
        public bool HasTextOutput { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("instruct_type")]
        public string InstructType { get; set; }

        [JsonProperty("default_system")]
        public string DefaultSystem { get; set; }

        [JsonProperty("default_stops")]
        public List<string> DefaultStops { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("permaslug")]
        public string Permaslug { get; set; }

        [JsonProperty("reasoning_config")]
        public ReasoningConfig ReasoningConfig { get; set; }

        [JsonProperty("endpoint")]
        public ModelEndpoint Endpoint { get; set; }
    }

    public class ReasoningConfig
    {
        [JsonProperty("start_token")]
        public string StartToken { get; set; }

        [JsonProperty("end_token")]
        public string EndToken { get; set; }

        [JsonProperty("system_prompt")]
        public string SystemPrompt { get; set; }
    }

    public class ModelEndpoint
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("context_length")]
        public int ContextLength { get; set; }

        [JsonProperty("model_variant_slug")]
        public string ModelVariantSlug { get; set; }

        [JsonProperty("model_variant_permaslug")]
        public string ModelVariantPermaslug { get; set; }

        [JsonProperty("provider_name")]
        public string ProviderName { get; set; }

        [JsonProperty("provider_slug")]
        public string ProviderSlug { get; set; }

        [JsonProperty("pricing")]
        public ModelPricing Pricing { get; set; }

        [JsonProperty("variable_pricings")]
        public List<VariablePricing> VariablePricings { get; set; }

        [JsonProperty("supported_parameters")]
        public List<string> SupportedParameters { get; set; }

        [JsonProperty("is_free")]
        public bool IsFree { get; set; }

        [JsonProperty("can_abort")]
        public bool CanAbort { get; set; }

        [JsonProperty("max_prompt_tokens")]
        public int? MaxPromptTokens { get; set; }

        [JsonProperty("max_completion_tokens")]
        public int? MaxCompletionTokens { get; set; }

        [JsonProperty("supports_reasoning")]
        public bool SupportsReasoning { get; set; }

        [JsonProperty("supports_multipart")]
        public bool SupportsMultipart { get; set; }

        [JsonProperty("supports_tool_parameters")]
        public bool SupportsToolParameters { get; set; }

        [JsonProperty("limit_rpm")]
        public int? LimitRpm { get; set; }

        [JsonProperty("limit_rpd")]
        public int? LimitRpd { get; set; }

        [JsonProperty("stats")]
        public ModelStats Stats { get; set; }

        [JsonProperty("features")]
        public ModelFeatures Features { get; set; }
    }

    public class ModelPricing
    {
        [JsonProperty("prompt")]
        public string Prompt { get; set; }

        [JsonProperty("completion")]
        public string Completion { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("request")]
        public string Request { get; set; }

        [JsonProperty("web_search")]
        public string WebSearch { get; set; }

        [JsonProperty("internal_reasoning")]
        public string InternalReasoning { get; set; }

        [JsonProperty("image_output")]
        public string ImageOutput { get; set; }
    }

    public class VariablePricing
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("threshold")]
        public string Threshold { get; set; }

        [JsonProperty("request")]
        public string Request { get; set; }
    }

    public class ModelStats
    {
        [JsonProperty("endpoint_id")]
        public string EndpointId { get; set; }

        [JsonProperty("p50_throughput")]
        public double P50Throughput { get; set; }

        [JsonProperty("p50_latency")]
        public double P50Latency { get; set; }

        [JsonProperty("request_count")]
        public int RequestCount { get; set; }
    }

    public class ModelFeatures
    {
        [JsonProperty("supports_tool_choice")]
        public ToolChoiceSupport SupportsToolChoice { get; set; }

        [JsonProperty("reasoning_config")]
        public ReasoningConfig ReasoningConfig { get; set; }
    }

    public class ToolChoiceSupport
    {
        [JsonProperty("literal_none")]
        public bool LiteralNone { get; set; }

        [JsonProperty("literal_auto")]
        public bool LiteralAuto { get; set; }

        [JsonProperty("literal_required")]
        public bool LiteralRequired { get; set; }

        [JsonProperty("type_function")]
        public bool TypeFunction { get; set; }
    }

    public class FrontendModelResponse
    {
        [JsonProperty("data")]
        public object Data { get; set; }  // Can be either List<FrontendModel> or FrontendModelData
    }

    public class FrontendModelData
    {
        [JsonProperty("models")]
        public List<FrontendModel> Models { get; set; }
    }

    public class FrontendModelStatsResponse
    {
        [JsonProperty("data")]
        public List<ModelEndpointStats> Data { get; set; }
    }

    public class ModelEndpointStats : ModelEndpoint
    {
        [JsonProperty("model")]
        public FrontendModel Model { get; set; }

        [JsonProperty("provider_info")]
        public FrontendProvider ProviderInfo { get; set; }

        [JsonProperty("data_policy")]
        public DataPolicy DataPolicy { get; set; }
    }
}