# CLAUDE.md

This file provides guidance to **Claude Code** (`claude.ai/code`) when working with code in this repository.

---

## Project Overview

**Codezerg.OpenRouter** is a **.NET Standard 2.0 client library** for OpenRouter's unified LLM API.  
It includes:

- **Chat Completions** with multiple models (OpenAI, Anthropic, Google, Meta, etc.).  
- Strongly typed request/response models (`ChatRequest`, `ChatResponse`, etc.).  
- **Streaming API** with `IAsyncEnumerable<ChatResponse>`.  
- **Multimodal** messaging (text, images, audio).  
- **Frontend API client** for providers/models (`OpenRouterFrontendClient`).  
- **Account APIs**: usage activity, credits, owned models.  
- **Discovery APIs**: providers, models, endpoints, generation history.  
- Fully compatible with **.NET Framework 4.6.2+, .NET Core, .NET 5, 6, 7, 8**.

---

## Build and Development Commands

### Build
```bash
dotnet build
dotnet build -c Release
dotnet build src/Codezerg.OpenRouter/Codezerg.OpenRouter.csproj
```

### Run Examples
```bash
# PowerShell
$env:OPENROUTER_API_KEY="your-api-key"
# Linux/macOS
export OPENROUTER_API_KEY="your-api-key"

dotnet run --project examples/Codezerg.OpenRouter.Examples.csproj
```

### Tests
```bash
dotnet test
dotnet test --logger "console;verbosity=detailed"
```

### Pack NuGet
```bash
dotnet pack src/Codezerg.OpenRouter/Codezerg.OpenRouter.csproj -c Release
```

---

## Key Classes

### `OpenRouterClient`
High‑level API client for OpenRouter LLM and account APIs.  
- `SendChatCompletionAsync(ChatRequest)` → non‑streaming response.  
- `StreamChatCompletionAsync(ChatRequest)` → streaming SSE response.  
- `GetActivityAsync(date?)` → recent usage grouped by endpoint.  
- `GetCreditsAsync()` → total purchased vs used.  
- `GetProvidersAsync()` → list of API providers.  
- `GetModelsAsync(category?)` → all models.  
- `GetUserModelsAsync()` → user‑filtered models.  
- `GetModelEndpointsAsync(author, slug)` → all endpoints of model.  
- `GetGenerationAsync(id)` → past generation details.  

### `OpenRouterFrontendClient`
Shell around undocumented **frontend/private APIs**:
- Discover `Providers`, `Models`, `Reasoning` models, `Free` models.  
- Filter by provider, modality.  
- Privacy‑friendly and BYOK providers.  
⚠️ Considered experimental.

### `OpenRouterClientOptions` & Extensions
Holds config:
- `ApiKey`, `Endpoint`, `DefaultModel`  
- `Timeout`, `UserAgent`, `Referer`, `EnableDebugLogging`  
Extensions add `.WithApiKey(...)`, `.WithDefaultModel(...)`, `.Validate()`, `.Clone()`.

### Models Namespace
- **Core chat**: `ChatRequest`, `ChatResponse`, `ChatMessage`  
- **Chat parts**: `MessagePart`, `ImageReference`, `AudioContent`  
- **Usage/Accounting**: `TokenUsage`, `Activity`, `Credits`, `GenerationDetails`  
- **Discovery**: `ProviderInfo`, `ModelInfo`, `ModelEndpoints`  
- **Tooling**: `ToolDefinition`, `ToolCall`, `FunctionDescription`  
- **Structured output**: `ResponseFormatOptions`, `ResponseFormatType`  
- **Enums**: `ChatRole`, `CompletionFinishReason`, `Modality`, `VerbosityLevel`  

---

## Design Principles

1. **Strong typing** over dynamic JSON.  
2. **Builder pattern** for complex multimodal messages.  
3. **Async Streaming** with `IAsyncEnumerable`.  
   - ⚠️ Streaming data is in `ChatChoice.Delta.Content`.  
   - Non‑streaming data is in `ChatChoice.Message.FirstTextContent`.  
4. **Immutable Config**: clone options on construction.  
5. **Undocumented APIs** (`FrontendClient`) are loosely typed.

---

## Example Usage

### Standard Chat
```csharp
var req = new ChatRequest {
    Messages = new() {
        ChatMessage.System("You are a helpful assistant."),
        ChatMessage.User("What is the capital of France?")
    }
};
var resp = await client.SendChatCompletionAsync(req);
Console.WriteLine(resp.Choices[0].Message?.FirstTextContent);
```

### Streaming
```csharp
await foreach (var chunk in client.StreamChatCompletionAsync(req))
{
    var token = chunk.Choices?[0].Delta?.Content;
    if (!string.IsNullOrEmpty(token))
        Console.Write(token);
}
```

### Multimodal (Text + Image)
```csharp
var msg = new ChatMessage(ChatRole.User)
    .AddText("Describe this picture:")
    .AddImage("https://example.com/cat.jpg");

var resp = await client.SendChatCompletionAsync(new ChatRequest { Messages = new(){ msg } });
Console.WriteLine(resp.Choices[0].Message?.FirstTextContent);
```

### Account & Usage
```csharp
var credits = await client.GetCreditsAsync();
Console.WriteLine($"Credits: {credits.TotalCredits}, Used: {credits.TotalUsage}");

var activity = await client.GetActivityAsync();
foreach (var day in activity)
    Console.WriteLine($"{day.Date}: {day.Model} - {day.Requests} requests");
```

### Providers / Models
```csharp
var providers = await client.GetProvidersAsync();
foreach (var p in providers)
    Console.WriteLine($"{p.Name} ({p.PrivacyPolicyUrl})");

var models = await client.GetModelsAsync();
foreach (var m in models)
    Console.WriteLine($"{m.Name}  ({m.Id})");
```

### Generation Details
```csharp
var gen = await client.GetGenerationAsync("generation-id");
Console.WriteLine($"Model: {gen.Model}, Cost: {gen.TotalCost}, Tokens: {gen.TokensCompletion}");
```

---

## Development Guidelines

- Target `.NET Standard 2.0` for broad compatibility.  
- Use `async/await` with `.ConfigureAwait(false)` inside library code.  
- Add XML doc comments for all public types.  
- Ensure streaming logic discards `[DONE]` SSE marker.  
- Preserve model schema (`ChatMessage`, `MessagePart`) because OpenRouter uses varying structures (`string` vs `array`).  
- Treat `FrontendClient` as experimental (API shape may break).  

---

## Project Layout

```
/
├── src/Codezerg.OpenRouter/
│   ├── Models/                  # DTOs and enums for requests/responses
│   ├── OpenRouterClient.cs      # Core API client (chat, activity, credits, models, etc.)
│   ├── OpenRouterFrontendClient.cs # Private frontend discovery client
│   └── ...
├── examples/                    # runnable demos
├── README.md
├── CLAUDE.md
└── Codezerg.OpenRouter.sln
```

