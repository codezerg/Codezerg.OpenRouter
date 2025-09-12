using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class ChatCompletionResponse
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("choices")]
    public List<ChatCompletionChoice> Choices { get; set; } = new List<ChatCompletionChoice>();

    [JsonProperty("created")]
    public long Created { get; set; }

    [JsonProperty("model")]
    public string Model { get; set; } = string.Empty;

    [JsonProperty("object")]
    public string Object { get; set; } = string.Empty;

    [JsonProperty("system_fingerprint")]
    public string? SystemFingerprint { get; set; }

    [JsonProperty("usage")]
    public Usage? Usage { get; set; }

    [JsonProperty("error")]
    public ErrorResponse? Error { get; set; }
}

public class ChatCompletionChoice
{
    [JsonProperty("index")]
    public int Index { get; set; }

    [JsonProperty("finish_reason")]
    public string? FinishReason { get; set; }

    [JsonProperty("native_finish_reason")]
    public string? NativeFinishReason { get; set; }

    [JsonProperty("error")]
    public ErrorResponse? Error { get; set; }

    [JsonProperty("message")]
    public ChatMessage? Message { get; set; }

    [JsonProperty("delta")]
    public Delta? Delta { get; set; }

    [JsonProperty("text")]
    public string? Text { get; set; }
}

public class Delta
{
    [JsonProperty("content")]
    public string? Content { get; set; }

    [JsonProperty("role")]
    public string? Role { get; set; }

    [JsonProperty("tool_calls")]
    public List<ToolCall>? ToolCalls { get; set; }

    [JsonProperty("images")]
    public List<GeneratedImage>? Images { get; set; }
}

public class Usage
{
    [JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonProperty("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonProperty("total_tokens")]
    public int TotalTokens { get; set; }

    [JsonProperty("cache_creation_input_tokens")]
    public int? CacheCreationInputTokens { get; set; }

    [JsonProperty("cache_read_input_tokens")]
    public int? CacheReadInputTokens { get; set; }
}
