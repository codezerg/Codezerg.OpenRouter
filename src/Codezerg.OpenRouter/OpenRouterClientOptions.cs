using System;

namespace Codezerg.OpenRouter;

/// <summary>
/// Options for configuring the <see cref="OpenRouterClient"/>.
/// </summary>
public class OpenRouterClientOptions
{
    /// <summary>
    /// The API key used to authenticate requests to OpenRouter.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The API endpoint URL (defaults to the OpenRouter API endpoint).
    /// </summary>
    public string Endpoint { get; set; } = "https://openrouter.ai/api/v1";

    /// <summary>
    /// The default model used for requests, unless overridden per request.
    /// </summary>
    public string DefaultModel { get; set; } = "deepseek/deepseek-chat-v3.1:free";

    /// <summary>
    /// Timeout for API requests (default: 100 seconds).
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);

    /// <summary>
    /// The required User-Agent string for OpenRouter attribution.
    /// Example: "my-app/1.0" or "github.com/myuser/myrepo".
    /// </summary>
    public string UserAgent { get; set; } = string.Empty;

    /// <summary>
    /// The required "HTTP-Referer" header for OpenRouter attribution.
    /// Example: "https://myapp.com".
    /// </summary>
    public string Referer { get; set; } = string.Empty;

    /// <summary>
    /// Whether requests should log debug information.
    /// </summary>
    public bool EnableDebugLogging { get; set; } = false;
}