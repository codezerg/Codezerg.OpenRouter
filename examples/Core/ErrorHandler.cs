using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Codezerg.OpenRouter.Examples.Core
{
    /// <summary>
    /// Common error handling patterns for OpenRouter API
    /// </summary>
    public static class ErrorHandler
    {
        /// <summary>
        /// Handles common API errors with user-friendly messages
        /// </summary>
        public static async Task<T> HandleApiCall<T>(Func<Task<T>> apiCall, string operation)
        {
            try
            {
                return await apiCall();
            }
            catch (HttpRequestException httpEx)
            {
                HandleHttpError(httpEx, operation);
                throw;
            }
            catch (TaskCanceledException)
            {
                ConsoleHelper.Error($"Request timeout during {operation}. The operation took too long to complete.");
                ConsoleHelper.Info("Try reducing the request size or using a simpler model.");
                throw;
            }
            catch (Exception ex)
            {
                HandleGenericError(ex, operation);
                throw;
            }
        }

        /// <summary>
        /// Handles HTTP errors with specific status codes
        /// </summary>
        private static void HandleHttpError(HttpRequestException ex, string operation)
        {
            ConsoleHelper.Error($"HTTP error during {operation}: {ex.Message}");

            if (ex.Message.Contains("401"))
            {
                ConsoleHelper.Error("Authentication failed - Invalid API key");
                ConsoleHelper.Info("Please check your OPENROUTER_API_KEY environment variable");
            }
            else if (ex.Message.Contains("403"))
            {
                ConsoleHelper.Error("Access forbidden - API key may lack necessary permissions");
                ConsoleHelper.Info("Check if the model requires specific access or credits");
            }
            else if (ex.Message.Contains("404"))
            {
                ConsoleHelper.Error("Resource not found");
                ConsoleHelper.Info("The requested model or endpoint may not exist");
            }
            else if (ex.Message.Contains("429"))
            {
                ConsoleHelper.Error("Rate limit exceeded");
                ConsoleHelper.Info("Please wait before making more requests");
            }
            else if (ex.Message.Contains("500") || ex.Message.Contains("502") || ex.Message.Contains("503"))
            {
                ConsoleHelper.Error("OpenRouter server error");
                ConsoleHelper.Info("The service may be temporarily unavailable. Please try again later.");
            }
        }

        /// <summary>
        /// Handles generic errors
        /// </summary>
        private static void HandleGenericError(Exception ex, string operation)
        {
            ConsoleHelper.Error($"Unexpected error during {operation}: {ex.Message}");

            if (ex.Message.Contains("connection") || ex.Message.Contains("network"))
            {
                ConsoleHelper.Info("Check your internet connection and try again");
            }
            else if (ex.Message.Contains("JSON") || ex.Message.Contains("deserialization"))
            {
                ConsoleHelper.Info("The API response format may have changed");
            }
        }

        /// <summary>
        /// Retries an operation with exponential backoff
        /// </summary>
        public static async Task<T> RetryWithBackoff<T>(
            Func<Task<T>> operation,
            int maxRetries = 3,
            int baseDelayMs = 1000)
        {
            Exception lastException = null;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    return await operation();
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("429") || ex.Message.Contains("503"))
                {
                    lastException = ex;
                    if (i < maxRetries - 1)
                    {
                        var delay = baseDelayMs * Math.Pow(2, i);
                        ConsoleHelper.Info($"Rate limited. Retrying in {delay / 1000:F1} seconds...");
                        await Task.Delay(TimeSpan.FromMilliseconds(delay));
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    throw;
                }
            }

            throw new Exception($"Operation failed after {maxRetries} retries", lastException);
        }

        /// <summary>
        /// Validates API key is set
        /// </summary>
        public static bool ValidateApiKey()
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");

            // For testing purposes, accept a test key
            if (apiKey == "test-key-for-demo")
            {
                ConsoleHelper.Warn("Using test API key - API calls will not work");
                return true;
            }

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                ConsoleHelper.Error("OPENROUTER_API_KEY environment variable is not set");
                Console.WriteLine("\nTo set your API key:");
                Console.WriteLine("  Windows (PowerShell): $env:OPENROUTER_API_KEY=\"your-key\"");
                Console.WriteLine("  Linux/Mac: export OPENROUTER_API_KEY=\"your-key\"");
                Console.WriteLine("\nGet your API key at: https://openrouter.ai/keys");
                return false;
            }

            if (!apiKey.StartsWith("sk-or-"))
            {
                ConsoleHelper.Warn("API key doesn't start with 'sk-or-' - it may be invalid");
            }

            return true;
        }
    }
}