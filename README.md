# Codezerg.OpenRouter

A strongly‑typed **.NET Standard 2.0 client library** for [OpenRouter](https://openrouter.ai)’s unified LLM API.  
Access multiple AI models (OpenAI, Anthropic, Google, Meta, Mistral, DeepSeek, etc.) through a single consistent interface.

---

## ✨ Features

- **Unified API**: Chat completions across all supported LLM providers.
- **Streaming Support**: Real‑time token streaming via `IAsyncEnumerable`.
- **Multimodal**: Build conversations with text, image, and audio inputs.
- **Tool Calling & Structured Outputs**: Define and resolve tool calls, enforce JSON/object schemas.
- **Typed Models**: Strongly‑typed request/response models with IntelliSense.
- **Account & Discovery APIs**: Usage, credits, models, providers, generation info.
- **Frontend Client (Experimental)**: Undocumented OpenRouter frontend/private APIs.
- **Cross‑Platform**: Runs on `.NET Framework 4.6.2+`, `.NET Core`, `.NET 5+`.

---

## 📦 Installation

```bash
dotnet add package Codezerg.OpenRouter
```

---

## 🚀 Quick Start

```csharp
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;

var config = new OpenRouterClientOptions
{
    ApiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY"),
    DefaultModel = "deepseek/deepseek-chat-v3.1:free",
    UserAgent = "myapp/1.0",                  // required by OpenRouter
    Referer = "https://github.com/myuser/app" // required by OpenRouter
};

using var client = new OpenRouterClient(config);

var req = new ChatRequest {
    Messages = new() {
        ChatMessage.System("You are a helpful assistant."),
        ChatMessage.User("What is the capital of France?")
    }
};

var resp = await client.SendChatCompletionAsync(req);

Console.WriteLine(resp.Choices[0].Message?.FirstTextContent);
// => "The capital of France is Paris."
```

---

## 📡 Streaming Responses

```csharp
var req = new ChatRequest {
    Messages = new() {
        ChatMessage.User("Write a short poem about a sunrise.")
    }
};

await foreach (var chunk in client.StreamChatCompletionAsync(req))
{
    var token = chunk.Choices?[0].Delta?.Content;
    if (!string.IsNullOrEmpty(token))
        Console.Write(token);
}
```

> ⚠️ In **streaming mode**, generated text arrives in   
> `ChatChoice.Delta.Content`.  
> In **non‑streaming mode**, use `ChatChoice.Message.FirstTextContent`.

---

## 🖼 Multimodal Example

```csharp
var message = new ChatMessage(ChatRole.User)
    .AddText("Describe this picture")
    .AddImage("https://upload.wikimedia.org/wikipedia/commons/3/3a/Cat03.jpg");

var resp = await client.SendChatCompletionAsync(new ChatRequest {
    Messages = new() { message }
});
Console.WriteLine(resp.Choices[0].Message?.FirstTextContent);
```

---

## 🔧 Tool Calling Example

```csharp
var req = new ChatRequest {
    Messages = new() { ChatMessage.User("What's the weather in Paris?") },
    Tools = new() {
        new ToolDefinition {
            Function = new FunctionDescription {
                Name = "get_weather",
                Description = "Gets weather",
                Parameters = JObject.Parse("{\"type\":\"object\",\"properties\":{\"city\":{\"type\":\"string\"}}}")
            }
        }
    }
};

var resp = await client.SendChatCompletionAsync(req);

if (resp.Choices[0].Message?.ToolCalls?.Count > 0) {
    var call = resp.Choices[0].Message.ToolCalls[0];
    Console.WriteLine($"{call.Function.Name}({call.Function.Arguments})");
}
```

---

## 📊 Account & Discovery APIs

```csharp
var credits = await client.GetCreditsAsync();
Console.WriteLine($"Credits: {credits.TotalCredits}, Used: {credits.TotalUsage}");

var activity = await client.GetActivityAsync();
foreach (var day in activity)
    Console.WriteLine($"{day.Date}: {day.Model} - {day.Requests} requests");

var models = await client.GetModelsAsync();
foreach (var m in models)
    Console.WriteLine($"{m.Name} ({m.Id})");
```

---

## ⚙️ Configuration

```csharp
var config = new OpenRouterClientOptions()
    .WithApiKey("your-api-key")
    .WithDefaultModel("openai/gpt-4o-mini")
    .WithUserAgent("myapp/1.0")
    .WithReferer("https://yourapp.com")
    .WithTimeout(TimeSpan.FromSeconds(60));
```

---

## 🧪 Examples

Try runnable demos under [`/examples`](./examples):

- Simple Chat
- Streaming Chat
- Multimodal (Text + Images)
- Image Analysis
- Tool Calling
- Structured JSON Output

Run an example:

```bash
export OPENROUTER_API_KEY="your-api-key"
dotnet run --project examples/Codezerg.OpenRouter.Examples.csproj
```

---

## 📝 Requirements

- **Library**: .NET Standard 2.0+  
- **Examples**: .NET 8+  
- **API Key**: [Get from OpenRouter](https://openrouter.ai/keys)

---

## ⚖️ License

MIT License © [Codezerg](https://github.com/codezerg)

---

📌 **Tip**: Prefer **streaming mode** for interactive apps and incrementally process responses from `Delta`.