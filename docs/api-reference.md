# API Reference

Complete API documentation for the Codezerg.OpenRouter library.

## Table of Contents

- [OpenRouterClient](#openrouterclient)
- [OpenRouterClientOptions](#openrouterclientoptions)
- [ChatRequest](#chatrequest)
- [ChatResponse](#chatresponse)
- [ChatMessage](#chatmessage)
- [MessagePart](#messagepart)
- [Enumerations](#enumerations)
- [Error Handling](#error-handling)

---

## OpenRouterClient

The main client class for interacting with the OpenRouter API.

### Constructor

```csharp
public OpenRouterClient(OpenRouterClientOptions config)
public OpenRouterClient(OpenRouterClientOptions config, HttpClient httpClient)
```

**Parameters:**
- `config`: Configuration options for the client (required)
- `httpClient`: Optional HTTP client instance for custom networking

**Example:**
```csharp
var client = new OpenRouterClient(config);
// Or with custom HttpClient
var httpClient = new HttpClient();
var client = new OpenRouterClient(config, httpClient);
```

### Methods

#### SendChatCompletionAsync

Sends a chat completion request and returns the complete response.

```csharp
public async Task<ChatResponse> SendChatCompletionAsync(
    ChatRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request`: The chat request to send
- `cancellationToken`: Optional cancellation token

**Returns:** `Task<ChatResponse>` - The complete chat response

**Example:**
```csharp
var response = await client.SendChatCompletionAsync(request);
```

#### StreamChatCompletionAsync

Streams a chat completion response as an async enumerable.

```csharp
public async IAsyncEnumerable<ChatResponse> StreamChatCompletionAsync(
    ChatRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request`: The chat request to send (must have `Stream = true`)
- `cancellationToken`: Optional cancellation token

**Returns:** `IAsyncEnumerable<ChatResponse>` - Stream of response chunks

**Example:**
```csharp
await foreach (var chunk in client.StreamChatCompletionAsync(request))
{
    Console.Write(chunk.Choices[0].Delta?.Content);
}
```

#### Dispose

Releases resources used by the client.

```csharp
public void Dispose()
```

---

## OpenRouterClientOptions

Configuration options for the OpenRouter client.

### Properties

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `ApiKey` | `string` | OpenRouter API key (required) | - |
| `BaseUrl` | `string` | Base URL for the API | `"https://openrouter.ai/api/v1"` |
| `DefaultModel` | `string` | Default model to use | `null` |
| `Timeout` | `TimeSpan` | Request timeout | `120 seconds` |
| `DefaultHeaders` | `Dictionary<string, string>` | Additional headers | `null` |
| `MaxRetries` | `int` | Maximum retry attempts | `3` |
| `RetryDelay` | `TimeSpan` | Delay between retries | `1 second` |

### Methods

#### Validate

Validates the configuration options.

```csharp
public void Validate()
```

**Throws:** `InvalidOperationException` if configuration is invalid

#### Clone

Creates a deep copy of the configuration.

```csharp
public OpenRouterClientOptions Clone()
```

**Example:**
```csharp
var config = new OpenRouterClientOptions
{
    ApiKey = "sk-or-v1-...",
    DefaultModel = "openai/gpt-4",
    Timeout = TimeSpan.FromMinutes(2),
    DefaultHeaders = new Dictionary<string, string>
    {
        ["HTTP-Referer"] = "https://myapp.com",
        ["X-Title"] = "My Application"
    }
};
```

---

## ChatRequest

Represents a chat completion request.

### Properties

| Property | Type | Description | Required |
|----------|------|-------------|----------|
| `Model` | `string` | Model identifier | No* |
| `Messages` | `List<ChatMessage>` | Conversation messages | Yes |
| `Stream` | `bool` | Enable streaming | No |
| `Temperature` | `double?` | Sampling temperature (0-2) | No |
| `TopP` | `double?` | Nucleus sampling parameter | No |
| `TopK` | `int?` | Top-K sampling parameter | No |
| `FrequencyPenalty` | `double?` | Frequency penalty (-2 to 2) | No |
| `PresencePenalty` | `double?` | Presence penalty (-2 to 2) | No |
| `RepetitionPenalty` | `double?` | Repetition penalty | No |
| `MaxTokens` | `int?` | Maximum tokens to generate | No |
| `Stop` | `List<string>` | Stop sequences | No |
| `Seed` | `int?` | Random seed for reproducibility | No |
| `ResponseFormat` | `object` | Response format specification | No |
| `Tools` | `List<ToolDefinition>` | Available tools/functions | No |
| `ToolChoice` | `object` | Tool selection strategy | No |
| `LogProbs` | `bool?` | Include log probabilities | No |
| `TopLogProbs` | `int?` | Number of top log probs | No |
| `User` | `string` | User identifier | No |
| `ProviderOptions` | `ProviderOptions` | Provider-specific options | No |

*Model is required unless DefaultModel is set in configuration

**Example:**
```csharp
var request = new ChatRequest
{
    Model = "anthropic/claude-3-sonnet",
    Messages = new List<ChatMessage>
    {
        ChatMessage.System("You are a helpful assistant"),
        ChatMessage.User("Hello!")
    },
    Temperature = 0.7,
    MaxTokens = 1000,
    Stream = true
};
```

---

## ChatResponse

Represents a chat completion response.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `string` | Unique response identifier |
| `Object` | `string` | Object type (e.g., "chat.completion") |
| `Created` | `long` | Unix timestamp of creation |
| `Model` | `string` | Model used for generation |
| `Choices` | `List<Choice>` | Response choices |
| `Usage` | `Usage` | Token usage information |
| `SystemFingerprint` | `string` | System fingerprint |

### Nested Types

#### Choice

| Property | Type | Description |
|----------|------|-------------|
| `Index` | `int` | Choice index |
| `Message` | `ChatMessage` | Complete message (non-streaming) |
| `Delta` | `ChatMessage` | Message delta (streaming) |
| `FinishReason` | `string` | Completion reason |
| `LogProbs` | `object` | Log probability information |

#### Usage

| Property | Type | Description |
|----------|------|-------------|
| `PromptTokens` | `int` | Tokens in prompt |
| `CompletionTokens` | `int` | Tokens in completion |
| `TotalTokens` | `int` | Total tokens used |

**Example:**
```csharp
var response = await client.SendChatCompletionAsync(request);

Console.WriteLine($"Response ID: {response.Id}");
Console.WriteLine($"Model: {response.Model}");
Console.WriteLine($"Content: {response.Choices[0].Message.FirstTextContent}");
Console.WriteLine($"Tokens: {response.Usage?.TotalTokens}");
```

---

## ChatMessage

Represents a message in a conversation.

### Constructor

```csharp
public ChatMessage()
public ChatMessage(ChatRole role)
public ChatMessage(string role)
```

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Role` | `ChatRole?` | Message role |
| `Content` | `object` | Message content (string or List<MessagePart>) |
| `Name` | `string` | Optional sender name |
| `ToolCalls` | `List<object>` | Tool/function calls |
| `ToolCallId` | `string` | Tool call identifier |

### Methods

#### Static Factory Methods

```csharp
public static ChatMessage System(string content)
public static ChatMessage User(string content)
public static ChatMessage Assistant(string content)
public static ChatMessage Tool(string content, string toolCallId)
```

#### Content Builders

```csharp
public ChatMessage AddText(string text)
public ChatMessage AddImage(string imageUrl)
public ChatMessage AddImage(byte[] imageData, string mimeType = "image/jpeg")
public ChatMessage AddAudio(string audioUrl)
public ChatMessage AddAudio(byte[] audioData, string mimeType = "audio/wav")
```

#### Content Accessors

```csharp
public string FirstTextContent { get; }
public List<MessagePart> Parts { get; }
public bool IsMultimodal { get; }
```

**Example:**
```csharp
// Simple text message
var textMessage = ChatMessage.User("Hello!");

// Multimodal message
var multiMessage = new ChatMessage(ChatRole.User)
    .AddText("What's in this image?")
    .AddImage("https://example.com/image.jpg")
    .AddText("Please describe it in detail.");
```

---

## MessagePart

Represents a part of a multimodal message.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Type` | `MessageContentType` | Content type |
| `Text` | `string` | Text content |
| `ImageUrl` | `ImageUrl` | Image URL details |
| `AudioUrl` | `AudioUrl` | Audio URL details |

### Nested Types

#### ImageUrl

| Property | Type | Description |
|----------|------|-------------|
| `Url` | `string` | Image URL or data URI |
| `Detail` | `string` | Detail level ("low", "high", "auto") |

#### AudioUrl

| Property | Type | Description |
|----------|------|-------------|
| `Url` | `string` | Audio URL or data URI |

---

## Enumerations

### ChatRole

Message sender roles.

```csharp
public enum ChatRole
{
    System,    // System message
    User,      // User message
    Assistant, // Assistant response
    Tool       // Tool/function result
}
```

### MessageContentType

Content types for message parts.

```csharp
public enum MessageContentType
{
    Text,      // Text content
    ImageUrl,  // Image URL or data
    AudioUrl   // Audio URL or data
}
```

### CompletionFinishReason

Reasons for completion termination.

```csharp
public static class CompletionFinishReason
{
    public const string Stop = "stop";              // Natural completion
    public const string Length = "length";          // Max tokens reached
    public const string ToolCalls = "tool_calls";   // Tool call required
    public const string ContentFilter = "content_filter"; // Filtered
    public const string Error = "error";            // Error occurred
}
```

---

## Error Handling

### ApiError

Represents an API error response.

```csharp
public class ApiError
{
    public ErrorDetail Error { get; set; }
}

public class ErrorDetail
{
    public int Code { get; set; }
    public string Message { get; set; }
    public object Metadata { get; set; }
}
```

### Exception Handling

The client may throw the following exceptions:

| Exception | Cause |
|-----------|-------|
| `ArgumentNullException` | Missing required parameters |
| `InvalidOperationException` | Invalid configuration or state |
| `HttpRequestException` | Network or API errors |
| `TaskCanceledException` | Request timeout or cancellation |
| `JsonException` | Response parsing errors |

**Example:**
```csharp
try
{
    var response = await client.SendChatCompletionAsync(request);
}
catch (HttpRequestException ex)
{
    // Handle API or network errors
    if (ex.StatusCode == HttpStatusCode.TooManyRequests)
    {
        // Rate limited - implement backoff
    }
}
catch (TaskCanceledException)
{
    // Request timed out
}
catch (Exception ex)
{
    // Other errors
}
```

---

## Extension Methods

### OpenRouterClientOptionsExtensions

Provides extension methods for configuration.

```csharp
public static class OpenRouterClientOptionsExtensions
{
    // Validates and returns the configuration
    public static OpenRouterClientOptions Validated(this OpenRouterClientOptions options)
    
    // Sets default headers
    public static OpenRouterClientOptions WithHeaders(
        this OpenRouterClientOptions options,
        Dictionary<string, string> headers)
    
    // Sets timeout
    public static OpenRouterClientOptions WithTimeout(
        this OpenRouterClientOptions options,
        TimeSpan timeout)
    
    // Sets default model
    public static OpenRouterClientOptions WithModel(
        this OpenRouterClientOptions options,
        string model)
}
```

**Example:**
```csharp
var config = new OpenRouterClientOptions { ApiKey = apiKey }
    .WithModel("openai/gpt-4")
    .WithTimeout(TimeSpan.FromMinutes(2))
    .WithHeaders(new Dictionary<string, string>
    {
        ["X-Title"] = "My App"
    })
    .Validated();
```

---

## Thread Safety

- `OpenRouterClient`: Thread-safe for all public methods
- `OpenRouterClientOptions`: Not thread-safe; create separate instances
- `ChatRequest`/`ChatResponse`: Not thread-safe; use per-request

---

## Performance Considerations

### Connection Pooling

The client reuses HTTP connections for better performance. When using a custom `HttpClient`:

```csharp
var httpClient = new HttpClient(new SocketsHttpHandler
{
    PooledConnectionLifetime = TimeSpan.FromMinutes(15),
    MaxConnectionsPerServer = 10
});
```

### Streaming Performance

For optimal streaming performance:

```csharp
await foreach (var chunk in client.StreamChatCompletionAsync(request)
    .ConfigureAwait(false))
{
    // Process chunk immediately
    // Avoid blocking operations
}
```

### Memory Usage

- Streaming responses use minimal memory
- Large conversations: Consider message pruning
- Multimodal content: Be aware of base64 encoding overhead