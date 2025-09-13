# Supported Models

This guide provides information about the AI models available through OpenRouter and how to use them effectively with Codezerg.OpenRouter.

## Model Selection

Models are identified by their provider and name in the format: `provider/model-name`

```csharp
var request = new ChatRequest
{
    Model = "openai/gpt-4",  // Format: provider/model
    Messages = messages
};
```

## Model Categories

### General Purpose Models

These models excel at a wide variety of tasks including conversation, analysis, and content generation.

#### OpenAI Models
```csharp
// GPT-4 - Most capable, higher cost
request.Model = "openai/gpt-4";
request.Model = "openai/gpt-4-32k";  // Extended context

// GPT-4 Turbo - Faster, more affordable
request.Model = "openai/gpt-4-turbo";
request.Model = "openai/gpt-4-turbo-preview";

// GPT-3.5 - Fast and affordable
request.Model = "openai/gpt-3.5-turbo";
request.Model = "openai/gpt-3.5-turbo-16k";
```

#### Anthropic Claude Models
```csharp
// Claude 3 Family
request.Model = "anthropic/claude-3-opus";     // Most capable
request.Model = "anthropic/claude-3-sonnet";   // Balanced
request.Model = "anthropic/claude-3-haiku";    // Fast and light

// Claude 2
request.Model = "anthropic/claude-2";
request.Model = "anthropic/claude-2.1";        // 200K context
```

#### Google Models
```csharp
// Gemini
request.Model = "google/gemini-pro";
request.Model = "google/gemini-2.0-flash-exp";  // Fast multimodal
request.Model = "google/gemini-pro-vision";     // Vision capable

// PaLM
request.Model = "google/palm-2-chat-bison";
request.Model = "google/palm-2-codechat-bison"; // Code-focused
```

### Open Source Models

Cost-effective models that can be self-hosted.

#### Meta Llama Models
```csharp
// Llama 3
request.Model = "meta-llama/llama-3-70b-instruct";
request.Model = "meta-llama/llama-3-8b-instruct";

// Llama 2
request.Model = "meta-llama/llama-2-70b-chat";
request.Model = "meta-llama/llama-2-13b-chat";
request.Model = "meta-llama/llama-2-7b-chat";
```

#### Mistral Models
```csharp
request.Model = "mistralai/mistral-large";
request.Model = "mistralai/mixtral-8x7b-instruct";
request.Model = "mistralai/mistral-7b-instruct";
```

### Specialized Models

#### Code Generation
```csharp
// DeepSeek Coder
request.Model = "deepseek/deepseek-coder-33b-instruct";

// Code Llama
request.Model = "meta-llama/codellama-70b-instruct";
request.Model = "meta-llama/codellama-34b-instruct";

// WizardCoder
request.Model = "wizardlm/wizardcoder-33b";
```

#### Vision Models
```csharp
// Multimodal models that can process images
request.Model = "openai/gpt-4o";               // GPT-4 with vision
request.Model = "openai/gpt-4-vision-preview"; 
request.Model = "google/gemini-pro-vision";
request.Model = "anthropic/claude-3-opus";     // All Claude 3 models support vision
```

#### Free Models
```csharp
// Models available at no cost (may have rate limits)
request.Model = "deepseek/deepseek-chat";      // Free tier available
request.Model = "google/gemini-2.0-flash-exp:free";
```

## Model Selection Guide

### By Use Case

```csharp
public static class ModelSelector
{
    // Creative writing, stories, marketing copy
    public static string Creative => "anthropic/claude-3-opus";
    
    // Code generation and debugging
    public static string Coding => "deepseek/deepseek-coder-33b-instruct";
    
    // General conversation and Q&A
    public static string Conversation => "openai/gpt-3.5-turbo";
    
    // Complex reasoning and analysis
    public static string Analysis => "openai/gpt-4";
    
    // Image understanding
    public static string Vision => "google/gemini-2.0-flash-exp";
    
    // Fast responses with good quality
    public static string Fast => "anthropic/claude-3-haiku";
    
    // Budget-friendly option
    public static string Budget => "meta-llama/llama-3-8b-instruct";
    
    // Maximum context length
    public static string LongContext => "anthropic/claude-2.1"; // 200K tokens
}
```

### Dynamic Model Selection

