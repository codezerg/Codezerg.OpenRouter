# Error Handling Guide

This guide covers error handling strategies, common errors, and best practices for building robust applications with Codezerg.OpenRouter.

## Error Types

### API Errors

OpenRouter API errors are returned with HTTP status codes and error details:

```csharp
public class ApiError
{
    public ErrorDetail Error { get; set; }
}

public class ErrorDetail
{
    public int Code { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }
    public object Metadata { get; set; }
}
```

### Common HTTP Status Codes

| Status Code | Meaning | Common Causes |
|------------|---------|---------------|
| 400 | Bad Request | Invalid parameters, malformed JSON |
| 401 | Unauthorized | Invalid or missing API key |
| 403 | Forbidden | Access denied to resource/model |
| 404 | Not Found | Invalid endpoint or model |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Internal Server Error | OpenRouter service issue |
| 502 | Bad Gateway | Provider (upstream) error |
| 503 | Service Unavailable | Temporary service outage |
| 504 | Gateway Timeout | Request took too long |

## Basic Error Handling

### Try-Catch Pattern

```csharp
try
{
    var response = await client.SendChatCompletionAsync(request);
    // Process successful response
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"HTTP Error: {ex.StatusCode} - {ex.Message}");
    // Handle HTTP errors
}
catch (TaskCanceledException ex)
{
    Console.WriteLine($"Request cancelled or timed out: {ex.Message}");
    // Handle timeout
}
catch (JsonException ex)
{
    Console.WriteLine($"JSON parsing error: {ex.Message}");
    // Handle parsing errors
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
    // Handle other errors
}
```

### Detailed Error Information

```csharp
public async Task<ChatResponse> SendWithDetailedErrorHandlingAsync(ChatRequest request)
{
    try
    {
        return await client.SendChatCompletionAsync(request);
    }
    catch (HttpRequestException ex)
    {
        var errorInfo = new
        {
            StatusCode = ex.StatusCode,
            Message = ex.Message,
            Request = new
            {
                Model = request.Model,
                MessageCount = request.Messages?.Count,
                Timestamp = DateTime.UtcNow
            }
        };
        
        LogError(errorInfo);
        
        // Re-throw with context
        throw new ApplicationException(
            $"OpenRouter API error: {ex.StatusCode}", ex);
    }
}
```

## Specific Error Scenarios

### Authentication Errors

```csharp
public class AuthenticationErrorHandler
{
    public async Task<ChatResponse> HandleAuthErrorsAsync(
        OpenRouterClient client, 
        ChatRequest request)
    {
        try
        {
            return await client.SendChatCompletionAsync(request);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Check API key format
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPENROUTER_API_KEY")))
            {
                throw new InvalidOperationException(
                    "API key not found. Set OPENROUTER_API_KEY environment variable.");
            }
            
            // API key might be invalid
            throw new InvalidOperationException(
                "Invalid API key. Please check your OpenRouter API key.", ex);
        }
    }
}
```

### Rate Limiting

```csharp
public class RateLimitHandler
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private DateTime _lastRequestTime = DateTime.MinValue;
    private int _requestCount = 0;
    
    public async Task<ChatResponse> SendWithRateLimitingAsync(
        OpenRouterClient client,
        ChatRequest request,
        int maxRetries = 3)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                await _semaphore.WaitAsync();
                
                // Simple rate limiting
                var timeSinceLastRequest = DateTime.UtcNow - _lastRequestTime;
                if (timeSinceLastRequest < TimeSpan.FromSeconds(1))
                {
                    await Task.Delay(1000 - (int)timeSinceLastRequest.TotalMilliseconds);
                }
                
                _lastRequestTime = DateTime.UtcNow;
                _requestCount++;
                
                return await client.SendChatCompletionAsync(request);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                if (attempt == maxRetries - 1) throw;
                
                // Parse retry-after header if available
                var retryAfter = GetRetryAfterSeconds(ex) ?? Math.Pow(2, attempt + 1);
                
                Console.WriteLine($"Rate limited. Retrying after {retryAfter} seconds...");
                await Task.Delay(TimeSpan.FromSeconds(retryAfter));
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        throw new Exception("Max retries exceeded");
    }
    
    private int? GetRetryAfterSeconds(HttpRequestException ex)
    {
        // Parse Retry-After header from exception details
        // Implementation depends on how headers are accessible
        return null;
    }
}
```

### Model Availability

