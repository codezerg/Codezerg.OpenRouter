# Streaming Responses

This guide covers the streaming capabilities of Codezerg.OpenRouter, allowing you to receive and process LLM responses in real-time as they are generated.

## Overview

Streaming enables you to receive partial responses as they are generated, rather than waiting for the complete response. This provides:

- **Better user experience**: Show content as it arrives
- **Lower perceived latency**: First tokens arrive quickly
- **Memory efficiency**: Process data without buffering entire response
- **Cancellation support**: Stop generation mid-stream

## How Streaming Works

1. Client sends request with `Stream = true`
2. Server responds with Server-Sent Events (SSE)
3. Client parses SSE chunks as they arrive
4. Each chunk contains a delta (partial content)
5. Stream ends with a `[DONE]` marker

## Basic Streaming Example

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        ChatMessage.User("Write a short story about a robot.")
    },
    Stream = true  // Enable streaming
};

await foreach (var chunk in client.StreamChatCompletionAsync(request))
{
    if (chunk.Choices?.Count > 0)
    {
        var delta = chunk.Choices[0].Delta;
        if (delta?.Content != null)
        {
            Console.Write(delta.Content);
        }
    }
}
```

## Advanced Streaming Patterns

### Collecting Full Response

```csharp
var fullResponse = new StringBuilder();

await foreach (var chunk in client.StreamChatCompletionAsync(request))
{
    if (chunk.Choices?.Count > 0)
    {
        var content = chunk.Choices[0].Delta?.Content;
        if (content != null)
        {
            fullResponse.Append(content);
            Console.Write(content);  // Display as it arrives
        }
    }
}

string completeText = fullResponse.ToString();
```

### Handling Metadata

```csharp
string responseId = null;
string model = null;
string finishReason = null;

await foreach (var chunk in client.StreamChatCompletionAsync(request))
{
    // Capture metadata from first chunk
    if (responseId == null && chunk.Id != null)
    {
        responseId = chunk.Id;
        model = chunk.Model;
    }
    
    if (chunk.Choices?.Count > 0)
    {
        var choice = chunk.Choices[0];
        
        // Process content
        if (choice.Delta?.Content != null)
        {
            Console.Write(choice.Delta.Content);
        }
        
        // Capture finish reason
        if (choice.FinishReason != null)
        {
            finishReason = choice.FinishReason;
        }
    }
}

Console.WriteLine($"\n\nResponse ID: {responseId}");
Console.WriteLine($"Model: {model}");
Console.WriteLine($"Finish Reason: {finishReason}");
```

### Progress Tracking

```csharp
int chunkCount = 0;
int characterCount = 0;
var startTime = DateTime.UtcNow;

await foreach (var chunk in client.StreamChatCompletionAsync(request))
{
    chunkCount++;
    
    if (chunk.Choices?.Count > 0)
    {
        var content = chunk.Choices[0].Delta?.Content;
        if (content != null)
        {
            characterCount += content.Length;
            Console.Write(content);
            
            // Update progress indicator
            Console.Title = $"Chunks: {chunkCount} | Chars: {characterCount}";
        }
    }
}

var duration = DateTime.UtcNow - startTime;
Console.WriteLine($"\n\nStreaming Stats:");
Console.WriteLine($"Total chunks: {chunkCount}");
Console.WriteLine($"Total characters: {characterCount}");
Console.WriteLine($"Duration: {duration.TotalSeconds:F2}s");
Console.WriteLine($"Throughput: {characterCount / duration.TotalSeconds:F0} chars/sec");
```

## Cancellation Support

### Using CancellationToken

```csharp
using var cts = new CancellationTokenSource();

// Cancel after 10 seconds
cts.CancelAfter(TimeSpan.FromSeconds(10));

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
    Console.WriteLine("\n\nStream cancelled.");
}
```

### User-Triggered Cancellation

```csharp
using var cts = new CancellationTokenSource();