```csharp
public class SmartModelSelector
{
    private readonly OpenRouterClient _client;
    
    public async Task<ChatResponse> SendWithBestModelAsync(
        ChatRequest request, 
        ModelPriority priority)
    {
        request.Model = SelectModel(request, priority);
        return await _client.SendChatCompletionAsync(request);
    }
    
    private string SelectModel(ChatRequest request, ModelPriority priority)
    {
        // Check if multimodal content
        bool hasImages = request.Messages.Any(m => m.IsMultimodal);
        if (hasImages)
        {
            return "google/gemini-2.0-flash-exp";
        }
        
        // Check message length
        int estimatedTokens = EstimateTokens(request);
        
        switch (priority)
        {
            case ModelPriority.Quality:
                return estimatedTokens > 8000 
                    ? "anthropic/claude-2.1"  // Long context
                    : "openai/gpt-4";          // High quality
                    
            case ModelPriority.Speed:
                return "anthropic/claude-3-haiku";
                
            case ModelPriority.Cost:
                return "meta-llama/llama-3-8b-instruct";
                
            default:
                return "openai/gpt-3.5-turbo";  // Balanced default
        }
    }
    
    private int EstimateTokens(ChatRequest request)
    {
        // Rough estimation: 1 token ≈ 4 characters
        int chars = request.Messages.Sum(m => 
            m.Content?.ToString()?.Length ?? 0);
        return chars / 4;
    }
}
```

## Model Capabilities

### Context Windows

| Model | Context Window | Notes |
|-------|---------------|-------|
| claude-2.1 | 200,000 tokens | Largest context |
| gpt-4-turbo | 128,000 tokens | Good for long documents |
| gpt-4-32k | 32,768 tokens | Extended GPT-4 |
| gemini-pro | 32,768 tokens | Multimodal support |
| gpt-3.5-turbo-16k | 16,384 tokens | Extended GPT-3.5 |
| llama-3-70b | 8,192 tokens | Open source |
| gpt-4 | 8,192 tokens | Standard GPT-4 |
| gpt-3.5-turbo | 4,096 tokens | Standard GPT-3.5 |

### Feature Support Matrix

| Feature | GPT-4 | Claude 3 | Gemini | Llama 3 |
|---------|-------|----------|--------|---------|
| Text Generation | ✅ | ✅ | ✅ | ✅ |
| Vision/Images | ✅* | ✅ | ✅ | ❌ |
| Function Calling | ✅ | ✅ | ✅ | ⚠️ |
| Streaming | ✅ | ✅ | ✅ | ✅ |
| JSON Mode | ✅ | ✅ | ✅ | ⚠️ |

*GPT-4 vision requires specific model variant

## Model Configuration

### Temperature Settings by Model

```csharp
public static class ModelTemperatures
{
    public static Dictionary<string, double> Recommended = new()
    {
        ["openai/gpt-4"] = 0.7,
        ["openai/gpt-3.5-turbo"] = 0.7,
        ["anthropic/claude-3-opus"] = 0.8,
        ["anthropic/claude-3-sonnet"] = 0.7,
        ["google/gemini-pro"] = 0.9,
        ["meta-llama/llama-3-70b-instruct"] = 0.8,
        ["deepseek/deepseek-coder-33b-instruct"] = 0.1  // Low for code
    };
}
```

### Model-Specific Parameters

```csharp
// OpenAI models
var openAiRequest = new ChatRequest
{
    Model = "openai/gpt-4",
    Messages = messages,
    Temperature = 0.7,
    TopP = 0.9,
    FrequencyPenalty = 0.5,
    PresencePenalty = 0.5,
    LogProbs = true,
    TopLogProbs = 5
};

// Anthropic models
var claudeRequest = new ChatRequest
{
    Model = "anthropic/claude-3-opus",
    Messages = messages,
    Temperature = 0.8,
    MaxTokens = 4000,  // Required for Claude
    TopK = 40,         // Claude-specific
    TopP = 0.95
};

// Google models
var geminiRequest = new ChatRequest
{
    Model = "google/gemini-pro",
    Messages = messages,
    Temperature = 0.9,
    TopP = 0.95,
    TopK = 40,
    MaxTokens = 2048
};
```

## Provider-Specific Options

```csharp
// OpenAI-specific options
request.ProviderOptions = new ProviderOptions
{
    OpenAI = new Dictionary<string, object>
    {
        ["logit_bias"] = new Dictionary<string, int> { ["50256"] = -100 },
        ["seed"] = 42
    }
};

// Anthropic-specific options
request.ProviderOptions = new ProviderOptions
{
    Anthropic = new Dictionary<string, object>
    {
        ["max_tokens"] = 4000,  // Required
        ["top_k"] = 40
    }
};
```

## Cost Optimization

### Model Pricing Tiers

