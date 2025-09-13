# Codezerg.OpenRouter Overview

## Introduction

Codezerg.OpenRouter is a comprehensive .NET client library designed to simplify integration with OpenRouter's unified LLM API. OpenRouter provides a single endpoint to access models from multiple providers including OpenAI, Anthropic, Google, Meta, and many others, eliminating the need to manage multiple API integrations.

## Key Features

### Unified API Access
- Single client interface for all supported LLM providers
- Consistent request/response format across different models
- Automatic routing to the appropriate provider backend

### Streaming Support
- Real-time token-by-token response streaming
- Modern `IAsyncEnumerable<T>` implementation for efficient memory usage
- Server-Sent Events (SSE) parsing for live updates

### Multimodal Capabilities
- Support for text, image, and audio content in messages
- Vision-capable model integration (GPT-4o, Gemini, Claude)
- Flexible content construction with fluent API

### Type Safety
- Strongly-typed request and response models
- Comprehensive IntelliSense support
- Compile-time validation of API interactions

### Platform Compatibility
- .NET Standard 2.0 for broad framework support
- Works with .NET Framework 4.6.1+, .NET Core 2.0+, and .NET 5.0+
- Minimal dependencies for reduced conflicts

## Why Use Codezerg.OpenRouter?

### Problem It Solves
Managing multiple AI provider APIs is complex and time-consuming. Each provider has different:
- Authentication mechanisms
- Request/response formats
- Error handling patterns
- Rate limiting strategies
- Feature sets and capabilities

### Solution
Codezerg.OpenRouter abstracts these differences, providing:
- **Single Integration Point**: One library to access all models
- **Consistent Interface**: Uniform API regardless of the underlying provider
- **Simplified Billing**: Single API key and billing through OpenRouter
- **Model Flexibility**: Easy switching between models without code changes
- **Fallback Support**: Automatic failover between providers

## Architecture Overview

The library follows a clean, modular architecture:

```
OpenRouterClient (Main Interface)
    ├── Configuration (OpenRouterClientOptions)
    ├── HTTP Communication Layer
    ├── Serialization/Deserialization
    └── Models
        ├── Request Models (ChatRequest, etc.)
        ├── Response Models (ChatResponse, etc.)
        └── Shared Types (ChatMessage, ChatRole, etc.)
```

### Core Components

1. **OpenRouterClient**: The main entry point for all API operations
2. **OpenRouterClientOptions**: Configuration container for API keys, timeouts, and defaults
3. **ChatMessage**: Flexible message structure supporting multiple content types
4. **Streaming Pipeline**: Async enumerable implementation for real-time responses

## Use Cases

### Chatbots and Conversational AI
Build sophisticated chatbots with context awareness and multimodal capabilities.

### Content Generation
Generate articles, code, documentation, and creative content using various models.

### Code Analysis and Generation
Leverage code-specialized models for development assistance and automation.

### Image Analysis
Process and analyze images using vision-capable models for descriptions, OCR, and insights.

### Translation and Localization
Use language models for accurate, context-aware translations.

### Data Processing
Extract, transform, and analyze unstructured data using LLM capabilities.

## Model Providers

Access models from leading AI providers:

- **OpenAI**: GPT-4, GPT-3.5, DALL-E
- **Anthropic**: Claude 3 family (Opus, Sonnet, Haiku)
- **Google**: Gemini Pro, PaLM
- **Meta**: Llama 2, Llama 3
- **Mistral**: Mistral Large, Mixtral
- **DeepSeek**: DeepSeek Chat models
- **And many more...**

## Getting Started

To begin using Codezerg.OpenRouter:

1. Install the NuGet package
2. Obtain an API key from [OpenRouter](https://openrouter.ai)
3. Initialize the client with your configuration
4. Start making API calls

See the [Getting Started Guide](getting-started.md) for detailed setup instructions.

## Performance Considerations

- **Efficient Streaming**: Memory-efficient streaming for long responses
- **Connection Pooling**: Reuses HTTP connections for better performance
- **Async Throughout**: Fully asynchronous operations for scalability
- **Minimal Overhead**: Thin wrapper over the OpenRouter API

## Security

- API keys are never logged or exposed
- HTTPS-only communication
- Support for custom headers and authentication
- Configurable timeout protection

## Next Steps

- [Getting Started Guide](getting-started.md) - Set up and make your first API call
- [API Reference](api-reference.md) - Detailed documentation of all classes and methods
- [Examples](examples.md) - Code samples for common scenarios
- [Configuration](configuration.md) - Advanced configuration options