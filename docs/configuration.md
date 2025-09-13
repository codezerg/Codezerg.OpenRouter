# Configuration Guide

This guide covers all configuration options available in Codezerg.OpenRouter and how to use them effectively.

## OpenRouterClientOptions

The `OpenRouterClientOptions` class is the primary configuration container for the client.

### Basic Configuration

```csharp
var config = new OpenRouterClientOptions
{
    ApiKey = "sk-or-v1-your-api-key",
    DefaultModel = "openai/gpt-3.5-turbo",
    Timeout = TimeSpan.FromSeconds(60)
};

var client = new OpenRouterClient(config);
```

### Complete Configuration Options

```csharp
var config = new OpenRouterClientOptions
{
    // Required
    ApiKey = "sk-or-v1-your-api-key",
    
    // Optional - API Settings
    BaseUrl = "https://openrouter.ai/api/v1",  // Default OpenRouter endpoint
    DefaultModel = "openai/gpt-3.5-turbo",      // Default model for requests
    
    // Optional - Timeout Settings
    Timeout = TimeSpan.FromMinutes(2),          // Overall request timeout
    
    // Optional - Retry Settings
    MaxRetries = 3,                             // Number of retry attempts
    RetryDelay = TimeSpan.FromSeconds(1),       // Delay between retries
    
    // Optional - Headers
    DefaultHeaders = new Dictionary<string, string>
    {
        ["HTTP-Referer"] = "https://myapp.com",
        ["X-Title"] = "My Application"
    }
};
```

## Configuration Sources

### Environment Variables

```csharp
public class EnvironmentConfig
{
    public static OpenRouterClientOptions FromEnvironment()
    {
        return new OpenRouterClientOptions
        {
            ApiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") 
                    ?? throw new InvalidOperationException("OPENROUTER_API_KEY not set"),
            DefaultModel = Environment.GetEnvironmentVariable("OPENROUTER_DEFAULT_MODEL") 
                          ?? "openai/gpt-3.5-turbo",
            BaseUrl = Environment.GetEnvironmentVariable("OPENROUTER_BASE_URL") 
                     ?? "https://openrouter.ai/api/v1"
        };
    }
}
```

### Configuration Files (appsettings.json)

```json
{
  "OpenRouter": {
    "ApiKey": "sk-or-v1-your-api-key",
    "DefaultModel": "openai/gpt-4",
    "Timeout": "00:02:00",
    "MaxRetries": 3,
    "Headers": {
      "HTTP-Referer": "https://myapp.com",
      "X-Title": "My Application"
    }
  }
}
```

```csharp
public class ConfigurationFileLoader
{
    public static OpenRouterClientOptions FromConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetSection("OpenRouter");
        
        return new OpenRouterClientOptions
        {
            ApiKey = section["ApiKey"],
            DefaultModel = section["DefaultModel"],
            Timeout = TimeSpan.Parse(section["Timeout"] ?? "00:02:00"),
            MaxRetries = int.Parse(section["MaxRetries"] ?? "3"),
            DefaultHeaders = section.GetSection("Headers")
                .GetChildren()
                .ToDictionary(x => x.Key, x => x.Value)
        };
    }
}
```

### Dependency Injection

```csharp
// In Startup.cs or Program.cs
public void ConfigureServices(IServiceCollection services)
{
    // Configure options
    services.Configure<OpenRouterClientOptions>(
        Configuration.GetSection("OpenRouter"));
    
    // Register client as singleton
    services.AddSingleton<OpenRouterClient>(provider =>
    {
        var options = provider.GetRequiredService<IOptions<OpenRouterClientOptions>>().Value;
        return new OpenRouterClient(options);
    });
    
    // Or as scoped/transient
    services.AddScoped<OpenRouterClient>(provider =>
    {
        var options = provider.GetRequiredService<IOptions<OpenRouterClientOptions>>().Value;
        return new OpenRouterClient(options);
    });
}
```

## Advanced Configuration

### Custom HTTP Client

