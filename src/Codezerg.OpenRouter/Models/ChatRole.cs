using System;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

/// <summary>
/// Represents the role of a participant in a chat conversation.
/// </summary>
[JsonConverter(typeof(ChatRoleConverter))]
public record struct ChatRole(string Value)
{
    /// <summary>
    /// System message role, typically used for instructions or context.
    /// </summary>
    public static readonly ChatRole System = new ChatRole("system");
    
    /// <summary>
    /// User message role, representing input from the user.
    /// </summary>
    public static readonly ChatRole User = new ChatRole("user");
    
    /// <summary>
    /// Assistant message role, representing responses from the AI model.
    /// </summary>
    public static readonly ChatRole Assistant = new ChatRole("assistant");
    
    /// <summary>
    /// Tool message role, representing results from tool/function calls.
    /// </summary>
    public static readonly ChatRole Tool = new ChatRole("tool");

    /// <summary>
    /// Returns the string representation of the chat role.
    /// </summary>
    public override string ToString() => Value;
}

/// <summary>
/// JSON converter for the ChatRole type.
/// </summary>
public class ChatRoleConverter : JsonConverter<ChatRole>
{
    public override ChatRole ReadJson(JsonReader reader, Type objectType, ChatRole existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return new ChatRole(reader.Value?.ToString() ?? string.Empty);
    }

    public override void WriteJson(JsonWriter writer, ChatRole value, JsonSerializer serializer)
    {
        writer.WriteValue(value.Value);
    }
}