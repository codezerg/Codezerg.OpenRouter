using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class ChatResponse
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("choices")]
    public List<ChatChoice> Choices { get; set; } = new List<ChatChoice>();

    [JsonProperty("created")]
    public long Created { get; set; }

    [JsonProperty("model")]
    public string Model { get; set; } = string.Empty;

    [JsonProperty("object")]
    public ObjectType Object { get; set; } = ObjectType.ChatCompletion;

    [JsonProperty("system_fingerprint")]
    public string? SystemFingerprint { get; set; }

    [JsonProperty("usage")]
    public TokenUsage? Usage { get; set; }

    [JsonProperty("error")]
    public ApiError? Error { get; set; }
}

public class ChatChoice
{
    [JsonProperty("index")]
    public int Index { get; set; }

    [JsonProperty("finish_reason")]
    public CompletionFinishReason? FinishReason { get; set; }

    [JsonProperty("native_finish_reason")]
    public CompletionFinishReason? NativeFinishReason { get; set; }

    [JsonProperty("error")]
    public ApiError? Error { get; set; }

    [JsonProperty("message")]
    public ChatMessage? Message { get; set; }

    [JsonProperty("delta")]
    public ChatDelta? Delta { get; set; }

    [JsonProperty("text")]
    public string? Text { get; set; }
}

public class ChatDelta
{
    [JsonProperty("content")]
    public string? Content { get; set; }

    [JsonProperty("role")]
    [JsonConverter(typeof(NullableChatRoleConverter))]
    public ChatRole? Role { get; set; }

    [JsonProperty("tool_calls")]
    public List<ToolCall>? ToolCalls { get; set; }

    [JsonProperty("images")]
    public List<GeneratedImage>? Images { get; set; }
}

public class TokenUsage
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
