using System;

namespace Codezerg.OpenRouter.Examples.Core
{
    /// <summary>
    /// Factory for creating OpenRouter configuration and client instances
    /// </summary>
    public static class OpenRouterFactory
    {
        /// <summary>
        /// Creates default configuration from environment variables
        /// </summary>
        public static OpenRouterClientOptions CreateDefaultConfig()
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException(
                    "OPENROUTER_API_KEY environment variable is not set. " +
                    "Please set it to your OpenRouter API key.");

            return new OpenRouterClientOptions
            {
                ApiKey = apiKey,
                DefaultModel = "deepseek/deepseek-chat-v3.1:free",
                UserAgent = "Codezerg.OpenRouter.Examples/1.0",
                Referer = "https://github.com/codezerg/Codezerg.OpenRouter"
            };
        }

        /// <summary>
        /// Creates a client with default configuration
        /// </summary>
        public static OpenRouterClient CreateClient()
        {
            return new OpenRouterClient(CreateDefaultConfig());
        }

        /// <summary>
        /// Creates a client with custom configuration
        /// </summary>
        public static OpenRouterClient CreateClient(OpenRouterClientOptions config)
        {
            return new OpenRouterClient(config);
        }
    }
}