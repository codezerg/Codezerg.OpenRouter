using System;
using System.Linq;
using System.Threading.Tasks;
using Codezerg.OpenRouter;

namespace Codezerg.OpenRouter.Examples
{
    /// <summary>
    /// Example demonstrating the use of OpenRouter's frontend/private API endpoints.
    /// WARNING: These are undocumented private APIs that may change without notice.
    /// </summary>
    public class FrontendApiExample
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== OpenRouter Frontend API Examples ===\n");
            Console.WriteLine("WARNING: These examples use undocumented private APIs that may change without notice.\n");

            // Create frontend client (API key not required for frontend endpoints)
            using (var client = new OpenRouterFrontendClient())
            {
                try
                {
                    // Example 1: Get all providers
                    Console.WriteLine("1. Fetching all providers...");
                    var providers = await client.GetProvidersAsync();
                    Console.WriteLine($"   Found {providers.Count} providers");

                    // Show some interesting provider details
                    var byokProviders = providers.Where(p => p.ByokEnabled).ToList();
                    Console.WriteLine($"   - BYOK-enabled providers: {byokProviders.Count}");

                    var moderationProviders = providers.Where(p => p.ModerationRequired).ToList();
                    Console.WriteLine($"   - Providers requiring moderation: {moderationProviders.Count}");

                    Console.WriteLine($"   - Sample providers: {string.Join(", ", providers.Take(5).Select(p => p.DisplayName))}\n");

                    // Example 2: Get top weekly models
                    Console.WriteLine("2. Fetching top weekly models...");
                    var topModels = await client.FindModelsAsync("top-weekly");
                    Console.WriteLine($"   Found {topModels.Count} models");

                    if (topModels.Any())
                    {
                        Console.WriteLine("   Top 5 models by weekly usage:");
                        foreach (var model in topModels.Take(5))
                        {
                            var pricing = model.Endpoint?.Pricing;
                            Console.WriteLine($"   - {model.ShortName} ({model.Slug})");
                            if (pricing != null)
                            {
                                Console.WriteLine($"     Input: ${pricing.Prompt}/token, Output: ${pricing.Completion}/token");
                            }
                        }
                    }
                    Console.WriteLine();

                    // Example 3: Get reasoning models
                    Console.WriteLine("3. Fetching reasoning-capable models...");
                    var reasoningModels = await client.GetReasoningModelsAsync();
                    Console.WriteLine($"   Found {reasoningModels.Count} reasoning models");

                    if (reasoningModels.Any())
                    {
                        Console.WriteLine("   Sample reasoning models:");
                        foreach (var model in reasoningModels.Take(5))
                        {
                            Console.WriteLine($"   - {model.ShortName}");
                            if (model.ReasoningConfig != null)
                            {
                                Console.WriteLine($"     Thinking tokens: {model.ReasoningConfig.StartToken} ... {model.ReasoningConfig.EndToken}");
                            }
                        }
                    }
                    Console.WriteLine();

                    // Example 4: Get free models
                    Console.WriteLine("4. Fetching free models...");
                    var freeModels = await client.GetFreeModelsAsync();
                    Console.WriteLine($"   Found {freeModels.Count} free models");

                    if (freeModels.Any())
                    {
                        Console.WriteLine("   Free models available:");
                        foreach (var model in freeModels.Take(10))
                        {
                            Console.WriteLine($"   - {model.ShortName} ({model.Slug})");
                        }
                    }
                    Console.WriteLine();

                    // Example 5: Get model statistics
                    Console.WriteLine("5. Fetching model statistics...");
                    var modelSlug = "openai/gpt-4o-mini";
                    var stats = await client.GetModelStatsAsync(modelSlug);

                    if (stats != null)
                    {
                        Console.WriteLine($"   Stats for {modelSlug}:");
                        Console.WriteLine($"   - Context length: {stats.ContextLength:N0} tokens");

                        if (stats.Stats != null)
                        {
                            Console.WriteLine($"   - P50 throughput: {stats.Stats.P50Throughput:F1} tokens/sec");
                            Console.WriteLine($"   - P50 latency: {stats.Stats.P50Latency:F0} ms");
                            Console.WriteLine($"   - Request count: {stats.Stats.RequestCount:N0}");
                        }

                        if (stats.Pricing != null)
                        {
                            Console.WriteLine($"   - Pricing: Input ${stats.Pricing.Prompt}, Output ${stats.Pricing.Completion}");
                        }
                    }
                    Console.WriteLine();

                    // Example 6: Find vision models
                    Console.WriteLine("6. Finding vision-capable models...");
                    var allModels = await client.GetModelsAsync();
                    var visionModels = allModels.Where(m =>
                        m.InputModalities?.Contains("image") == true).ToList();

                    Console.WriteLine($"   Found {visionModels.Count} vision models");
                    if (visionModels.Any())
                    {
                        Console.WriteLine("   Sample vision models:");
                        foreach (var model in visionModels.Take(5))
                        {
                            var modalities = string.Join(", ", model.InputModalities ?? new System.Collections.Generic.List<string>());
                            Console.WriteLine($"   - {model.ShortName}: {modalities}");
                        }
                    }
                    Console.WriteLine();

                    // Example 7: Analyze provider data policies
                    Console.WriteLine("7. Analyzing provider data policies...");
                    var privacyFriendly = await client.GetPrivacyFriendlyProvidersAsync();

                    Console.WriteLine($"   Privacy-friendly providers (no training, no retention): {privacyFriendly.Count}");
                    foreach (var provider in privacyFriendly.Take(5))
                    {
                        Console.WriteLine($"   - {provider.DisplayName}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine("\nNote: The frontend API may be unavailable or may have changed.");
                    Console.WriteLine("These are undocumented private APIs and should not be used in production.");
                }
            }

            Console.WriteLine("\n=== Frontend API Examples Complete ===");
        }
    }
}