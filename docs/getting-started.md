# Getting Started with Codezerg.OpenRouter

This guide will help you set up and start using the Codezerg.OpenRouter library in your .NET projects.

## Prerequisites

- **.NET Standard 2.0 compatible runtime**:
  - .NET Framework 4.6.1 or later
  - .NET Core 2.0 or later
  - .NET 5.0 or later
- **OpenRouter API Key**: Sign up at [OpenRouter.ai](https://openrouter.ai) to get your API key
- **NuGet Package Manager**: For installing the library

## Installation

### Via Package Manager Console

```powershell
Install-Package Codezerg.OpenRouter
```

### Via .NET CLI

```bash
dotnet add package Codezerg.OpenRouter
```

### Via PackageReference

Add to your `.csproj` file:

```xml
<PackageReference Include="Codezerg.OpenRouter" Version="1.0.0" />
```

## Setting Up Your API Key

### Option 1: Environment Variable (Recommended)

Set the `OPENROUTER_API_KEY` environment variable:

**Windows (PowerShell):**
```powershell
$env:OPENROUTER_API_KEY = "sk-or-v1-your-api-key-here"
```

**Windows (Command Prompt):**
```cmd
set OPENROUTER_API_KEY=sk-or-v1-your-api-key-here
```

**Linux/macOS:**
```bash
export OPENROUTER_API_KEY="sk-or-v1-your-api-key-here"
```

### Option 2: Configuration File

Store in `appsettings.json`:

```json
{
  "OpenRouter": {
    "ApiKey": "sk-or-v1-your-api-key-here"
  }
}
```

### Option 3: Direct Configuration

```csharp
var config = new OpenRouterClientOptions
{
    ApiKey = "sk-or-v1-your-api-key-here"
};
```

> **Security Note**: Never commit API keys to source control. Use environment variables or secure configuration management.

## Your First API Call

### Basic Example

```csharp
using System;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;

class Program
{
    static async Task Main(string[] args)
    {
        // Initialize the client
        var config = new OpenRouterClientOptions
        {
            ApiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY"),
            DefaultModel = "openai/gpt-3.5-turbo"
        };
        
        var client = new OpenRouterClient(config);
        
        // Create a chat request
        var request = new ChatRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.User("What is the capital of France?")
            }
        };
        
        // Send the request
        var response = await client.SendChatCompletionAsync(request);
        
        // Display the response
        Console.WriteLine(response.Choices[0].Message.FirstTextContent);
        // Output: The capital of France is Paris.
    }
}
```

## Step-by-Step Breakdown

### 1. Import Required Namespaces

```csharp
using Codezerg.OpenRouter;          // Main client
using Codezerg.OpenRouter.Models;    // Request/response models
```

### 2. Configure the Client

```csharp
var config = new OpenRouterClientOptions
{
    ApiKey = "your-api-key",
    DefaultModel = "openai/gpt-3.5-turbo",  // Optional: Set default model
    Timeout = TimeSpan.FromSeconds(30)      // Optional: Set timeout
};
```

### 3. Create the Client Instance

```csharp
var client = new OpenRouterClient(config);
```

### 4. Build Your Request

```csharp
var request = new ChatRequest
{
    Model = "openai/gpt-4",  // Override default model if needed
    Messages = new List<ChatMessage>
    {
        ChatMessage.System("You are a helpful assistant."),
        ChatMessage.User("Hello! How are you?")
    },
    Temperature = 0.7,  // Control randomness (0-2)
    MaxTokens = 150     // Limit response length
};
```

### 5. Send Request and Handle Response

```csharp
try
{
    var response = await client.SendChatCompletionAsync(request);
    
    // Access the response
    var message = response.Choices[0].Message;
    Console.WriteLine($"Assistant: {message.FirstTextContent}");
    
    // Check usage
    Console.WriteLine($"Tokens used: {response.Usage?.TotalTokens}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Common Patterns

### Conversation History

```csharp
var messages = new List<ChatMessage>
{
    ChatMessage.User("What's 2+2?"),
    ChatMessage.Assistant("2+2 equals 4."),
    ChatMessage.User("What about 3+3?")
};

var request = new ChatRequest { Messages = messages };
var response = await client.SendChatCompletionAsync(request);
```

### Using Different Models

```csharp
// For code generation
request.Model = "deepseek/deepseek-coder";

// For creative writing
request.Model = "anthropic/claude-3-sonnet";

// For vision tasks
request.Model = "google/gemini-2.0-flash-exp";
```

### Adjusting Parameters

```csharp
var request = new ChatRequest
{
    Messages = messages,
    Temperature = 0.2,      // More focused, deterministic
    MaxTokens = 500,        // Longer responses
    TopP = 0.9,            // Nucleus sampling
    FrequencyPenalty = 0.5, // Reduce repetition
    PresencePenalty = 0.5   // Encourage topic diversity
};
```

## Best Practices

### 1. Error Handling

Always wrap API calls in try-catch blocks:

```csharp
try
{
    var response = await client.SendChatCompletionAsync(request);
    // Process response
}
catch (HttpRequestException ex)
{
    // Network errors
    Console.WriteLine($"Network error: {ex.Message}");
}
catch (TaskCanceledException ex)
{
    // Timeout
    Console.WriteLine($"Request timed out: {ex.Message}");
}
catch (Exception ex)
{
    // Other errors
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

### 2. Reuse Client Instance

Create one client instance and reuse it:

```csharp
public class ChatService
{
    private readonly OpenRouterClient _client;
    
    public ChatService(OpenRouterClientOptions config)
    {
        _client = new OpenRouterClient(config);
    }
    
    public async Task<string> GetResponseAsync(string prompt)
    {
        // Use _client for all requests
    }
}
```

### 3. Async/Await Best Practices

Use `ConfigureAwait(false)` in library code:

```csharp
public async Task<string> ProcessAsync()
{
    var response = await client
        .SendChatCompletionAsync(request)
        .ConfigureAwait(false);
    
    return response.Choices[0].Message.FirstTextContent;
}
```

### 4. Dispose Pattern

Properly dispose of the client when done:

```csharp
using (var client = new OpenRouterClient(config))
{
    // Use client
} // Automatically disposed
```

Or manually:

```csharp
client.Dispose();
```

## Next Steps

Now that you have the basics:

1. **Explore Streaming**: See [Streaming Guide](streaming.md) for real-time responses
2. **Try Multimodal**: Check [Multimodal Guide](multimodal.md) for image/audio support
3. **Advanced Configuration**: Read [Configuration Guide](configuration.md)
4. **View Examples**: Browse [Code Examples](examples.md) for more scenarios
5. **API Reference**: Consult [API Reference](api-reference.md) for detailed documentation

## Troubleshooting

### API Key Issues

**Error**: "Invalid API key"
- Verify your API key is correct
- Check it starts with `sk-or-v1-`
- Ensure no extra spaces or quotes

### Model Not Available

**Error**: "Model not found"
- Check [available models](https://openrouter.ai/models)
- Verify model name format (e.g., "provider/model-name")
- Some models require specific permissions

### Rate Limiting

**Error**: "Rate limit exceeded"
- Implement exponential backoff
- Check your usage at [OpenRouter Dashboard](https://openrouter.ai/dashboard)
- Consider upgrading your plan

### Timeout Issues

Increase timeout in configuration:
```csharp
var config = new OpenRouterClientOptions
{
    Timeout = TimeSpan.FromMinutes(2)
};
```

## Support

- **Documentation**: [Full Documentation](overview.md)
- **Examples**: [GitHub Examples](https://github.com/codezerg/Codezerg.OpenRouter/tree/main/examples)
- **Issues**: [GitHub Issues](https://github.com/codezerg/Codezerg.OpenRouter/issues)
- **OpenRouter Support**: [OpenRouter Docs](https://openrouter.ai/docs)