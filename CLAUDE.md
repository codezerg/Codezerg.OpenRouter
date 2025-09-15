# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Codezerg.OpenRouter is a .NET Standard 2.0 client library for OpenRouter's unified LLM API, enabling access to multiple AI models (OpenAI, Anthropic, Google, Meta) through a single interface. The library provides streaming support, multimodal capabilities, and type-safe request/response models.

## Build and Development Commands

### Building the Solution
```bash
# Build entire solution
dotnet build

# Build release configuration
dotnet build -c Release

# Build specific project
dotnet build src/Codezerg.OpenRouter/Codezerg.OpenRouter.csproj
```

### Running Examples
```bash
# Set API key (required)
# Windows PowerShell:
$env:OPENROUTER_API_KEY="your-api-key"
# Linux/Mac:
export OPENROUTER_API_KEY="your-api-key"

# Run examples
dotnet run --project examples/Codezerg.OpenRouter.Examples.csproj
```

### Testing
```bash
# Run all tests (when test project is added)
dotnet test

# Run with verbosity
dotnet test --logger "console;verbosity=detailed"
```

### Package Management
```bash
# Create NuGet package
dotnet pack src/Codezerg.OpenRouter/Codezerg.OpenRouter.csproj -c Release

# Restore dependencies
dotnet restore
```

## Architecture and Key Components

### Core Client Structure
The library follows a modular architecture with clear separation of concerns:

- **OpenRouterClient** (`src/Codezerg.OpenRouter/OpenRouterClient.cs`): Main client class handling HTTP communication, request serialization, and response streaming
- **OpenRouterClientOptions** (`src/Codezerg.OpenRouter/OpenRouterClientOptions.cs`): Configuration class for API keys, timeouts, default headers, and model selection
- **Models namespace** (`src/Codezerg.OpenRouter/Models/`): Contains all request/response DTOs and enums

### Key Design Patterns

1. **Fluent Builder Pattern**: ChatMessage uses fluent methods for multimodal content construction:
   ```csharp
   var message = new ChatMessage(ChatRole.User)
       .AddText("Analyze this")
       .AddImage("url");
   ```

2. **Async Streaming**: Uses `IAsyncEnumerable<T>` for token-by-token streaming responses, enabling real-time output processing.  
   ⚠️ **Important:** In streaming responses, output is received incrementally in the **`ChatChoice.Delta`** property (`Delta.Content`, `Delta.ToolCalls`, etc.). The **`Message`** property is only populated in *non-streaming* responses.

3. **Custom JSON Converters**: Implements custom converters for complex types (MessageContentConverter, NullableChatRoleConverter) to handle OpenRouter's API response format

### Request/Response Flow

1. Client validates configuration and constructs HTTP request with authentication headers
2. ChatRequest is serialized with proper handling of optional fields and provider-specific options
3. For streaming: Parses Server-Sent Events (SSE) and yields ChatResponse chunks  
   ⚠️ Use `ChatChoice.Delta.Content`, not `ChatChoice.Message`, when handling streaming chunks.
4. For non-streaming: Deserializes complete response with error handling

### Model Organization

- **ChatMessage**: Supports multiple content types (text, image, audio) through MessagePart collection
- **ChatRequest**: Configurable with model selection, temperature, streaming options, and provider-specific settings
- **ChatResponse**: Handles both complete responses and streaming deltas with usage statistics

## Project Structure

```
/
├── src/
│   └── Codezerg.OpenRouter/       # Main library project
│       ├── Models/                 # Request/response models and enums
│       ├── OpenRouterClient.cs     # Core client implementation
│       └── *.cs                    # Supporting classes
├── examples/                       # Runnable examples demonstrating features
└── Codezerg.OpenRouter.sln        # Solution file
```

## Development Guidelines

When modifying the codebase:

1. Maintain .NET Standard 2.0 compatibility for broad framework support
2. Preserve nullable reference type annotations for API clarity
3. Follow existing async/await patterns and use ConfigureAwait(false) in library code
4. Ensure all public APIs have XML documentation comments
5. Keep the streaming implementation using IAsyncEnumerable for modern async patterns
6. Test multimodal features with appropriate vision-capable models (e.g., Gemini, GPT-4o)
7. **Always document clearly**: Streaming responses use `Delta.Content`; non-streaming uses `Message.FirstTextContent`.
