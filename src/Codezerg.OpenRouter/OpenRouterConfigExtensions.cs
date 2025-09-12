using System;

namespace Codezerg.OpenRouter;

/// <summary>
/// Extension methods for <see cref="OpenRouterConfig"/>.
/// Provides validation, cloning, and fluent-style configuration.
/// </summary>
public static class OpenRouterConfigExtensions
{
    /// <summary>
    /// Validates that the <see cref="OpenRouterConfig"/> instance has the required properties set.
    /// Throws <see cref="InvalidOperationException"/> if validation fails.
    /// </summary>
    public static void Validate(this OpenRouterConfig options)
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
    public static OpenRouterConfig Clone(this OpenRouterConfig options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        return new OpenRouterConfig
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

    public static OpenRouterConfig WithApiKey(this OpenRouterConfig options, string apiKey)
    {
        options.ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        return options;
    }

    public static OpenRouterConfig WithEndpoint(this OpenRouterConfig options, string endpoint)
    {
        options.Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        return options;
    }

    public static OpenRouterConfig WithDefaultModel(this OpenRouterConfig options, string defaultModel)
    {
        options.DefaultModel = defaultModel ?? throw new ArgumentNullException(nameof(defaultModel));
        return options;
    }

    public static OpenRouterConfig WithTimeout(this OpenRouterConfig options, TimeSpan timeout)
    {
        options.Timeout = timeout;
        return options;
    }

    public static OpenRouterConfig WithUserAgent(this OpenRouterConfig options, string userAgent)
    {
        options.UserAgent = userAgent ?? throw new ArgumentNullException(nameof(userAgent));
        return options;
    }

    public static OpenRouterConfig WithReferer(this OpenRouterConfig options, string referer)
    {
        options.Referer = referer ?? throw new ArgumentNullException(nameof(referer));
        return options;
    }

    public static OpenRouterConfig WithEnableDebugLogging(this OpenRouterConfig options, bool enableDebugLogging = true)
    {
        options.EnableDebugLogging = enableDebugLogging;
        return options;
    }
}