```csharp
// Configure custom HttpClient
var httpClient = new HttpClient(new HttpClientHandler
{
    // Proxy settings
    Proxy = new WebProxy("http://proxy.example.com:8080"),
    UseProxy = true,
    
    // Certificate validation
    ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => true,
    
    // Compression
    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
});

// Set custom timeout
httpClient.Timeout = TimeSpan.FromMinutes(5);

// Use with OpenRouterClient
var client = new OpenRouterClient(config, httpClient);
```

### Connection Pooling

```csharp
public class OptimizedHttpClientFactory
{
    public static HttpClient CreateOptimizedClient()
    {
        var socketsHandler = new SocketsHttpHandler
        {
            // Connection pooling
            PooledConnectionLifetime = TimeSpan.FromMinutes(15),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
            MaxConnectionsPerServer = 10,
            
            // DNS refresh
            ConnectTimeout = TimeSpan.FromSeconds(30),
            
            // Keep-alive
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            
            // Automatic decompression
            AutomaticDecompression = DecompressionMethods.All
        };
        
        return new HttpClient(socketsHandler);
    }
}
```

### Request Headers

```csharp
public class HeaderConfiguration
{
    public static Dictionary<string, string> GetDefaultHeaders(string appName, string appUrl)
    {
        return new Dictionary<string, string>
        {
            // Required by some models
            ["HTTP-Referer"] = appUrl,
            ["X-Title"] = appName,
            
            // Optional tracking
            ["X-Request-ID"] = Guid.NewGuid().ToString(),
            ["X-Session-ID"] = GetSessionId(),
            
            // User agent
            ["User-Agent"] = $"{appName}/1.0 Codezerg.OpenRouter/1.0"
        };
    }
    
    private static string GetSessionId()
    {
        // Implement session tracking logic
        return Guid.NewGuid().ToString();
    }
}
```

## Configuration Patterns

### Multi-Environment Configuration

```csharp
public class EnvironmentAwareConfig
{
    public static OpenRouterClientOptions GetConfig(string environment)
    {
        return environment switch
        {
            "Development" => new OpenRouterClientOptions
            {
                ApiKey = GetDevApiKey(),
                DefaultModel = "meta-llama/llama-3-8b-instruct", // Cheaper for dev
                Timeout = TimeSpan.FromMinutes(5), // Longer for debugging
                BaseUrl = "http://localhost:8080/mock-api" // Mock server
            },
            
            "Staging" => new OpenRouterClientOptions
            {
                ApiKey = GetStagingApiKey(),
                DefaultModel = "openai/gpt-3.5-turbo",
                Timeout = TimeSpan.FromMinutes(2)
            },
            
            "Production" => new OpenRouterClientOptions
            {
                ApiKey = GetProductionApiKey(),
                DefaultModel = "openai/gpt-4",
                Timeout = TimeSpan.FromSeconds(60),
                MaxRetries = 5 // More retries for production
            },
            
            _ => throw new InvalidOperationException($"Unknown environment: {environment}")
        };
    }
}
```

### Configuration Validation

```csharp
public class ConfigurationValidator
{
    public static void ValidateConfiguration(OpenRouterClientOptions options)
    {
        var errors = new List<string>();
        
        // API Key validation
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            errors.Add("ApiKey is required");
        }
        else if (!options.ApiKey.StartsWith("sk-or-"))
        {
            errors.Add("ApiKey should start with 'sk-or-'");
        }
        
        // URL validation
        if (!string.IsNullOrEmpty(options.BaseUrl))
        {
            if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri))
            {
                errors.Add("BaseUrl must be a valid URL");
            }
            else if (uri.Scheme != "https" && !IsDevelopment())
            {
                errors.Add("BaseUrl must use HTTPS in production");
            }
        }
        
        // Timeout validation
        if (options.Timeout <= TimeSpan.Zero)
        {
            errors.Add("Timeout must be positive");
        }
        else if (options.Timeout > TimeSpan.FromMinutes(10))
        {
            errors.Add("Timeout cannot exceed 10 minutes");
        }
        
        // Retry validation
        if (options.MaxRetries < 0)
        {
            errors.Add("MaxRetries cannot be negative");
        }
        else if (options.MaxRetries > 10)
        {
            errors.Add("MaxRetries cannot exceed 10");
        }
        
        if (errors.Any())
        {
            throw new InvalidOperationException(
                $"Configuration validation failed:\n{string.Join("\n", errors)}");
        }
    }
    
    private static bool IsDevelopment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    }
}
```

