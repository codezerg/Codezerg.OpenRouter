#nullable enable

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class ChatMessage
{
    [JsonProperty("role")]
    public string Role { get; set; } = "user";

    [JsonProperty("content")]
    [JsonConverter(typeof(ChatMessageContentConverter))]
    public List<ChatContentPart> Content { get; set; } = new List<ChatContentPart>();

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("tool_calls")]
    public List<ToolCall>? ToolCalls { get; set; }

    [JsonProperty("tool_call_id")]
    public string? ToolCallId { get; set; }

    [JsonProperty("images")]
    public List<GeneratedImage>? Images { get; set; }

    // Convenience constructors
    public ChatMessage() { }

    public ChatMessage(string role, string text)
    {
        Role = role;
        Content = new List<ChatContentPart> { ChatContentPart.CreateText(text) };
    }

    public ChatMessage(string role, params ChatContentPart[] parts)
    {
        Role = role;
        Content = parts.ToList();
    }

    // Static factory methods for common cases
    public static ChatMessage User(string text) 
        => new("user", text);

    public static ChatMessage User(params ChatContentPart[] parts) 
        => new("user", parts);

    public static ChatMessage Assistant(string text) 
        => new("assistant", text);

    public static ChatMessage Assistant(params ChatContentPart[] parts) 
        => new("assistant", parts);

    public static ChatMessage System(string text) 
        => new("system", text);

    public static ChatMessage System(params ChatContentPart[] parts) 
        => new("system", parts);

    public static ChatMessage Tool(string content, string toolCallId)
    {
        return new ChatMessage
        {
            Role = "tool",
            Content = new List<ChatContentPart> { ChatContentPart.CreateText(content) },
            ToolCallId = toolCallId
        };
    }

    // Builder methods for fluent API
    public ChatMessage WithName(string name)
    {
        Name = name;
        return this;
    }

    public ChatMessage WithToolCalls(params ToolCall[] toolCalls)
    {
        ToolCalls = toolCalls.ToList();
        return this;
    }

    public ChatMessage WithImages(params GeneratedImage[] images)
    {
        Images = images.ToList();
        return this;
    }

    public ChatMessage AddText(string text)
    {
        Content.Add(ChatContentPart.CreateText(text));
        return this;
    }

    public ChatMessage AddImage(string url, string? detail = null)
    {
        Content.Add(ChatContentPart.CreateImage(url, detail));
        return this;
    }


    public ChatMessage AddAudio(string data, string format = "wav")
    {
        Content.Add(ChatContentPart.CreateAudio(data, format));
        return this;
    }


    // Helper properties for common scenarios
    [JsonIgnore]
    public string? FirstTextContent => Content
        .Where(c => c.IsText)
        .FirstOrDefault()?.Text;

    [JsonIgnore]
    public string CombinedTextContent => string.Join(" ", Content
        .Where(c => c.IsText && c.Text != null)
        .Select(c => c.Text));

    [JsonIgnore]
    public bool HasImages => Content.Any(c => c.IsImage);


    [JsonIgnore]
    public bool HasAudio => Content.Any(c => c.IsAudio);

    [JsonIgnore]
    public bool IsMultimodal => Content.Count > 1 || Content.Any(c => !c.IsText);

    [JsonIgnore]
    public bool IsToolResponse => Role == "tool" && !string.IsNullOrEmpty(ToolCallId);

    [JsonIgnore]
    public bool HasToolCalls => ToolCalls?.Any() == true;
}