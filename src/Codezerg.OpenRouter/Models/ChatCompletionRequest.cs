#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class ChatCompletionRequest
{
    [JsonProperty("messages")]
    public List<ChatMessage>? Messages { get; set; }

    [JsonProperty("prompt")]
    public string? Prompt { get; set; }

    [JsonProperty("model")]
    public string? Model { get; set; }

    [JsonProperty("response_format")]
    public ResponseFormat? ResponseFormat { get; set; }

    [JsonProperty("stop", NullValueHandling = NullValueHandling.Ignore)]
    public object? Stop { get; set; }

    [JsonProperty("stream")]
    public bool? Stream { get; set; }

    [JsonProperty("max_tokens")]
    public int? MaxTokens { get; set; }

    [JsonProperty("temperature")]
    public double? Temperature { get; set; }

    [JsonProperty("tools")]
    public List<Tool>? Tools { get; set; }

    [JsonProperty("tool_choice")]
    public object? ToolChoice { get; set; }

    [JsonProperty("seed")]
    public int? Seed { get; set; }

    [JsonProperty("top_p")]
    public double? TopP { get; set; }

    [JsonProperty("top_k")]
    public int? TopK { get; set; }

    [JsonProperty("frequency_penalty")]
    public double? FrequencyPenalty { get; set; }

    [JsonProperty("presence_penalty")]
    public double? PresencePenalty { get; set; }

    [JsonProperty("repetition_penalty")]
    public double? RepetitionPenalty { get; set; }

    [JsonProperty("logit_bias")]
    public Dictionary<int, double>? LogitBias { get; set; }

    [JsonProperty("logprobs")]
    public bool? Logprobs { get; set; }

    [JsonProperty("top_logprobs")]
    public int? TopLogprobs { get; set; }

    [JsonProperty("min_p")]
    public double? MinP { get; set; }

    [JsonProperty("top_a")]
    public double? TopA { get; set; }

    [JsonProperty("prediction")]
    public Prediction? Prediction { get; set; }

    [JsonProperty("transforms")]
    public List<string>? Transforms { get; set; }

    [JsonProperty("models")]
    public List<string>? Models { get; set; }

    [JsonProperty("route")]
    public string? Route { get; set; }

    [JsonProperty("provider")]
    public ProviderPreferences? Provider { get; set; }

    [JsonProperty("user")]
    public string? User { get; set; }

    [JsonProperty("modalities")]
    public List<Modality>? Modalities { get; set; }

    [JsonProperty("parallel_tool_calls")]
    public bool? ParallelToolCalls { get; set; }

    [JsonProperty("verbosity")]
    public VerbosityLevel? Verbosity { get; set; }
}