// Start streaming in background task
var streamTask = Task.Run(async () =>
{
    await foreach (var chunk in client.StreamChatCompletionAsync(request, cts.Token))
    {
        if (chunk.Choices?[0].Delta?.Content != null)
        {
            Console.Write(chunk.Choices[0].Delta.Content);
        }
    }
});

// Wait for user input to cancel
Console.WriteLine("Press any key to stop streaming...");
Console.ReadKey(true);
cts.Cancel();

try
{
    await streamTask;
}
catch (OperationCanceledException)
{
    Console.WriteLine("\nStream stopped by user.");
}
```

## Error Handling in Streams

### Graceful Error Recovery

```csharp
try
{
    await foreach (var chunk in client.StreamChatCompletionAsync(request))
    {
        try
        {
            if (chunk.Choices?[0].Delta?.Content != null)
            {
                Console.Write(chunk.Choices[0].Delta.Content);
            }
        }
        catch (Exception chunkError)
        {
            // Log individual chunk errors without stopping stream
            Console.Error.WriteLine($"\nChunk error: {chunkError.Message}");
        }
    }
}
catch (HttpRequestException httpEx)
{
    Console.Error.WriteLine($"\nHTTP error: {httpEx.Message}");
}
catch (TaskCanceledException)
{
    Console.WriteLine("\nStream timed out or was cancelled.");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"\nUnexpected error: {ex.Message}");
}
```

### Retry Logic for Streams

```csharp
int maxRetries = 3;
int retryCount = 0;