```csharp
public enum PricingTier
{
    Premium,    // GPT-4, Claude Opus
    Standard,   // GPT-3.5, Claude Sonnet, Gemini Pro
    Budget,     // Llama, Mistral
    Free        // DeepSeek Chat (free tier)
}

public class CostOptimizer
{
    public string SelectModelByBudget(
        decimal maxCostPer1kTokens, 
        bool needsVision = false)
    {
        if (maxCostPer1kTokens < 0.001m)
        {
            return "meta-llama/llama-3-8b-instruct";
        }
        else if (maxCostPer1kTokens < 0.01m)
        {
            return needsVision 
                ? "google/gemini-2.0-flash-exp" 
                : "openai/gpt-3.5-turbo";
        }
        else
        {
            return needsVision 
                ? "openai/gpt-4o" 
                : "openai/gpt-4";
        }
    }
}
```

### Token Usage Estimation

```csharp
public class TokenEstimator
{
    // Rough estimates for planning
    public static int EstimateTokens(string text)
    {
        // English: ~1 token per 4 characters
        // Code: ~1 token per 3 characters
        bool isCode = text.Contains("function") || 
                     text.Contains("class") || 
                     text.Contains("import");
        
        int charsPerToken = isCode ? 3 : 4;
        return text.Length / charsPerToken;
    }
    
    public static decimal EstimateCost(
        string model, 
        int inputTokens, 
        int outputTokens)
    {
        // Example pricing (check current rates)
        var pricing = new Dictionary<string, (decimal input, decimal output)>
        {
            ["openai/gpt-4"] = (0.03m, 0.06m),
            ["openai/gpt-3.5-turbo"] = (0.001m, 0.002m),
            ["anthropic/claude-3-opus"] = (0.015m, 0.075m),
            ["meta-llama/llama-3-70b-instruct"] = (0.0007m, 0.0009m)
        };
        
        if (pricing.TryGetValue(model, out var rate))
        {
            return (inputTokens * rate.input + outputTokens * rate.output) / 1000;
        }
        
        return 0;
    }
}
```

## Model Fallback Strategy

```csharp
public class ModelFallbackHandler
{
    private readonly List<string> _fallbackChain = new()
    {
        "openai/gpt-4",
        "anthropic/claude-3-sonnet",
        "openai/gpt-3.5-turbo",
        "meta-llama/llama-3-70b-instruct"
    };
    
    public async Task<ChatResponse> SendWithFallbackAsync(
        OpenRouterClient client, 
        ChatRequest request)
    {
        Exception lastException = null;
        
        foreach (var model in _fallbackChain)
        {
            try
            {
                request.Model = model;
                return await client.SendChatCompletionAsync(request);
            }
            catch (HttpRequestException ex) when 
                (ex.Message.Contains("rate_limit") || 
                 ex.Message.Contains("model_not_available"))
            {
                lastException = ex;
                Console.WriteLine($"Model {model} failed, trying next...");
                await Task.Delay(1000);  // Brief delay before retry
            }
        }
        
        throw new Exception("All models failed", lastException);
    }
}
```

## Best Practices

### 1. Model Versioning

```csharp
public static class ModelVersions
{
    // Pin specific versions for consistency
    public const string StableGpt4 = "openai/gpt-4-0613";
    public const string StableGpt35 = "openai/gpt-3.5-turbo-0613";
    public const string LatestClaude = "anthropic/claude-3-opus-20240229";
}
```

### 2. A/B Testing

```csharp
public class ModelABTester
{
    private readonly Random _random = new();
    
    public async Task<(string model, ChatResponse response)> TestModelsAsync(
        OpenRouterClient client,
        ChatRequest request,
        params string[] models)
    {
        var selectedModel = models[_random.Next(models.Length)];
        request.Model = selectedModel;
        
        var response = await client.SendChatCompletionAsync(request);
        
        // Log for analysis
        LogModelPerformance(selectedModel, response);
        
        return (selectedModel, response);
    }
}
```

### 3. Model Monitoring

```csharp
public class ModelMonitor
{
    public async Task<ModelMetrics> GetModelMetricsAsync(
        string model,
        ChatRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await client.SendChatCompletionAsync(request);
        stopwatch.Stop();
        
        return new ModelMetrics
        {
            Model = model,
            Latency = stopwatch.ElapsedMilliseconds,
            InputTokens = response.Usage?.PromptTokens ?? 0,
            OutputTokens = response.Usage?.CompletionTokens ?? 0,
            TotalCost = EstimateCost(model, response.Usage),
            FinishReason = response.Choices[0].FinishReason
        };
    }
}
```

## Checking Model Availability

Visit [OpenRouter Models](https://openrouter.ai/models) for:
- Current model list
- Pricing information
- Context window sizes
- Feature support
- Rate limits

## Further Reading

- [Getting Started](getting-started.md)
- [API Reference](api-reference.md)
- [Configuration](configuration.md)
- [Examples](examples.md)