## Dynamic Configuration

### Runtime Configuration Changes

```csharp
public class DynamicConfigManager
{
    private OpenRouterClientOptions _config;
    private OpenRouterClient _client;
    private readonly object _lock = new();
    
    public DynamicConfigManager(OpenRouterClientOptions initialConfig)
    {
        _config = initialConfig;
        _client = new OpenRouterClient(_config);
    }
    
    public void UpdateConfiguration(Action<OpenRouterClientOptions> updateAction)
    {
        lock (_lock)
        {
            var newConfig = _config.Clone();
            updateAction(newConfig);
            
            // Validate new configuration
            newConfig.Validate();
            
            // Dispose old client
            _client?.Dispose();
            
            // Create new client with updated config
            _config = newConfig;
            _client = new OpenRouterClient(_config);
        }
    }
    
    public OpenRouterClient GetClient()
    {
        lock (_lock)
        {
            return _client;
        }
    }
}
```

### Feature Flags

```csharp
public class FeatureFlagConfig
{
    public bool EnableStreaming { get; set; } = true;
    public bool EnableMultimodal { get; set; } = true;
    public bool EnableFallbackModels { get; set; } = true;
    public bool EnableCaching { get; set; } = false;
    public bool EnableLogging { get; set; } = true;
    
    public static FeatureFlagConfig FromConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetSection("Features");
        return new FeatureFlagConfig
        {
            EnableStreaming = bool.Parse(section["Streaming"] ?? "true"),
            EnableMultimodal = bool.Parse(section["Multimodal"] ?? "true"),
            EnableFallbackModels = bool.Parse(section["FallbackModels"] ?? "true"),
            EnableCaching = bool.Parse(section["Caching"] ?? "false"),
            EnableLogging = bool.Parse(section["Logging"] ?? "true")
        };
    }
}
```

## Security Configuration

### Secure API Key Storage

```csharp
public class SecureConfigProvider
{
    // Using Azure Key Vault
    public static async Task<string> GetApiKeyFromKeyVaultAsync()
    {
        var keyVaultUrl = Environment.GetEnvironmentVariable("KEY_VAULT_URL");
        var client = new SecretClient(
            new Uri(keyVaultUrl), 
            new DefaultAzureCredential());
        
        var secret = await client.GetSecretAsync("OpenRouterApiKey");
        return secret.Value.Value;
    }
    
    // Using AWS Secrets Manager
    public static async Task<string> GetApiKeyFromSecretsManagerAsync()
    {
        var client = new AmazonSecretsManagerClient();
        var request = new GetSecretValueRequest
        {
            SecretId = "openrouter-api-key"
        };
        
        var response = await client.GetSecretValueAsync(request);
        return response.SecretString;
    }
    
    // Using environment variable with encryption
    public static string GetEncryptedApiKey()
    {
        var encryptedKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY_ENCRYPTED");
        return DecryptApiKey(encryptedKey);
    }
    
    private static string DecryptApiKey(string encryptedKey)
    {
        // Implement decryption logic
        throw new NotImplementedException();
    }
}
```

## Performance Configuration

### Caching Configuration

