using System;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class NullableChatRoleConverter : JsonConverter<ChatRole?>
{
    public override ChatRole? ReadJson(JsonReader reader, Type objectType, ChatRole? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
            return null;
        
        return new ChatRole(reader.Value.ToString() ?? string.Empty);
    }

    public override void WriteJson(JsonWriter writer, ChatRole? value, JsonSerializer serializer)
    {
        if (value == null)
            writer.WriteNull();
        else
            writer.WriteValue(value.Value.Value);
    }
}