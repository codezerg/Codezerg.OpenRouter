# Codezerg.OpenRouter

A .NET client library for seamless integration with [OpenRouter](https://openrouter.ai)'s unified LLM API, providing access to multiple AI models through a single, consistent interface.

## Features

- **Unified API**: Access multiple LLM providers (OpenAI, Anthropic, Google, Meta, etc.) through a single client.
- **Streaming Support**: Real-time token-by-token response streaming with `IAsyncEnumerable`.
- **Multimodal Capabilities**: Build messages with text, images, and audio.
- **Type-Safe Models**: Strongly-typed request/response models with IntelliSense support.
- **.NET Standard 2.0**: Compatible with .NET Framework 4.6.1+, .NET Core 2.0+, and .NET 5.0+, 6.0, 7.0, 8.0.
- **Flexible Configuration**: Customize timeouts, headers, and provider-specific options.

## Installation

```bash
dotnet add package Codezerg.OpenRouter
```

## Quick Start

```csharp
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;

// Load API key from environment
var config = new OpenRouterClientOptions
{
    ApiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY"),
    DefaultModel = "deepseek/deepseek-chat-v3.1:free",
    UserAgent = "myapp/1.0",
    Referer = "https://github.com/myuser/myapp"
};

using var client = new OpenRouterClient(config);

// Build request
var request = new ChatRequest
{
    Messages = new List<ChatMessage>
    {
        ChatMessage.System("You are a helpful assistant."),
        ChatMessage.User("What is the capital of France?")
    }
};

// Send request
var response = await client.SendChatCompletionAsync(request);

// Read response
Console.WriteLine(response.Choices[0].Message?.FirstTextContent);
// => "The capital of France is Paris."
```

## Streaming Example

```csharp
var request = new ChatRequest
{
    Messages = new List<ChatMessage>
    {
        ChatMessage.User("Tell me a short story about a robot learning to paint.")
    },
    MaxTokens = 200
};

await foreach (var chunk in client.StreamChatCompletionAsync(request))
{
    if (chunk.Choices?.Count > 0)
    {
        // ⚠️ NOTE: In streaming responses, content comes in Delta, not Message.
        var content = chunk.Choices[0].Delta?.Content;
        if (!string.IsNullOrEmpty(content))
        {
            Console.Write(content);
        }
    }
}
```

> ⚠️ **Important:** In streaming mode, the generated text is delivered incrementally through  
> `ChatChoice.Delta.Content`. In non-streaming requests, you access the full text via   
> `ChatChoice.Message.FirstTextContent`.

## Multimodal Example (Text + Image)

```csharp
var message = new ChatMessage(ChatRole.User)
    .AddText("What do you see in this image?")
    .AddImage("https://upload.wikimedia.org/wikipedia/commons/3/3a/Cat03.jpg");

var request = new ChatRequest
{
    Messages = new List<ChatMessage> { message }
};

var response = await client.SendChatCompletionAsync(request);
Console.WriteLine(response.Choices[0].Message?.FirstTextContent);
```

## Image Analysis Example (from `examples/`)

```csharp
var visionConfig = config.Clone();
visionConfig.DefaultModel = "openai/gpt-5-mini";

using var visionClient = new OpenRouterClient(visionConfig);

var imageUrl = "https://images.unsplash.com/photo-1619507938536-39981994f4e9?w=800&auto=webp";

var message = new ChatMessage(ChatRole.User)
    .AddText("Please describe this image in detail.")
    .AddImage(imageUrl);

var request = new ChatRequest
{
    Messages = new List<ChatMessage> { message }
};

var response = await visionClient.SendChatCompletionAsync(request);
Console.WriteLine(response.Choices[0].Message?.FirstTextContent);
```

## Configuration

Available options in `OpenRouterClientOptions`:

```csharp
var config = new OpenRouterClientOptions
{
    ApiKey = "your-api-key",
    Endpoint = "https://openrouter.ai/api/v1",   // default
    DefaultModel = "deepseek/deepseek-chat-v3.1:free",
    Timeout = TimeSpan.FromSeconds(100),
    UserAgent = "myapp/1.0",                     // required by OpenRouter
    Referer = "https://myapp.com",               // required by OpenRouter
    EnableDebugLogging = false
};
```

Fluent extension methods are available:

```csharp
var cfg = new OpenRouterClientOptions()
    .WithApiKey("your-api-key")
    .WithDefaultModel("openai/gpt-4o-mini")
    .WithUserAgent("my-app/1.0")
    .WithReferer("https://yourapp.com");
```

## Examples

Runnable demos are in the [`/examples`](./examples) folder:

- **Simple Chat**: Make a basic request/response.
- **Streaming Chat**: Receive a response token-by-token (using `Delta.Content`).
- **Multimodal Chat**: Combine text + images.
- **Image Analysis**: Send real world photos to vision models.
- **Image Generation**: Generate images from text prompts.

Run them with:

```bash
export OPENROUTER_API_KEY=your-api-key
dotnet run --project examples/Codezerg.OpenRouter.Examples.csproj
```

## Requirements

- .NET Standard 2.0+ (for library usage).
- .NET 8+ (for the provided examples project).
- An OpenRouter API key ([sign up here](https://openrouter.ai/keys)).

## License

MIT License. See [LICENSE](LICENSE).

---