```csharp
public class ModelAvailabilityHandler
{
    private readonly Dictionary<string, List<string>> _modelFallbacks = new()
    {
        ["openai/gpt-4"] = new() { "openai/gpt-4-turbo", "openai/gpt-3.5-turbo" },
        ["anthropic/claude-3-opus"] = new() { "anthropic/claude-3-sonnet", "anthropic/claude-3-haiku" },
        ["google/gemini-pro"] = new() { "google/gemini-2.0-flash-exp" }
    };
    
    public async Task<ChatResponse> SendWithFallbackAsync(
        OpenRouterClient client,
        ChatRequest request)
    {
        var modelsToTry = new List<string> { request.Model };
        
        if (_modelFallbacks.TryGetValue(request.Model, out var fallbacks))
        {
            modelsToTry.AddRange(fallbacks);
        }
        
        Exception lastException = null;
        
        foreach (var model in modelsToTry)
        {
            try
            {
                request.Model = model;
                Console.WriteLine($"Trying model: {model}");
                return await client.SendChatCompletionAsync(request);
            }
            catch (HttpRequestException ex) when 
                (ex.StatusCode == HttpStatusCode.NotFound ||
                 ex.StatusCode == HttpStatusCode.Forbidden)
            {
                lastException = ex;
                Console.WriteLine($"Model {model} not available: {ex.Message}");
            }
        }
        
        throw new Exception($"No available models found. Last error: {lastException?.Message}", 
                          lastException);
    }
}
```

### Timeout Handling

```csharp
public class TimeoutHandler
{
    public async Task<ChatResponse> SendWithTimeoutHandlingAsync(
        OpenRouterClient client,
        ChatRequest request,
        TimeSpan? customTimeout = null)
    {
        using var cts = new CancellationTokenSource(customTimeout ?? TimeSpan.FromMinutes(2));
        
        try
        {
            return await client.SendChatCompletionAsync(request, cts.Token);
        }
        catch (TaskCanceledException ex)
        {
            if (cts.IsCancellationRequested)
            {
                // Our timeout was hit
                throw new TimeoutException(
                    $"Request timed out after {customTimeout?.TotalSeconds ?? 120} seconds", ex);
            }
            else
            {
                // External cancellation
                throw new OperationCanceledException("Request was cancelled", ex);
            }
        }
    }
}
```

## Retry Strategies

### Exponential Backoff

```csharp
public class ExponentialBackoffRetry
{
    public async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation,
        int maxRetries = 3,
        int baseDelayMs = 1000)
    {
        Exception lastException = null;
        
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                return await operation();
            }
            catch (HttpRequestException ex) when (IsRetriable(ex))
            {
                lastException = ex;
                
                if (attempt < maxRetries - 1)
                {
                    var delay = baseDelayMs * Math.Pow(2, attempt);
                    var jitter = new Random().Next(0, (int)(delay * 0.1));
                    
                    Console.WriteLine($"Attempt {attempt + 1} failed. " +
                                    $"Retrying in {delay + jitter}ms...");
                    
                    await Task.Delay(TimeSpan.FromMilliseconds(delay + jitter));
                }
            }
        }
        
        throw new Exception($"Operation failed after {maxRetries} attempts", lastException);
    }
    
    private bool IsRetriable(HttpRequestException ex)
    {
        return ex.StatusCode switch
        {
            HttpStatusCode.TooManyRequests => true,
            HttpStatusCode.ServiceUnavailable => true,
            HttpStatusCode.GatewayTimeout => true,
            HttpStatusCode.BadGateway => true,
            HttpStatusCode.InternalServerError => true,
            _ => false
        };
    }
}
```

### Circuit Breaker Pattern

```csharp
public class CircuitBreaker
{
    private readonly int _threshold;
    private readonly TimeSpan _timeout;
    private int _failureCount;
    private DateTime _lastFailureTime;
    private CircuitState _state = CircuitState.Closed;
    
    public CircuitBreaker(int threshold = 5, TimeSpan? timeout = null)
    {
        _threshold = threshold;
        _timeout = timeout ?? TimeSpan.FromMinutes(1);
    }
    
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        if (_state == CircuitState.Open)
        {
            if (DateTime.UtcNow - _lastFailureTime > _timeout)
            {
                _state = CircuitState.HalfOpen;
            }
            else
            {
                throw new CircuitOpenException("Circuit breaker is open");
            }
        }
        
        try
        {
            var result = await operation();
            
            if (_state == CircuitState.HalfOpen)
            {
                _state = CircuitState.Closed;
                _failureCount = 0;
            }
            
            return result;
        }
        catch (Exception)
        {
            _lastFailureTime = DateTime.UtcNow;
            _failureCount++;
            
            if (_failureCount >= _threshold)
            {
                _state = CircuitState.Open;
                throw new CircuitOpenException("Circuit breaker opened due to failures");
            }
            
            throw;
        }
    }
    
    private enum CircuitState { Closed, Open, HalfOpen }
}

public class CircuitOpenException : Exception
{
    public CircuitOpenException(string message) : base(message) { }
}
```

## Error Logging

### Structured Logging

