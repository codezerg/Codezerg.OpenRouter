using System;
using System.Text.Json;
using Codezerg.OpenRouter.Models;

namespace Codezerg.OpenRouter.Examples.Core
{
    /// <summary>
    /// Utility for logging raw JSON requests and responses
    /// </summary>
    public static class RequestLogger
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Logs a chat request in JSON format
        /// </summary>
        public static void LogRequest(ChatRequest request, string model = null)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n=== REQUEST ===");
            Console.ResetColor();

            if (!string.IsNullOrEmpty(model))
                Console.WriteLine($"Model: {model}");

            try
            {
                var json = JsonSerializer.Serialize(request, JsonOptions);
                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error serializing request: {ex.Message}]");
            }
        }

        /// <summary>
        /// Logs a chat response in JSON format
        /// </summary>
        public static void LogResponse(ChatResponse response)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n=== RESPONSE ===");
            Console.ResetColor();

            try
            {
                var json = JsonSerializer.Serialize(response, JsonOptions);
                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error serializing response: {ex.Message}]");
            }
        }

        /// <summary>
        /// Logs raw JSON string
        /// </summary>
        public static void LogRawJson(string label, string json)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n=== {label.ToUpper()} ===");
            Console.ResetColor();

            try
            {
                // Try to format the JSON if it's valid
                var parsed = JsonDocument.Parse(json);
                var formatted = JsonSerializer.Serialize(parsed, JsonOptions);
                Console.WriteLine(formatted);
            }
            catch
            {
                // If parsing fails, just print as-is
                Console.WriteLine(json);
            }
        }

        /// <summary>
        /// Logs an API error
        /// </summary>
        public static void LogError(string operation, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n=== ERROR in {operation} ===");
            Console.ResetColor();

            Console.WriteLine($"Type: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }

        /// <summary>
        /// Logs timing information
        /// </summary>
        public static void LogTiming(string operation, TimeSpan duration)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n[TIMING] {operation}: {duration.TotalSeconds:F2} seconds");
            Console.ResetColor();
        }

        /// <summary>
        /// Logs token estimation
        /// </summary>
        public static void LogTokenEstimate(int estimatedTokens, string context)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"[TOKENS] Estimated {estimatedTokens:N0} tokens for {context}");
            Console.ResetColor();
        }
    }
}