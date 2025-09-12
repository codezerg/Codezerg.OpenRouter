# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Codezerg.OpenRouter is a .NET client library for seamless integration with OpenRouter's unified LLM API. It provides a normalized interface to interact with multiple AI model providers through OpenRouter's gateway.

## Build and Development Commands

### Building the Solution
```bash
# Build entire solution
dotnet build

# Build in Release mode
dotnet build -c Release

# Build specific project
dotnet build src/Codezerg.OpenRouter/Codezerg.OpenRouter.csproj
```

### Running Examples
```bash
# Set API key environment variable first
# Windows (User level):
setx OPENROUTER_API_KEY "your-api-key-here"

# Windows (System level):
setx OPENROUTER_API_KEY "your-api-key-here" /M

# Linux/Mac:
export OPENROUTER_API_KEY=your-api-key-here

# Run examples
dotnet run --project examples/Codezerg.OpenRouter.Examples.csproj
```

### Running Demo Applications
```bash
# Image Analyzer demo
dotnet run --project demo/ImageAnalyzer/ImageAnalyzer.csproj

# Streaming Response demo
dotnet run --project demo/StreamingResponse/StreamingResponse.csproj
```

### Package Management
```bash
# Create NuGet package
dotnet pack src/Codezerg.OpenRouter/Codezerg.OpenRouter.csproj -c Release

# Restore packages
dotnet restore
```

## Architecture and Key Components

### Core Client Architecture
The library follows a client-service pattern with these key components:

1. **OpenRouterClient** (`src/Codezerg.OpenRouter/OpenRouterClient.cs`): Main entry point for API interactions. Manages HTTP communication, handles authentication, and provides methods for synchronous and streaming chat completions.

2. **OpenRouterConfig** (`src/Codezerg.OpenRouter/OpenRouterConfig.cs`): Configuration object containing API key, base URL, timeout settings, and optional headers (UserAgent, Referer). Includes validation and cloning for immutability.

3. **Models Namespace** (`src/Codezerg.OpenRouter/Models/`): Contains request/response DTOs and domain models:
   - `ChatCompletionRequest`: Request structure matching OpenRouter's API schema
   - `ChatCompletionResponse`: Response structure with streaming support
   - `ChatMessage`: Message representation with role and content
   - `ChatContentPart`: Supports multimodal content (text, images)
   - `Tool`: Tool/function calling support
   - `ModelConstants`: Predefined model identifiers for various providers

### Key Design Patterns

1. **Streaming Support**: The client supports both synchronous and streaming responses using `IAsyncEnumerable` for efficient real-time processing.

2. **Multimodal Content**: Messages can contain mixed content types (text, images) through the `ChatContentPart` abstraction.

3. **Provider Normalization**: The library normalizes different provider APIs through OpenRouter's unified interface, allowing model switching without code changes.

4. **Configuration Immutability**: Configuration objects are cloned to prevent runtime modifications after client initialization.

## Target Framework and Dependencies

- **Target Framework**: .NET Standard 2.0 (broad compatibility)
- **Language Version**: Latest C# with nullable reference types enabled
- **Key Dependencies**:
  - Newtonsoft.Json 13.0.3 (JSON serialization)
  - Microsoft.Bcl.AsyncInterfaces 8.0.0 (async enumerable support)

## API Integration Notes

- The library implements OpenRouter's chat completion API at `/api/v1/chat/completions`
- Authentication uses Bearer token in Authorization header
- Supports all OpenRouter parameters including temperature, max_tokens, streaming, tools, and response formats
- Error responses are properly deserialized into `ErrorResponse` objects

## Documentation Structure

The `/docs` directory contains comprehensive API documentation:
- `overview.md`: API reference and request/response schemas
- `parameters.md`: Detailed parameter documentation
- `streaming.md`: Streaming implementation guide
- `tool-calling.md`: Function/tool calling examples
- `multimodal-*.md`: Image handling and generation guides
- `prompt-caching.md`: Caching strategies
- `errors.md`: Error handling reference