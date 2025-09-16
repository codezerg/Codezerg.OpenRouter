using System;
using System.Linq;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;

namespace Codezerg.OpenRouter.Examples
{
    /// <summary>
    /// Example demonstrating usage of various OpenRouter API endpoints.
    /// </summary>
    public class ApiEndpointsExample
    {
        public static async Task RunAsync()
        {
            // Get API key from environment variable
            var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Please set the OPENROUTER_API_KEY environment variable.");
                return;
            }

            // Create client with configuration
            var config = new OpenRouterClientOptions
            {
                ApiKey = apiKey,
                DefaultModel = "openai/gpt-3.5-turbo"
            };

            using var client = new OpenRouterClient(config);

            try
            {
                // Example 1: Get credit balance
                Console.WriteLine("=== Credit Balance ===");
                try
                {
                    var credits = await client.GetCreditsAsync();
                    Console.WriteLine($"Total Credits: ${credits.TotalCredits}");
                    Console.WriteLine($"Total Usage: ${credits.TotalUsage}");
                    Console.WriteLine($"Remaining: ${credits.TotalCredits - credits.TotalUsage}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Credits API Error: {ex.Message}");
                }
                Console.WriteLine();

                // Example 2: Get list of providers
                Console.WriteLine("=== Available Providers ===");
                var providers = await client.GetProvidersAsync();
                foreach (var provider in providers)
                {
                    Console.WriteLine($"- {provider.Name} ({provider.Slug})");
                    if (!string.IsNullOrEmpty(provider.StatusPageUrl))
                        Console.WriteLine($"  Status: {provider.StatusPageUrl}");
                }
                Console.WriteLine();

                // Example 3: Get available models
                Console.WriteLine("=== Available Models (Top 10) ===");
                var models = await client.GetModelsAsync();
                var topModels = models.Take(10);
                foreach (var model in topModels)
                {
                    Console.WriteLine($"- {model.Name} (ID: {model.Id})");
                    if (model.Pricing != null)
                    {
                        Console.WriteLine($"  Pricing - Prompt: ${model.Pricing.Prompt}/token, Completion: ${model.Pricing.Completion}/token");
                    }
                    if (model.ContextLength.HasValue)
                    {
                        Console.WriteLine($"  Context Length: {model.ContextLength:N0} tokens");
                    }
                }
                Console.WriteLine();

                // Example 4: Get models filtered by category
                Console.WriteLine("=== Programming Models ===");
                var programmingModels = await client.GetModelsAsync(category: "programming");
                var topProgrammingModels = programmingModels.Take(5);
                foreach (var model in topProgrammingModels)
                {
                    Console.WriteLine($"- {model.Name}");
                    if (model.Architecture != null)
                    {
                        Console.WriteLine($"  Input: {string.Join(", ", model.Architecture.InputModalities)}");
                        Console.WriteLine($"  Output: {string.Join(", ", model.Architecture.OutputModalities)}");
                    }
                }
                Console.WriteLine();

                // Example 5: Get model endpoints (example with Claude)
                Console.WriteLine("=== Model Endpoints Example ===");
                try
                {
                    var endpoints = await client.GetModelEndpointsAsync("anthropic", "claude-3-sonnet");
                    Console.WriteLine($"Model: {endpoints.Name}");
                    Console.WriteLine($"Description: {endpoints.Description}");
                    Console.WriteLine("Endpoints:");
                    foreach (var endpoint in endpoints.Endpoints)
                    {
                        Console.WriteLine($"- Provider: {endpoint.ProviderName}");
                        Console.WriteLine($"  Status: {endpoint.Status}");
                        Console.WriteLine($"  Context: {endpoint.ContextLength:N0} tokens");
                        if (endpoint.UptimeLast30m.HasValue)
                            Console.WriteLine($"  Uptime (30m): {endpoint.UptimeLast30m:P}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Model endpoints error: {ex.Message}");
                }
                Console.WriteLine();

                // Example 6: Get activity data (requires provisioning key)
                Console.WriteLine("=== Activity Data ===");
                try
                {
                    var activity = await client.GetActivityAsync();
                    if (activity.Any())
                    {
                        Console.WriteLine("Recent activity:");
                        foreach (var item in activity.Take(5))
                        {
                            Console.WriteLine($"- Date: {item.Date}");
                            Console.WriteLine($"  Model: {item.Model}");
                            Console.WriteLine($"  Provider: {item.ProviderName}");
                            Console.WriteLine($"  Requests: {item.Requests}");
                            Console.WriteLine($"  Usage: ${item.Usage:F4}");
                            Console.WriteLine($"  Tokens: {item.PromptTokens} prompt, {item.CompletionTokens} completion");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No activity data available.");
                    }
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("403"))
                {
                    Console.WriteLine("Activity API requires a provisioning key (not available with standard API key).");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Activity API Error: {ex.Message}");
                }
                Console.WriteLine();

                // Example 7: Get user models (filtered by preferences)
                Console.WriteLine("=== User Models (Filtered) ===");
                try
                {
                    var userModels = await client.GetUserModelsAsync();
                    Console.WriteLine($"Found {userModels.Count} models based on your preferences.");
                    var topUserModels = userModels.Take(5);
                    foreach (var model in topUserModels)
                    {
                        Console.WriteLine($"- {model.Name}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"User models error: {ex.Message}");
                }
                Console.WriteLine();

                // Example 8: Get generation details (requires a valid generation ID)
                Console.WriteLine("=== Generation Details ===");
                Console.WriteLine("Note: Generation details are only available for a limited time after creation.");
                Console.WriteLine("To get generation details, you need a valid generation ID from a recent chat completion.");
                Console.WriteLine("Example usage:");
                Console.WriteLine("var generation = await client.GetGenerationAsync(\"gen_abc123xyz\");");
                Console.WriteLine();

                // Example 9: Send a chat completion and get its ID
                Console.WriteLine("=== Chat Completion with Generation ID ===");
                var chatRequest = new ChatRequest
                {
                    Model = "openai/gpt-3.5-turbo",
                    Messages = new()
                    {
                        ChatMessage.System("You are a helpful assistant."),
                        ChatMessage.User("What is 2+2?")
                    }
                };

                var response = await client.SendChatCompletionAsync(chatRequest);
                Console.WriteLine($"Response: {response.Choices[0].Message?.FirstTextContent}");
                Console.WriteLine($"Generation ID: {response.Id}");

                // Now retrieve generation details using the ID
                // Note: Generation details may take a moment to become available
                if (!string.IsNullOrEmpty(response.Id))
                {
                    // Wait a moment for the generation details to be available
                    await Task.Delay(1000);

                    try
                    {
                        var generationDetails = await client.GetGenerationAsync(response.Id);
                        Console.WriteLine($"Generation Cost: ${generationDetails.TotalCost:F8}");
                        Console.WriteLine($"Latency: {generationDetails.Latency}ms");
                        Console.WriteLine($"Provider: {generationDetails.ProviderName}");
                        Console.WriteLine($"Tokens - Prompt: {generationDetails.TokensPrompt}, Completion: {generationDetails.TokensCompletion}");
                    }
                    catch (HttpRequestException ex) when (ex.Message.Contains("404"))
                    {
                        Console.WriteLine($"Generation details not available yet (may need more time)");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Generation details error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}