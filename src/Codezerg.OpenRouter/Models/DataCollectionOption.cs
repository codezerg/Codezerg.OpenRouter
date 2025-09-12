using System;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

/// <summary>
/// Represents data collection preferences for provider routing.
/// </summary>
[JsonConverter(typeof(DataCollectionOptionConverter))]
public record struct DataCollectionOption(string Value)
{
    /// <summary>
    /// Allow data collection by the provider.
    /// </summary>
    public static readonly DataCollectionOption Allow = new DataCollectionOption("allow");
    
    /// <summary>
    /// Deny data collection by the provider.
    /// </summary>
    public static readonly DataCollectionOption Deny = new DataCollectionOption("deny");

    /// <summary>
    /// Returns the string representation of the data collection option.
    /// </summary>
    public override string ToString() => Value;
}

/// <summary>
/// JSON converter for the DataCollectionOption type.
/// </summary>
public class DataCollectionOptionConverter : JsonConverter<DataCollectionOption?>
{
    public override DataCollectionOption? ReadJson(JsonReader reader, Type objectType, DataCollectionOption? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
            return null;
        
        return new DataCollectionOption(reader.Value.ToString() ?? string.Empty);
    }

    public override void WriteJson(JsonWriter writer, DataCollectionOption? value, JsonSerializer serializer)
    {
        if (value == null)
            writer.WriteNull();
        else
            writer.WriteValue(value.Value.Value);
    }
}