```csharp
public class CacheConfiguration
{
    public bool EnableCaching { get; set; }
    public TimeSpan CacheDuration { get; set; }
    public int MaxCacheSize { get; set; }
    
    public static CacheConfiguration Default => new()
    {
        EnableCaching = true,
        CacheDuration = TimeSpan.FromMinutes(5),
        MaxCacheSize = 100
    };
}

public class CachedOpenRouterClient
{
    private readonly OpenRouterClient _client;
    private readonly IMemoryCache _cache;
    private readonly CacheConfiguration _cacheConfig;
    
    public CachedOpenRouterClient(
        OpenRouterClient client, 
        IMemoryCache cache,
        CacheConfiguration cacheConfig)
    {
        _client = client;
        _cache = cache;
        _cacheConfig = cacheConfig;
    }
    
    public async Task<ChatResponse> SendChatCompletionAsync(ChatRequest request)
    {
        if (!_cacheConfig.EnableCaching)
        {
            return await _client.SendChatCompletionAsync(request);
        }
        
        var cacheKey = GenerateCacheKey(request);
        
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SlidingExpiration = _cacheConfig.CacheDuration;
            return await _client.SendChatCompletionAsync(request);
        });
    }
    
    private string GenerateCacheKey(ChatRequest request)
    {
        // Generate unique key based on request parameters
        var json = JsonConvert.SerializeObject(request);
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
        return Convert.ToBase64String(hash);
    }
}
```

## Monitoring Configuration

### Telemetry and Metrics

```csharp
public class TelemetryConfiguration
{
    public bool EnableTelemetry { get; set; }
    public string ApplicationInsightsKey { get; set; }
    public LogLevel MinimumLogLevel { get; set; }
    
    public static void ConfigureTelemetry(
        IServiceCollection services, 
        TelemetryConfiguration config)
    {
        if (config.EnableTelemetry)
        {
            services.AddApplicationInsightsTelemetry(config.ApplicationInsightsKey);
            
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(config.MinimumLogLevel);
                builder.AddApplicationInsights();
                builder.AddConsole();
            });
        }
    }
}
```

## Configuration Best Practices

1. **Never hardcode API keys** - Use environment variables or secure vaults
2. **Validate configuration on startup** - Fail fast with clear error messages
3. **Use different configurations per environment** - Dev, staging, production
4. **Implement configuration hot-reload** - For dynamic updates without restart
5. **Log configuration values** - Except sensitive data like API keys
6. **Set appropriate timeouts** - Based on expected response times
7. **Configure retry policies** - Handle transient failures gracefully
8. **Use connection pooling** - For better performance
9. **Monitor configuration changes** - Audit who changed what and when
10. **Document all configuration options** - Include defaults and valid ranges

## Troubleshooting Configuration Issues

### Common Problems

| Issue | Cause | Solution |
|-------|-------|----------|
| "Invalid API key" | Wrong or missing key | Check environment variables |
| "Timeout" errors | Timeout too short | Increase timeout value |
| Connection failures | Wrong base URL | Verify endpoint URL |
| SSL/TLS errors | Certificate issues | Configure certificate validation |
| Proxy errors | Proxy not configured | Set proxy in HttpClient |

### Configuration Debugging

```csharp
public class ConfigurationDebugger
{
    public static void LogConfiguration(
        OpenRouterClientOptions config, 
        ILogger logger)
    {
        logger.LogInformation("OpenRouter Configuration:");
        logger.LogInformation($"  BaseUrl: {config.BaseUrl}");
        logger.LogInformation($"  DefaultModel: {config.DefaultModel}");
        logger.LogInformation($"  Timeout: {config.Timeout}");
        logger.LogInformation($"  MaxRetries: {config.MaxRetries}");
        logger.LogInformation($"  API Key: {MaskApiKey(config.ApiKey)}");
        
        if (config.DefaultHeaders?.Any() == true)
        {
            logger.LogInformation("  Headers:");
            foreach (var header in config.DefaultHeaders)
            {
                logger.LogInformation($"    {header.Key}: {header.Value}");
            }
        }
    }
    
    private static string MaskApiKey(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey)) return "[NOT SET]";
        if (apiKey.Length <= 8) return "***";
        return $"{apiKey.Substring(0, 4)}...{apiKey.Substring(apiKey.Length - 4)}";
    }
}
```

## Further Reading

- [Getting Started](getting-started.md)
- [API Reference](api-reference.md#openrouterclientoptions)
- [Error Handling](error-handling.md)
- [Examples](examples.md)