using System;

namespace Codezerg.OpenRouter;

/// <summary>
/// Extension methods for <see cref="OpenRouterClientOptions"/>.
/// Provides validation, cloning, and fluent-style configuration.
/// </summary>
public static class OpenRouterClientOptionsExtensions
{
    /// <summary>
    /// Validates that the <see cref="OpenRouterClientOptions"/> instance has the required properties set.
    /// Throws <see cref="InvalidOperationException"/> if validation fails.
    /// </summary>
    public static void Validate(this OpenRouterClientOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(options.ApiKey))
            throw new InvalidOperationException("The API key must be provided in OpenRouterOptions.ApiKey.");

        if (string.IsNullOrWhiteSpace(options.Endpoint))
            throw new InvalidOperationException("The API endpoint must be provided in OpenRouterOptions.Endpoint.");

        if (string.IsNullOrWhiteSpace(options.DefaultModel))
            throw new InvalidOperationException("The default model must be provided in OpenRouterOptions.DefaultModel.");
    }

    /// <summary>
    /// Creates a shallow copy of the options instance.
    /// </summary>
    public static OpenRouterClientOptions Clone(this OpenRouterClientOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        return new OpenRouterClientOptions
        {
            ApiKey = options.ApiKey,
            Endpoint = options.Endpoint,
            DefaultModel = options.DefaultModel,
            Timeout = options.Timeout,
            UserAgent = options.UserAgent,
            Referer = options.Referer,
            EnableDebugLogging = options.EnableDebugLogging
        };
    }

    // ================
    // Fluent Setters
    // ================

    public static OpenRouterClientOptions WithApiKey(this OpenRouterClientOptions options, string apiKey)
    {
        options.ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        return options;
    }

    public static OpenRouterClientOptions WithEndpoint(this OpenRouterClientOptions options, string endpoint)
    {
        options.Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        return options;
    }

    public static OpenRouterClientOptions WithDefaultModel(this OpenRouterClientOptions options, string defaultModel)
    {
        options.DefaultModel = defaultModel ?? throw new ArgumentNullException(nameof(defaultModel));
        return options;
    }

    public static OpenRouterClientOptions WithTimeout(this OpenRouterClientOptions options, TimeSpan timeout)
    {
        options.Timeout = timeout;
        return options;
    }

    public static OpenRouterClientOptions WithUserAgent(this OpenRouterClientOptions options, string userAgent)
    {
        options.UserAgent = userAgent ?? throw new ArgumentNullException(nameof(userAgent));
        return options;
    }

    public static OpenRouterClientOptions WithReferer(this OpenRouterClientOptions options, string referer)
    {
        options.Referer = referer ?? throw new ArgumentNullException(nameof(referer));
        return options;
    }

    public static OpenRouterClientOptions WithEnableDebugLogging(this OpenRouterClientOptions options, bool enableDebugLogging = true)
    {
        options.EnableDebugLogging = enableDebugLogging;
        return options;
    }
}