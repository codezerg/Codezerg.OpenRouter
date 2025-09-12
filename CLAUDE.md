# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Codezerg.OpenRouter is a .NET client library for OpenRouter's unified LLM API, providing access to multiple AI models through a single consistent interface. The library targets .NET Standard 2.0 for broad compatibility.

## Commands

### Build
```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build src/Codezerg.OpenRouter/Codezerg.OpenRouter.csproj

# Build in Release mode
dotnet build -c Release
```

### Test
Currently no test project exists. When adding tests, create a test project in the solution structure.

### Run Examples
```bash
# Set API key first (required)
set OPENROUTER_API_KEY=your-api-key   # Windows
export OPENROUTER_API_KEY=your-api-key # Linux/Mac

# Run examples
dotnet run --project examples/Codezerg.OpenRouter.Examples.csproj
```

### Package
```bash
# Create NuGet package
dotnet pack src/Codezerg.OpenRouter/Codezerg.OpenRouter.csproj -c Release
```

## Architecture

### Core Components

**OpenRouterClient** (`src/Codezerg.OpenRouter/OpenRouterClient.cs`)
- Main client class for API interactions
- Handles HTTP communication, authentication, and request/response serialization
- Supports both synchronous completion and streaming responses via `IAsyncEnumerable`
- Manages connection lifecycle and proper resource disposal

**OpenRouterClientOptions** (`src/Codezerg.OpenRouter/OpenRouterClientOptions.cs`)
- Configuration class for client settings
- Handles API key, base URL, default model, timeouts, and custom headers
- Provides validation and cloning for configuration immutability

### Models Structure

The `Models` namespace contains strongly-typed representations of API entities:

- **ChatMessage/ChatRequest/ChatResponse**: Core chat completion models
- **MessagePart**: Multimodal content support (text, images, audio)
- **ChatRole**: Enum-like value object for message roles (system, user, assistant, tool)
- **ToolDefinition**: Function calling/tool use definitions
- **ProviderOptions**: Provider-specific configuration options

### Key Design Patterns

1. **Fluent API for Message Building**: `ChatMessage` supports method chaining for adding multimodal content
2. **Custom JSON Converters**: `MessageContentConverter` and `NullableChatRoleConverter` handle complex serialization scenarios
3. **Value Objects for Enums**: Type-safe alternatives to enums (e.g., `ChatRole`, `ResponseFormatType`) with string conversion support
4. **Async Streaming**: Uses `IAsyncEnumerable` for efficient token-by-token streaming

### Extension Points

- Custom HTTP client injection for advanced scenarios
- Provider-specific options through `ProviderOptions` dictionary
- Extensible message content types via `MessagePart` hierarchy

## Important Notes

- The library uses Newtonsoft.Json for serialization (not System.Text.Json)
- All API interactions require an OpenRouter API key
- The ChatSession class appears to be a placeholder and is not yet implemented
- No unit tests currently exist in the repository