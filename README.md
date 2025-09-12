# Codezerg.OpenRouter

A .NET client library for seamless integration with OpenRouter's unified LLM API, providing access to multiple AI models through a single, consistent interface.

## Features

- **Unified API**: Access multiple LLM providers (OpenAI, Anthropic, Google, Meta, etc.) through a single client
- **Streaming Support**: Real-time token-by-token response streaming with `IAsyncEnumerable`
- **Multimodal Capabilities**: Support for text, images, and audio content in messages
- **Type-Safe Models**: Strongly-typed request/response models with comprehensive IntelliSense support
- **.NET Standard 2.0**: Compatible with .NET Framework 4.6.1+, .NET Core 2.0+, and .NET 5.0+
- **Flexible Configuration**: Customizable timeouts, headers, and provider-specific options

## Installation

```bash
dotnet add package Codezerg.OpenRouter
```

## Quick Start

```csharp
using System.Collections.Generic;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;

// Initialize the client
var config = new OpenRouterClientOptions
{
    ApiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY"),
    DefaultModel = "openai/gpt-4"
};

var client = new OpenRouterClient(config);

// Send a chat request
var request = new ChatRequest
{
    Messages = new List<ChatMessage>
    {
        ChatMessage.User("Hello! How can you help me today?")
    }
};

var response = await client.SendChatCompletionAsync(request);
Console.WriteLine(response.Choices[0].Message.FirstTextContent);
```

## Streaming Example

```csharp
var streamRequest = new ChatRequest
{
    Messages = new List<ChatMessage>
    {
        ChatMessage.User("Write a short story about a robot learning to paint.")
    },
    Stream = true
};

await foreach (var chunk in client.StreamChatCompletionAsync(streamRequest))
{
    if (chunk.Choices?.Count > 0 && chunk.Choices[0].Delta?.Content != null)
    {
        Console.Write(chunk.Choices[0].Delta.Content);
    }
}
```

## Multimodal Example

```csharp
var multimodalMessage = new ChatMessage(ChatRole.User)
    .AddText("What's in this image?")
    .AddImage("https://example.com/image.jpg");

var multimodalRequest = new ChatRequest
{
    Messages = new List<ChatMessage> { multimodalMessage }
};

var response = await client.SendChatCompletionAsync(multimodalRequest);
```

## Configuration

The `OpenRouterClientOptions` class provides extensive configuration options:

```csharp
var config = new OpenRouterClientOptions
{
    ApiKey = "your-api-key",
    BaseUrl = "https://openrouter.ai/api/v1", // Optional: custom endpoint
    DefaultModel = "openai/gpt-4",
    Timeout = TimeSpan.FromSeconds(60),
    DefaultHeaders = new Dictionary<string, string>
    {
        ["HTTP-Referer"] = "https://your-app.com",
        ["X-Title"] = "Your App Name"
    }
};
```

## Available Models

OpenRouter provides access to models from multiple providers:

- **OpenAI**: GPT-4, GPT-3.5-Turbo, DALL-E
- **Anthropic**: Claude 3 (Opus, Sonnet, Haiku)
- **Google**: Gemini Pro, PaLM
- **Meta**: Llama 2, Llama 3
- **And many more...**

Check [OpenRouter's model list](https://openrouter.ai/models) for the complete catalog.

## Examples

The [/examples](./examples) directory contains runnable examples demonstrating various features:

```bash
# Run all examples
dotnet run --project examples/Codezerg.OpenRouter.Examples.csproj

# Set your API key first
export OPENROUTER_API_KEY="your-api-key"
```

## Requirements

- .NET Standard 2.0 or higher
- An OpenRouter API key ([sign up here](https://openrouter.ai))

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

For issues and questions:
- Open an issue on [GitHub](https://github.com/codezerg/Codezerg.OpenRouter/issues)
- Visit [OpenRouter's documentation](https://openrouter.ai/docs)
