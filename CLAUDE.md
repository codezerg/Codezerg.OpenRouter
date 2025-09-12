# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Development Commands

```bash
# Build the entire solution
dotnet build

# Build in Release mode
dotnet build -c Release

# Restore NuGet packages
dotnet restore

# Clean build artifacts
dotnet clean

# Run examples (requires OPENROUTER_API_KEY environment variable)
dotnet run --project examples/Codezerg.OpenRouter.Examples.csproj


# Create NuGet package
dotnet pack src/Codezerg.OpenRouter/Codezerg.OpenRouter.csproj -c Release
```

## High-Level Architecture

The Codezerg.OpenRouter library is a .NET client for OpenRouter's unified LLM API, targeting .NET Standard 2.0 for broad compatibility.

### Core Components

**OpenRouterClient** (`src/Codezerg.OpenRouter/OpenRouterClient.cs`): Main client class that handles HTTP communication with the OpenRouter API. It manages authentication, request/response serialization, and supports both synchronous and streaming responses.

**OpenRouterClientOptions** (`src/Codezerg.OpenRouter/OpenRouterClientOptions.cs`): Configuration object that encapsulates API credentials, default model settings, timeouts, and HTTP headers. Supports cloning to prevent external modifications after initialization.

**Model System**: Located in `src/Codezerg.OpenRouter/Models/`, uses strongly-typed value objects for enums (ChatRole, MessageContentType, CompletionFinishReason, etc.) instead of traditional enums, providing better type safety and extensibility. Key request/response models include:
- `ChatRequest`: Main request object for chat completions
- `ChatResponse`: Response object containing model outputs
- `ChatMessage`: Represents individual messages in a conversation
- `MessagePart`: Supports multimodal content (text, images, audio)
- `ChatChoice`: Individual completion choice with message and metadata
- `ChatDelta`: Incremental updates for streaming responses
- `TokenUsage`: Token consumption metrics

### Project Structure

- **src/Codezerg.OpenRouter**: Core library (.NET Standard 2.0)
- **examples/**: Runnable examples demonstrating various features (chat, streaming, multimodal, image generation)
- **docs/**: API documentation for different features

### Key Model Classes (After Refactoring)

**Core Classes:**
- `OpenRouterClient`: Main client for API communication
- `OpenRouterClientOptions`: Configuration and settings
- `ChatSession`: Manages conversation state

**Request/Response Models:**
- `ChatRequest` / `ChatResponse`: Main chat completion models
- `ChatChoice`: Individual completion choice
- `ChatDelta`: Streaming response updates
- `TokenUsage`: Token consumption metrics

**Message Components:**
- `ChatMessage`: Individual conversation messages
- `MessagePart`: Multimodal content parts
- `ImageReference`: Image content references
- `AudioContent`: Audio content data

**Configuration Models:**
- `ProviderOptions`: Provider-specific settings
- `ResponseFormatOptions`: Output format configuration
- `PredictionOptions`: Prediction behavior settings
- `ToolDefinition`: Function/tool definitions

**Error Handling:**
- `ApiError`: Main error response
- `ModerationErrorDetails`: Content moderation errors
- `ProviderErrorDetails`: Provider-specific errors

### Key Design Patterns

1. **Multimodal Support**: The library uses `MessagePart` objects to support mixed content types (text, images, audio) in messages, with specialized converters for JSON serialization.
2. **Streaming Responses**: Implements async enumerable pattern for streaming chat completions, allowing real-time token-by-token response processing.
3. **Configuration Management**: Uses immutable configuration with cloning to prevent runtime modifications after client initialization.
4. **Custom JSON Converters**: Implements custom Newtonsoft.Json converters for complex types like `MessageContentConverter` and `NullableChatRoleConverter` to handle OpenRouter's API format requirements.

## Environment Setup

The library requires an OpenRouter API key to function. Set the `OPENROUTER_API_KEY` environment variable before running examples or demos.

## Dependencies

- Newtonsoft.Json 13.0.3 (JSON serialization)
- Microsoft.Bcl.AsyncInterfaces 8.0.0 (async support for .NET Standard 2.0)