while (retryCount < maxRetries)
{
    try
    {
        await foreach (var chunk in client.StreamChatCompletionAsync(request))
        {
            if (chunk.Choices?[0].Delta?.Content != null)
            {
                Console.Write(chunk.Choices[0].Delta.Content);
            }
        }
        break; // Success
    }
    catch (Exception ex) when (retryCount < maxRetries - 1)
    {
        retryCount++;
        Console.WriteLine($"\nRetry {retryCount}/{maxRetries} after error: {ex.Message}");
        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount))); // Exponential backoff
    }
}
```

## Streaming with UI Updates

### WPF/WinForms Example

```csharp
private async void StartStreamButton_Click(object sender, EventArgs e)
{
    outputTextBox.Clear();
    
    var request = new ChatRequest
    {
        Messages = new List<ChatMessage> { ChatMessage.User(inputTextBox.Text) },
        Stream = true
    };
    
    await foreach (var chunk in client.StreamChatCompletionAsync(request))
    {
        if (chunk.Choices?[0].Delta?.Content != null)
        {
            // Update UI on main thread
            this.Invoke((Action)(() =>
            {
                outputTextBox.AppendText(chunk.Choices[0].Delta.Content);
            }));
        }
    }
}
```

### ASP.NET Core Streaming Response

```csharp
[HttpGet("stream")]
public async IAsyncEnumerable<string> StreamChat([FromQuery] string prompt)
{
    var request = new ChatRequest
    {
        Messages = new List<ChatMessage> { ChatMessage.User(prompt) },
        Stream = true
    };
    
    await foreach (var chunk in _client.StreamChatCompletionAsync(request))
    {
        if (chunk.Choices?[0].Delta?.Content != null)
        {
            yield return chunk.Choices[0].Delta.Content;
        }
    }
}
```

## Performance Optimization

### Buffering Strategies

```csharp
public async Task<string> StreamWithBufferingAsync(ChatRequest request)
{
    const int bufferSize = 100; // Buffer 100 characters
    var buffer = new StringBuilder(bufferSize);
    var fullResponse = new StringBuilder();
    
    await foreach (var chunk in client.StreamChatCompletionAsync(request))
    {
        if (chunk.Choices?[0].Delta?.Content != null)
        {
            buffer.Append(chunk.Choices[0].Delta.Content);
            
            // Flush buffer when it reaches threshold
            if (buffer.Length >= bufferSize)
            {
                var content = buffer.ToString();
                fullResponse.Append(content);
                Console.Write(content);
                buffer.Clear();
            }
        }
    }
    
    // Flush remaining buffer
    if (buffer.Length > 0)
    {
        var content = buffer.ToString();
        fullResponse.Append(content);
        Console.Write(content);
    }
    
    return fullResponse.ToString();
}
```

### Parallel Stream Processing

```csharp
public async Task ProcessMultipleStreamsAsync(List<ChatRequest> requests)
{
    var tasks = requests.Select(async (request, index) =>
    {
        var response = new StringBuilder();
        
        await foreach (var chunk in client.StreamChatCompletionAsync(request))
        {
            if (chunk.Choices?[0].Delta?.Content != null)
            {
                response.Append(chunk.Choices[0].Delta.Content);
            }
        }
        
        return (Index: index, Response: response.ToString());
    });
    
    var results = await Task.WhenAll(tasks);
    
    foreach (var result in results.OrderBy(r => r.Index))
    {
        Console.WriteLine($"Response {result.Index}: {result.Response}");
    }
}
```

## Stream Response Format

### Chunk Structure

Each streaming chunk follows this structure:

```json
{
  "id": "chatcmpl-abc123",
  "object": "chat.completion.chunk",
  "created": 1234567890,
  "model": "gpt-3.5-turbo",
  "choices": [
    {
      "index": 0,
      "delta": {
        "content": "Hello"
      },
      "finish_reason": null
    }
  ]
}
```

### Final Chunk

The final chunk includes the finish reason:

```json
{
  "choices": [
    {
      "index": 0,
      "delta": {},
      "finish_reason": "stop"
    }
  ]
}
```

## Best Practices

### 1. Always Enable Streaming for Long Responses

```csharp
var request = new ChatRequest
{
    Messages = messages,
    Stream = true,  // Enable for better UX
    MaxTokens = 2000  // Long responses benefit most
};
```

### 2. Handle Connection Interruptions

```csharp
public async Task<string> ResilientStreamAsync(ChatRequest request)
{
    var collected = new StringBuilder();
    var lastPosition = 0;
    
    while (true)
    {
        try
        {
            await foreach (var chunk in client.StreamChatCompletionAsync(request))
            {
                if (chunk.Choices?[0].Delta?.Content != null)
                {
                    collected.Append(chunk.Choices[0].Delta.Content);
                    lastPosition = collected.Length;
                }
                
                if (chunk.Choices?[0].FinishReason != null)
                {
                    return collected.ToString();
                }
            }
        }
        catch (Exception ex) when (lastPosition > 0)
        {
            // Attempt to continue from where we left off
            Console.WriteLine($"Stream interrupted at position {lastPosition}: {ex.Message}");
            // In practice, you'd need to modify the request to continue
            throw;
        }
    }
}
```

### 3. Monitor Stream Health

```csharp
var lastChunkTime = DateTime.UtcNow;
var timeout = TimeSpan.FromSeconds(30);

await foreach (var chunk in client.StreamChatCompletionAsync(request))
{
    var now = DateTime.UtcNow;
    if (now - lastChunkTime > timeout)
    {
        throw new TimeoutException("Stream stalled");
    }
    lastChunkTime = now;
    
    // Process chunk...
}
```

## Troubleshooting

### Stream Not Starting

- Verify `Stream = true` in request
- Check network connectivity
- Ensure model supports streaming

### Incomplete Responses

- Check for `finish_reason` in final chunk
- Monitor for connection timeouts
- Verify `MaxTokens` setting

### Performance Issues

- Use `ConfigureAwait(false)` in library code
- Avoid blocking operations in stream loop
- Consider buffering for UI updates

## Limitations

- Not all models support streaming
- Token usage statistics arrive in final chunk only
- Cannot modify request mid-stream
- No built-in resume capability

## Further Reading

- [API Reference](api-reference.md#streamchatcompletionasync)
- [Examples](examples.md#streaming-examples)
- [Error Handling](error-handling.md)