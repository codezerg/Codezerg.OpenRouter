using System;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;

namespace Codezerg.OpenRouter.Examples
{
    /// <summary>
    /// Example demonstrating cost calculation for OpenRouter API usage.
    /// </summary>
    public class CostCalculationExample
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
                ApiKey = apiKey
            };

            using var client = new OpenRouterClient(config);

            try
            {
                Console.WriteLine("=== OpenRouter Cost Calculation Example ===\n");

                // First, get model pricing information
                Console.WriteLine("Fetching model pricing information...");
                var models = await client.GetModelsAsync();

                var model_id = "openai/gpt-5-chat";

                // Find GPT-3.5 Turbo pricing
                var model = models.Find(m => m.Id == model_id);
                if (model?.Pricing != null)
                {
                    Console.WriteLine($"Model: {model.Name}");
                    Console.WriteLine($"Prompt price: ${model.Pricing.Prompt} per token");
                    Console.WriteLine($"Completion price: ${model.Pricing.Completion} per token\n");
                }

                // Create a chat request
                var chatRequest = new ChatRequest
                {
                    Model = model_id,
                    Messages = new()
                    {
                        ChatMessage.System("You are a helpful assistant that provides concise answers."),
                        ChatMessage.User("Explain the concept of recursion in programming in 2-3 sentences.")
                    }
                };

                Console.WriteLine("Sending chat completion request...");
                var response = await client.SendChatCompletionAsync(chatRequest);

                Console.WriteLine("\n=== Response ===");
                Console.WriteLine($"Answer: {response.Choices[0].Message?.FirstTextContent}");
                Console.WriteLine($"Generation ID: {response.Id}");

                // Get token usage from response
                if (response.Usage != null)
                {
                    Console.WriteLine("\n=== Token Usage (from response) ===");
                    Console.WriteLine($"Prompt tokens: {response.Usage.PromptTokens}");
                    Console.WriteLine($"Completion tokens: {response.Usage.CompletionTokens}");
                    Console.WriteLine($"Total tokens: {response.Usage.TotalTokens}");
                }

                // Wait for generation details to be available
                await Task.Delay(1500);

                // Get detailed cost information from generation endpoint
                Console.WriteLine("\n=== Detailed Cost Information ===");
                try
                {
                    var generationDetails = await client.GetGenerationAsync(response.Id);

                    Console.WriteLine($"Provider: {generationDetails.ProviderName}");
                    Console.WriteLine($"Model: {generationDetails.Model}");
                    Console.WriteLine($"Streamed: {generationDetails.Streamed}");
                    Console.WriteLine($"Latency: {generationDetails.Latency}ms");
                    Console.WriteLine($"Generation time: {generationDetails.GenerationTime}ms");

                    Console.WriteLine("\n=== Token Counts ===");
                    Console.WriteLine($"Prompt tokens: {generationDetails.TokensPrompt}");
                    Console.WriteLine($"Completion tokens: {generationDetails.TokensCompletion}");
                    Console.WriteLine($"Native prompt tokens: {generationDetails.NativeTokensPrompt}");
                    Console.WriteLine($"Native completion tokens: {generationDetails.NativeTokensCompletion}");
                    if (generationDetails.NativeTokensReasoning > 0)
                    {
                        Console.WriteLine($"Reasoning tokens: {generationDetails.NativeTokensReasoning}");
                    }
                    if (generationDetails.NativeTokensCached.HasValue && generationDetails.NativeTokensCached > 0)
                    {
                        Console.WriteLine($"Cached tokens: {generationDetails.NativeTokensCached}");
                    }

                    Console.WriteLine("\n=== Cost Breakdown ===");
                    Console.WriteLine($"Total cost: ${generationDetails.TotalCost:F8}");
                    Console.WriteLine($"Usage cost: ${generationDetails.Usage:F8}");
                    Console.WriteLine($"Upstream inference cost: ${generationDetails.UpstreamInferenceCost:F8}");
                    if (generationDetails.CacheDiscount.HasValue && generationDetails.CacheDiscount > 0)
                    {
                        Console.WriteLine($"Cache discount: ${generationDetails.CacheDiscount:F8}");
                    }

                    // Manual cost calculation
                    if (model?.Pricing != null)
                    {
                        Console.WriteLine("\n=== Manual Cost Calculation ===");

                        // Parse pricing strings to doubles
                        if (double.TryParse(model.Pricing.Prompt, out double promptPrice) &&
                            double.TryParse(model.Pricing.Completion, out double completionPrice))
                        {
                            double promptCost = generationDetails.TokensPrompt * promptPrice;
                            double completionCost = generationDetails.TokensCompletion * completionPrice;
                            double totalCalculatedCost = promptCost + completionCost;

                            Console.WriteLine($"Prompt cost: {generationDetails.TokensPrompt} tokens × ${promptPrice:F10}/token = ${promptCost:F8}");
                            Console.WriteLine($"Completion cost: {generationDetails.TokensCompletion} tokens × ${completionPrice:F10}/token = ${completionCost:F8}");
                            Console.WriteLine($"Total calculated cost: ${totalCalculatedCost:F8}");
                            Console.WriteLine($"Actual charged cost: ${generationDetails.TotalCost:F8}");

                            double difference = Math.Abs(totalCalculatedCost - generationDetails.TotalCost);
                            if (difference < 0.00000001)
                            {
                                Console.WriteLine("✓ Calculated cost matches actual cost!");
                            }
                            else
                            {
                                Console.WriteLine($"Difference: ${difference:F8}");
                            }
                        }
                    }
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("404"))
                {
                    Console.WriteLine("Generation details not available. The API may need more time to process.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting generation details: {ex.Message}");
                }

                // Example: Calculate costs for different models
                Console.WriteLine("\n=== Cost Comparison for Different Models ===");

                var modelsToCompare = new[]
                {
                    "openai/gpt-3.5-turbo",
                    "openai/gpt-4",
                    "anthropic/claude-3-haiku",
                    "anthropic/claude-3-sonnet",
                    "google/gemini-flash-1.5",
                    "meta-llama/llama-3-8b-instruct"
                };

                // Assume 1000 prompt tokens and 500 completion tokens
                int examplePromptTokens = 1000;
                int exampleCompletionTokens = 500;

                foreach (var modelId in modelsToCompare)
                {
                    var modelOther = models.Find(m => m.Id == modelId);
                    if (modelOther?.Pricing != null)
                    {
                        if (double.TryParse(modelOther.Pricing.Prompt, out double promptPrice) &&
                            double.TryParse(modelOther.Pricing.Completion, out double completionPrice))
                        {
                            double cost = (examplePromptTokens * promptPrice) + (exampleCompletionTokens * completionPrice);
                            Console.WriteLine($"{modelOther.Name,-40} ${cost:F6}");
                        }
                    }
                }

                Console.WriteLine($"\n(Based on {examplePromptTokens} prompt tokens and {exampleCompletionTokens} completion tokens)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}