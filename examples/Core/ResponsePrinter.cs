using System;
using System.Linq;
using Codezerg.OpenRouter.Models;

namespace Codezerg.OpenRouter.Examples.Core
{
    /// <summary>
    /// Helper class for printing standardized responses
    /// </summary>
    public static class ResponsePrinter
    {
        /// <summary>
        /// Prints a chat response with optional usage statistics
        /// </summary>
        public static void PrintChatResponse(ChatResponse response, bool showUsage = true)
        {
            if (response == null)
            {
                ConsoleHelper.Warn("No response received");
                return;
            }

            var content = response.Choices?.FirstOrDefault()?.Message?.FirstTextContent
                         ?? "[No content]";

            Console.WriteLine(content);

            if (showUsage && response.Usage != null)
            {
                PrintUsage(response.Usage);
            }
        }

        /// <summary>
        /// Prints token usage statistics
        /// </summary>
        public static void PrintUsage(TokenUsage usage)
        {
            if (usage == null) return;

            ConsoleHelper.Section("Token Usage");
            Console.WriteLine($"  Prompt Tokens: {usage.PromptTokens:N0}");
            Console.WriteLine($"  Completion Tokens: {usage.CompletionTokens:N0}");
            Console.WriteLine($"  Total Tokens: {usage.TotalTokens:N0}");
        }

        /// <summary>
        /// Prints streaming response tokens
        /// </summary>
        public static void PrintStreamToken(ChatResponse chunk)
        {
            var token = chunk?.Choices?.FirstOrDefault()?.Delta?.Content;
            if (!string.IsNullOrEmpty(token))
            {
                Console.Write(token);
            }
        }

        /// <summary>
        /// Prints model information
        /// </summary>
        public static void PrintModelInfo(ModelInfo model)
        {
            if (model == null) return;

            Console.WriteLine($"\nModel: {model.Name ?? model.Id}");
            Console.WriteLine($"  ID: {model.Id}");

            if (!string.IsNullOrEmpty(model.Description))
                Console.WriteLine($"  Description: {model.Description}");

            if (model.Pricing != null)
            {
                Console.WriteLine($"  Pricing:");
                Console.WriteLine($"    Prompt: ${model.Pricing.Prompt:F6}/token");
                Console.WriteLine($"    Completion: ${model.Pricing.Completion:F6}/token");

                if (!string.IsNullOrEmpty(model.Pricing.Request))
                    Console.WriteLine($"    Request: {model.Pricing.Request}");

                if (!string.IsNullOrEmpty(model.Pricing.Image))
                    Console.WriteLine($"    Image: {model.Pricing.Image}");
            }

            if (model.ContextLength > 0)
                Console.WriteLine($"  Context Length: {model.ContextLength:N0} tokens");

            if (model.TopProvider != null)
            {
                if (model.TopProvider.MaxCompletionTokens > 0)
                    Console.WriteLine($"  Max Output: {model.TopProvider.MaxCompletionTokens:N0} tokens");
                if (model.TopProvider.ContextLength > 0)
                    Console.WriteLine($"  Context Length: {model.TopProvider.ContextLength:N0} tokens");
            }
        }

        /// <summary>
        /// Prints activity information
        /// </summary>
        public static void PrintActivity(Activity activity)
        {
            if (activity == null) return;

            Console.WriteLine($"Date: {activity.Date:yyyy-MM-dd}");
            Console.WriteLine($"  Model: {activity.Model}");
            Console.WriteLine($"  Requests: {activity.Requests:N0}");
        }

        /// <summary>
        /// Prints credits information
        /// </summary>
        public static void PrintCredits(Credits credits)
        {
            if (credits == null) return;

            ConsoleHelper.Section("Account Credits");
            Console.WriteLine($"  Total Credits: ${credits.TotalCredits:F2}");
            Console.WriteLine($"  Total Used: ${credits.TotalUsage:F2}");
            Console.WriteLine($"  Remaining: ${(credits.TotalCredits - credits.TotalUsage):F2}");
        }

        /// <summary>
        /// Prints generation details
        /// </summary>
        public static void PrintGenerationDetails(GenerationDetails generation)
        {
            if (generation == null) return;

            ConsoleHelper.Section("Generation Details");
            Console.WriteLine($"  ID: {generation.Id}");
            Console.WriteLine($"  Created: {generation.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"  Model: {generation.Model}");
            Console.WriteLine($"  Prompt Tokens: {generation.TokensPrompt:N0}");
            Console.WriteLine($"  Completion Tokens: {generation.TokensCompletion:N0}");
            Console.WriteLine($"  Total Cost: ${generation.TotalCost:F6}");
        }
    }
}