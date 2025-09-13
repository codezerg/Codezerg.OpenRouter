# Code Examples

This guide provides practical code examples for common use cases with Codezerg.OpenRouter.

## Table of Contents

- [Basic Examples](#basic-examples)
- [Conversation Management](#conversation-management)
- [Streaming Examples](#streaming-examples)
- [Multimodal Examples](#multimodal-examples)
- [Advanced Patterns](#advanced-patterns)
- [Real-World Applications](#real-world-applications)

## Basic Examples

### Simple Question-Answer

```csharp
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;

var config = new OpenRouterClientOptions
{
    ApiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY")
};

var client = new OpenRouterClient(config);

var request = new ChatRequest
{
    Model = "openai/gpt-3.5-turbo",
    Messages = new List<ChatMessage>
    {
        ChatMessage.User("What is the capital of Japan?")
    }
};

var response = await client.SendChatCompletionAsync(request);
Console.WriteLine(response.Choices[0].Message.FirstTextContent);
// Output: The capital of Japan is Tokyo.
```

### With System Message

```csharp
var request = new ChatRequest
{
    Model = "anthropic/claude-3-sonnet",
    Messages = new List<ChatMessage>
    {
        ChatMessage.System("You are a helpful geography teacher. Provide detailed, educational answers."),
        ChatMessage.User("What is the capital of Japan and what makes it significant?")
    },
    Temperature = 0.7,
    MaxTokens = 500
};

var response = await client.SendChatCompletionAsync(request);
Console.WriteLine(response.Choices[0].Message.FirstTextContent);
```

### JSON Response Format

```csharp
var request = new ChatRequest
{
    Model = "openai/gpt-4",
    Messages = new List<ChatMessage>
    {
        ChatMessage.System("Always respond with valid JSON."),
        ChatMessage.User("List 3 programming languages with their key features.")
    },
    ResponseFormat = new { type = "json_object" }
};

var response = await client.SendChatCompletionAsync(request);
var json = response.Choices[0].Message.FirstTextContent;

// Parse JSON response
dynamic languages = JsonConvert.DeserializeObject(json);
foreach (var lang in languages.languages)
{
    Console.WriteLine($"{lang.name}: {lang.features}");
}
```

## Conversation Management

### Maintaining Context

```csharp
public class ConversationManager
{
    private readonly OpenRouterClient _client;
    private readonly List<ChatMessage> _messages;
    private readonly int _maxMessages;
    
    public ConversationManager(OpenRouterClient client, int maxMessages = 10)
    {
        _client = client;
        _messages = new List<ChatMessage>();
        _maxMessages = maxMessages;
    }
    
    public async Task<string> SendMessageAsync(string userMessage)
    {
        // Add user message
        _messages.Add(ChatMessage.User(userMessage));
        
        // Trim old messages if needed
        while (_messages.Count > _maxMessages)
        {
            _messages.RemoveAt(0);
        }
        
        // Send request
        var request = new ChatRequest
        {
            Model = "openai/gpt-3.5-turbo",
            Messages = new List<ChatMessage>(_messages)
        };
        
        var response = await _client.SendChatCompletionAsync(request);
        var assistantMessage = response.Choices[0].Message;
        
        // Add assistant response to context
        _messages.Add(assistantMessage);
        
        return assistantMessage.FirstTextContent;
    }
    
    public void ClearContext()
    {
        _messages.Clear();
    }
}

// Usage
var conversation = new ConversationManager(client);

var response1 = await conversation.SendMessageAsync("My name is Alice.");
Console.WriteLine(response1);

var response2 = await conversation.SendMessageAsync("What's my name?");
Console.WriteLine(response2); // Will remember "Alice"
```

### Multi-Turn Dialogue

```csharp
public class DialogueExample
{
    public static async Task RunDialogueAsync(OpenRouterClient client)
    {
        var messages = new List<ChatMessage>
        {
            ChatMessage.System("You are a helpful coding assistant."),
            ChatMessage.User("I want to learn Python. Where should I start?"),
            ChatMessage.Assistant("Great choice! Here's where to start with Python:\n\n1. Install Python from python.org\n2. Learn basic syntax (variables, loops, functions)\n3. Practice with simple projects\n\nWould you like specific resource recommendations?"),
            ChatMessage.User("Yes, what are the best free resources?")
        };
        
        var request = new ChatRequest
        {
            Model = "openai/gpt-3.5-turbo",
            Messages = messages
        };
        
        var response = await client.SendChatCompletionAsync(request);
        Console.WriteLine(response.Choices[0].Message.FirstTextContent);
    }
}
```

## Streaming Examples

### Basic Streaming

```csharp
public async Task StreamResponseAsync(string prompt)
{
    var request = new ChatRequest
    {
        Model = "openai/gpt-3.5-turbo",
        Messages = new List<ChatMessage> { ChatMessage.User(prompt) },
        Stream = true
    };
    
    await foreach (var chunk in client.StreamChatCompletionAsync(request))
    {
        if (chunk.Choices?.Count > 0 && chunk.Choices[0].Delta?.Content != null)
        {
            Console.Write(chunk.Choices[0].Delta.Content);
        }
    }
}
```

### Streaming with Progress

```csharp
public class StreamingWithProgress
{
    public async Task StreamWithProgressAsync(string prompt)
    {
        var request = new ChatRequest
        {
            Model = "anthropic/claude-3-haiku",
            Messages = new List<ChatMessage> { ChatMessage.User(prompt) },
            Stream = true,
            MaxTokens = 1000
        };
        
        var progress = new Progress<StreamProgress>(p =>
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write($"Tokens: {p.TokenCount} | Speed: {p.TokensPerSecond:F1} t/s");
        });
        
        var startTime = DateTime.UtcNow;
        var tokenCount = 0;
        var content = new StringBuilder();
        
        await foreach (var chunk in client.StreamChatCompletionAsync(request))
        {
            if (chunk.Choices?.Count > 0 && chunk.Choices[0].Delta?.Content != null)
            {
                var text = chunk.Choices[0].Delta.Content;
                content.Append(text);
                tokenCount++;
                
                var elapsed = (DateTime.UtcNow - startTime).TotalSeconds;
                var tokensPerSecond = tokenCount / elapsed;
                
                ((IProgress<StreamProgress>)progress).Report(new StreamProgress
                {
                    TokenCount = tokenCount,
                    TokensPerSecond = tokensPerSecond
                });
            }
        }
        
        Console.WriteLine($"\n\nFinal content ({content.Length} chars):");
        Console.WriteLine(content.ToString());
    }
}

public class StreamProgress
{
    public int TokenCount { get; set; }
    public double TokensPerSecond { get; set; }
}
```

### Cancellable Streaming

```csharp
public class CancellableStreaming
{
    public async Task StreamWithCancellationAsync(string prompt)
    {
        using var cts = new CancellationTokenSource();
        
        // Start background task for user input
        var cancelTask = Task.Run(() =>
        {
            Console.WriteLine("Press 'Q' to stop streaming...\n");
            while (Console.ReadKey(true).Key != ConsoleKey.Q) { }
            cts.Cancel();
        });
        
        var request = new ChatRequest
        {
            Model = "openai/gpt-3.5-turbo",
            Messages = new List<ChatMessage> { ChatMessage.User(prompt) },
            Stream = true
        };
        
        try
        {
            await foreach (var chunk in client.StreamChatCompletionAsync(request, cts.Token))
            {
                if (chunk.Choices?[0].Delta?.Content != null)
                {
                    Console.Write(chunk.Choices[0].Delta.Content);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\n\n[Stream cancelled by user]");
        }
    }
}
```

## Multimodal Examples

### Image Analysis

```csharp
public class ImageAnalyzer
{
    private readonly OpenRouterClient _client;
    
    public ImageAnalyzer(OpenRouterClient client)
    {
        _client = client;
    }
    
    public async Task<string> AnalyzeImageAsync(string imagePath)
    {
        var imageBytes = File.ReadAllBytes(imagePath);
        
        var message = new ChatMessage(ChatRole.User)
            .AddText("Please analyze this image and describe:")
            .AddText("1. What you see in the image")
            .AddText("2. The main subjects or objects")
            .AddText("3. Any text visible in the image")
            .AddText("4. The overall mood or atmosphere")
            .AddImage(imageBytes, "image/jpeg");
        
        var request = new ChatRequest
        {
            Model = "google/gemini-2.0-flash-exp",
            Messages = new List<ChatMessage> { message }
        };
        
        var response = await _client.SendChatCompletionAsync(request);
        return response.Choices[0].Message.FirstTextContent;
    }
}
```

### Multiple Images Comparison

```csharp
public async Task CompareImagesAsync(string image1Path, string image2Path)
{
    var image1 = File.ReadAllBytes(image1Path);
    var image2 = File.ReadAllBytes(image2Path);
    
    var message = new ChatMessage(ChatRole.User)
        .AddText("Compare these two images:")
        .AddText("\nImage 1:")
        .AddImage(image1, "image/jpeg")
        .AddText("\nImage 2:")
        .AddImage(image2, "image/jpeg")
        .AddText("\nWhat are the similarities and differences?");
    
    var request = new ChatRequest
    {
        Model = "anthropic/claude-3-opus",
        Messages = new List<ChatMessage> { message },
        MaxTokens = 1000
    };
    
    var response = await client.SendChatCompletionAsync(request);
    Console.WriteLine(response.Choices[0].Message.FirstTextContent);
}
```

### OCR and Text Extraction

```csharp
public class OCRProcessor
{
    public async Task<ExtractedText> ExtractTextFromImageAsync(string imageUrl)
    {
        var message = new ChatMessage(ChatRole.User)
            .AddText("Extract all text from this image. Format the response as JSON with 'title', 'body', and 'metadata' fields.")
            .AddImage(imageUrl);
        
        var request = new ChatRequest
        {
            Model = "openai/gpt-4o",
            Messages = new List<ChatMessage> { message },
            ResponseFormat = new { type = "json_object" },
            Temperature = 0.1  // Low temperature for accuracy
        };
        
        var response = await client.SendChatCompletionAsync(request);
        var json = response.Choices[0].Message.FirstTextContent;
        
        return JsonConvert.DeserializeObject<ExtractedText>(json);
    }
}

public class ExtractedText
{
    public string Title { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
}
```

## Advanced Patterns

### Function Calling

```csharp
public class FunctionCallingExample
{
    public async Task UseFunctionCallingAsync()
    {
        var tools = new List<ToolDefinition>
        {
            new ToolDefinition
            {
                Type = "function",
                Function = new
                {
                    name = "get_weather",
                    description = "Get the current weather for a location",
                    parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            location = new
                            {
                                type = "string",
                                description = "City and state, e.g., San Francisco, CA"
                            },
                            unit = new
                            {
                                type = "string",
                                @enum = new[] { "celsius", "fahrenheit" }
                            }
                        },
                        required = new[] { "location" }
                    }
                }
            }
        };
        
        var request = new ChatRequest
        {
            Model = "openai/gpt-4",
            Messages = new List<ChatMessage>
            {
                ChatMessage.User("What's the weather in New York?")
            },
            Tools = tools,
            ToolChoice = "auto"
        };
        
        var response = await client.SendChatCompletionAsync(request);
        
        // Check if function was called
        if (response.Choices[0].Message.ToolCalls?.Any() == true)
        {
            var toolCall = response.Choices[0].Message.ToolCalls[0];
            Console.WriteLine($"Function called: {toolCall}");
            
            // Simulate function execution
            var weatherResult = GetWeather("New York, NY", "fahrenheit");
            
            // Send function result back
            var followUp = new ChatRequest
            {
                Model = "openai/gpt-4",
                Messages = new List<ChatMessage>
                {
                    ChatMessage.User("What's the weather in New York?"),
                    response.Choices[0].Message,
                    ChatMessage.Tool(weatherResult, toolCall.Id)
                }
            };
            
            var finalResponse = await client.SendChatCompletionAsync(followUp);
            Console.WriteLine(finalResponse.Choices[0].Message.FirstTextContent);
        }
    }
    
    private string GetWeather(string location, string unit)
    {
        // Simulate weather API call
        return JsonConvert.SerializeObject(new
        {
            location,
            temperature = 72,
            unit,
            conditions = "Partly cloudy"
        });
    }
}
```

### Parallel Processing

```csharp
public class ParallelProcessor
{
    public async Task<Dictionary<string, string>> ProcessMultiplePromptsAsync(
        List<string> prompts)
    {
        var tasks = prompts.Select(async prompt =>
        {
            var request = new ChatRequest
            {
                Model = "openai/gpt-3.5-turbo",
                Messages = new List<ChatMessage> { ChatMessage.User(prompt) },
                MaxTokens = 100
            };
            
            var response = await client.SendChatCompletionAsync(request);
            return (prompt, response.Choices[0].Message.FirstTextContent);
        });
        
        var results = await Task.WhenAll(tasks);
        return results.ToDictionary(r => r.prompt, r => r.Item2);
    }
}
```

### Retry with Exponential Backoff

```csharp
public class RetryExample
{
    public async Task<ChatResponse> SendWithRetryAsync(ChatRequest request)
    {
        const int maxRetries = 3;
        var delay = TimeSpan.FromSeconds(1);
        
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                return await client.SendChatCompletionAsync(request);
            }
            catch (HttpRequestException ex) when (i < maxRetries - 1)
            {
                Console.WriteLine($"Attempt {i + 1} failed: {ex.Message}");
                await Task.Delay(delay);
                delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2);
            }
        }
        
        throw new Exception("Max retries exceeded");
    }
}
```

## Real-World Applications

### Code Generator

```csharp
public class CodeGenerator
{
    private readonly OpenRouterClient _client;
    
    public async Task<string> GenerateCodeAsync(
        string language, 
        string description)
    {
        var prompt = $@"Generate {language} code for the following requirement:
{description}

Requirements:
- Include error handling
- Add comments explaining the logic
- Follow best practices for {language}
- Make it production-ready";
        
        var request = new ChatRequest
        {
            Model = "deepseek/deepseek-coder-33b-instruct",
            Messages = new List<ChatMessage>
            {
                ChatMessage.System($"You are an expert {language} developer."),
                ChatMessage.User(prompt)
            },
            Temperature = 0.2,  // Lower temperature for code generation
            MaxTokens = 2000
        };
        
        var response = await _client.SendChatCompletionAsync(request);
        return response.Choices[0].Message.FirstTextContent;
    }
}

// Usage
var generator = new CodeGenerator(client);
var code = await generator.GenerateCodeAsync(
    "Python", 
    "A function to validate email addresses using regex"
);
Console.WriteLine(code);
```

### Content Summarizer

```csharp
public class ContentSummarizer
{
    public async Task<Summary> SummarizeAsync(string content, int maxWords = 100)
    {
        var request = new ChatRequest
        {
            Model = "anthropic/claude-3-haiku",
            Messages = new List<ChatMessage>
            {
                ChatMessage.System("You are a professional content summarizer."),
                ChatMessage.User($@"Summarize the following content in approximately {maxWords} words:

{content}

Provide:
1. A brief summary
2. Key points (as bullet points)
3. Main conclusion")
            },
            Temperature = 0.3
        };
        
        var response = await client.SendChatCompletionAsync(request);
        var result = response.Choices[0].Message.FirstTextContent;
        
        // Parse response into structured format
        return ParseSummary(result);
    }
    
    private Summary ParseSummary(string text)
    {
        // Implementation to parse the response
        return new Summary
        {
            Brief = ExtractSection(text, "summary"),
            KeyPoints = ExtractBulletPoints(text),
            Conclusion = ExtractSection(text, "conclusion")
        };
    }
}

public class Summary
{
    public string Brief { get; set; }
    public List<string> KeyPoints { get; set; }
    public string Conclusion { get; set; }
}
```

### Language Translator

```csharp
public class LanguageTranslator
{
    public async Task<TranslationResult> TranslateAsync(
        string text, 
        string targetLanguage,
        bool preserveFormatting = true)
    {
        var systemPrompt = preserveFormatting
            ? "You are a professional translator. Preserve all formatting, line breaks, and structure."
            : "You are a professional translator.";
        
        var request = new ChatRequest
        {
            Model = "openai/gpt-3.5-turbo",
            Messages = new List<ChatMessage>
            {
                ChatMessage.System(systemPrompt),
                ChatMessage.User($"Translate the following text to {targetLanguage}:\n\n{text}")
            },
            Temperature = 0.3  // Lower temperature for accuracy
        };
        
        var response = await client.SendChatCompletionAsync(request);
        
        return new TranslationResult
        {
            OriginalText = text,
            TranslatedText = response.Choices[0].Message.FirstTextContent,
            TargetLanguage = targetLanguage,
            TokensUsed = response.Usage?.TotalTokens ?? 0
        };
    }
}

public class TranslationResult
{
    public string OriginalText { get; set; }
    public string TranslatedText { get; set; }
    public string TargetLanguage { get; set; }
    public int TokensUsed { get; set; }
}
```

### Chatbot with Personality

```csharp
public class PersonalityChatbot
{
    private readonly OpenRouterClient _client;
    private readonly string _personality;
    private readonly List<ChatMessage> _context;
    
    public PersonalityChatbot(OpenRouterClient client, string personality)
    {
        _client = client;
        _personality = personality;
        _context = new List<ChatMessage>
        {
            ChatMessage.System($"You are {personality}. Stay in character and respond accordingly.")
        };
    }
    
    public async Task<string> ChatAsync(string message)
    {
        _context.Add(ChatMessage.User(message));
        
        var request = new ChatRequest
        {
            Model = "anthropic/claude-3-sonnet",
            Messages = new List<ChatMessage>(_context),
            Temperature = 0.8  // Higher for more personality
        };
        
        var response = await _client.SendChatCompletionAsync(request);
        var reply = response.Choices[0].Message;
        
        _context.Add(reply);
        
        // Keep context size manageable
        if (_context.Count > 20)
        {
            _context.RemoveRange(1, 2); // Remove old messages, keep system
        }
        
        return reply.FirstTextContent;
    }
}

// Usage
var pirateChatbot = new PersonalityChatbot(client, 
    "a friendly pirate who loves to share sailing stories");
    
var response = await pirateChatbot.ChatAsync("Hello there!");
Console.WriteLine(response);
// Output: "Ahoy there, matey! Welcome aboard! 'Tis a fine day for sailin'..."
```

## Testing Examples

### Unit Test Example

```csharp
[TestClass]
public class OpenRouterClientTests
{
    private OpenRouterClient _client;
    
    [TestInitialize]
    public void Setup()
    {
        var config = new OpenRouterClientOptions
        {
            ApiKey = Environment.GetEnvironmentVariable("TEST_API_KEY"),
            DefaultModel = "openai/gpt-3.5-turbo"
        };
        
        _client = new OpenRouterClient(config);
    }
    
    [TestMethod]
    public async Task SendChatCompletion_WithValidRequest_ReturnsResponse()
    {
        // Arrange
        var request = new ChatRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.User("Say 'Hello Test'")
            }
        };
        
        // Act
        var response = await _client.SendChatCompletionAsync(request);
        
        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Choices);
        Assert.IsTrue(response.Choices.Count > 0);
        Assert.IsNotNull(response.Choices[0].Message);
        StringAssert.Contains(response.Choices[0].Message.FirstTextContent, "Hello Test");
    }
}
```

## Further Reading

- [API Reference](api-reference.md)
- [Streaming Guide](streaming.md)
- [Multimodal Guide](multimodal.md)
- [Error Handling](error-handling.md)
- [Configuration](configuration.md)