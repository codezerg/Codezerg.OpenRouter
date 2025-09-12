using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Codezerg.OpenRouter.Models;

public class Tool
{
    [JsonProperty("type")]
    public ToolType Type { get; set; } = ToolType.Function;

    [JsonProperty("function")]
    public FunctionDescription Function { get; set; } = new FunctionDescription();
}

public class FunctionDescription
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("parameters")]
    public JObject? Parameters { get; set; }
}

public class ToolCall
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("type")]
    public ToolType Type { get; set; } = ToolType.Function;

    [JsonProperty("function")]
    public FunctionCall Function { get; set; } = new FunctionCall();
}

public class FunctionCall
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("arguments")]
    public string Arguments { get; set; } = string.Empty;
}

public class ToolChoiceFunction
{
    [JsonProperty("type")]
    public ToolType Type { get; set; } = ToolType.Function;

    [JsonProperty("function")]
    public ToolChoiceFunctionDetails Function { get; set; } = new ToolChoiceFunctionDetails();
}

public class ToolChoiceFunctionDetails
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}