```csharp
public class ErrorLogger
{
    private readonly ILogger<ErrorLogger> _logger;
    
    public void LogApiError(HttpRequestException ex, ChatRequest request)
    {
        _logger.LogError(ex, "OpenRouter API error occurred",
            new
            {
                StatusCode = ex.StatusCode,
                Model = request.Model,
                MessageCount = request.Messages?.Count,
                IsStreaming = request.Stream,
                Timestamp = DateTime.UtcNow,
                RequestId = Guid.NewGuid()
            });
    }
    
    public void LogRetry(int attempt, TimeSpan delay, Exception error)
    {
        _logger.LogWarning("Retrying API request",
            new
            {
                Attempt = attempt,
                DelayMs = delay.TotalMilliseconds,
                ErrorType = error.GetType().Name,
                ErrorMessage = error.Message
            });
    }
}
```

## Graceful Degradation

### Feature Fallback

```csharp
public class GracefulDegradation
{
    public async Task<ChatResponse> SendWithDegradationAsync(
        OpenRouterClient client,
        ChatRequest request)
    {
        // Try with full features
        try
        {
            return await client.SendChatCompletionAsync(request);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("max_tokens"))
        {
            // Reduce token limit
            request.MaxTokens = Math.Min(request.MaxTokens ?? 1000, 500);
            return await client.SendChatCompletionAsync(request);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("context_length"))
        {
            // Truncate messages
            request.Messages = TruncateMessages(request.Messages);
            return await client.SendChatCompletionAsync(request);
        }
    }
    
    private List<ChatMessage> TruncateMessages(List<ChatMessage> messages)
    {
        // Keep system message and last few user/assistant messages
        var truncated = new List<ChatMessage>();
        
        var systemMessage = messages.FirstOrDefault(m => m.Role == ChatRole.System);
        if (systemMessage != null)
        {
            truncated.Add(systemMessage);
        }
        
        // Keep last 5 messages
        truncated.AddRange(messages.TakeLast(5));
        
        return truncated;
    }
}
```

## Validation and Prevention

### Request Validation

```csharp
public class RequestValidator
{
    public void ValidateRequest(ChatRequest request)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrEmpty(request.Model))
        {
            errors.Add("Model is required");
        }
        
        if (request.Messages == null || request.Messages.Count == 0)
        {
            errors.Add("At least one message is required");
        }
        
        if (request.Temperature.HasValue && 
            (request.Temperature < 0 || request.Temperature > 2))
        {
            errors.Add("Temperature must be between 0 and 2");
        }
        
        if (request.MaxTokens.HasValue && request.MaxTokens < 1)
        {
            errors.Add("MaxTokens must be positive");
        }
        
        if (errors.Any())
        {
            throw new ArgumentException(
                $"Invalid request: {string.Join(", ", errors)}");
        }
    }
}
```

## Error Recovery Patterns

### Automatic Recovery

```csharp
public class AutoRecoveryHandler
{
    private readonly Dictionary<string, DateTime> _errorHistory = new();
    private readonly TimeSpan _recoveryWindow = TimeSpan.FromMinutes(5);
    
    public async Task<ChatResponse> SendWithAutoRecoveryAsync(
        OpenRouterClient client,
        ChatRequest request)
    {
        var key = $"{request.Model}:{request.GetHashCode()}";
        
        // Check if we've seen this error recently
        if (_errorHistory.TryGetValue(key, out var lastError))
        {
            if (DateTime.UtcNow - lastError < _recoveryWindow)
            {
                // Apply recovery strategy
                request = ApplyRecoveryStrategy(request);
            }
        }
        
        try
        {
            var response = await client.SendChatCompletionAsync(request);
            _errorHistory.Remove(key); // Success, clear error history
            return response;
        }
        catch (Exception)
        {
            _errorHistory[key] = DateTime.UtcNow;
            throw;
        }
    }
    
    private ChatRequest ApplyRecoveryStrategy(ChatRequest request)
    {
        // Reduce complexity
        request.Temperature = Math.Min(request.Temperature ?? 0.7, 0.5);
        request.MaxTokens = Math.Min(request.MaxTokens ?? 1000, 500);
        
        return request;
    }
}
```

## Best Practices

1. **Always handle specific exceptions first**
2. **Log errors with context for debugging**
3. **Implement retry logic for transient failures**
4. **Use circuit breakers for failing services**
5. **Validate requests before sending**
6. **Provide meaningful error messages to users**
7. **Monitor error rates and patterns**
8. **Test error scenarios**

## Common Error Messages

| Error Message | Cause | Solution |
|--------------|-------|----------|
| "Invalid API key" | Wrong or missing API key | Check API key configuration |
| "Model not found" | Invalid model name | Verify model name format |
| "Rate limit exceeded" | Too many requests | Implement rate limiting |
| "Context length exceeded" | Message too long | Reduce message size |
| "Invalid request format" | Malformed JSON | Validate request structure |
| "Timeout" | Request took too long | Increase timeout or simplify request |

## Further Reading

- [API Reference](api-reference.md#error-handling)
- [Configuration](configuration.md)
- [Models](models.md#model-fallback-strategy)