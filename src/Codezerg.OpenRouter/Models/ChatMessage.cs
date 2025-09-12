#nullable enable

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class ChatMessage
{
    [JsonProperty("role")]
    public ChatRole Role { get; set; } = ChatRole.User;

    [JsonProperty("content")]
    [JsonConverter(typeof(MessageContentConverter))]
    public List<MessagePart> Content { get; set; } = new List<MessagePart>();

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

    public ChatMessage(ChatRole role, string text)
    {
        Role = role;
        Content = new List<MessagePart> { MessagePart.CreateText(text) };
    }

    public ChatMessage(ChatRole role, params MessagePart[] parts)
    {
        Role = role;
        Content = parts.ToList();
    }

    // Static factory methods for common cases
    public static ChatMessage User(string text) 
        => new(ChatRole.User, text);

    public static ChatMessage User(params MessagePart[] parts) 
        => new(ChatRole.User, parts);

    public static ChatMessage Assistant(string text) 
        => new(ChatRole.Assistant, text);

    public static ChatMessage Assistant(params MessagePart[] parts) 
        => new(ChatRole.Assistant, parts);

    public static ChatMessage System(string text) 
        => new(ChatRole.System, text);

    public static ChatMessage System(params MessagePart[] parts) 
        => new(ChatRole.System, parts);

    public static ChatMessage Tool(string content, string toolCallId)
    {
        return new ChatMessage
        {
            Role = ChatRole.Tool,
            Content = new List<MessagePart> { MessagePart.CreateText(content) },
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
        Content.Add(MessagePart.CreateText(text));
        return this;
    }

    public ChatMessage AddImage(string url, string? detail = null)
    {
        Content.Add(MessagePart.CreateImage(url, detail));
        return this;
    }


    public ChatMessage AddAudio(string data, string format = "wav")
    {
        Content.Add(MessagePart.CreateAudio(data, format));
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
    public bool IsToolResponse => Role == ChatRole.Tool && !string.IsNullOrEmpty(ToolCallId);

    [JsonIgnore]
    public bool HasToolCalls => ToolCalls?.Any() == true;
}