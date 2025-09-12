using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Codezerg.OpenRouter.Models;

public class ChatMessageContentConverter : JsonConverter<List<ChatContentPart>>
{
    public override List<ChatContentPart> ReadJson(JsonReader reader, Type objectType, List<ChatContentPart>? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        
        // If it's a string, convert to a text content part
        if (token.Type == JTokenType.String)
        {
            return new List<ChatContentPart>
            {
                ChatContentPart.CreateText(token.Value<string>() ?? string.Empty)
            };
        }
        
        // If it's an array, deserialize normally
        if (token.Type == JTokenType.Array)
        {
            var list = new List<ChatContentPart>();
            foreach (var item in token)
            {
                var part = item.ToObject<ChatContentPart>(serializer);
                if (part != null)
                    list.Add(part);
            }
            return list;
        }
        
        // If null, return empty list
        if (token.Type == JTokenType.Null)
        {
            return new List<ChatContentPart>();
        }
        
        throw new JsonSerializationException($"Unexpected token type for content: {token.Type}");
    }

    public override void WriteJson(JsonWriter writer, List<ChatContentPart>? value, JsonSerializer serializer)
    {
        // If there's only one text part, write it as a string
        if (value?.Count == 1 && value[0].IsText && !string.IsNullOrEmpty(value[0].Text))
        {
            writer.WriteValue(value[0].Text);
        }
        else
        {
            // Otherwise write as array
            serializer.Serialize(writer, value);
        }
    }
}