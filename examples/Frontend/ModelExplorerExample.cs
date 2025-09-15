using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Frontend;

namespace Codezerg.OpenRouter.Examples.Frontend
{
    /// <summary>
    /// Example showing how to explore and analyze models using the frontend API
    /// </summary>
    public class ModelExplorerExample
    {
        public static async Task RunAsync(OpenRouterClientOptions config)
        {
            Console.WriteLine("\n=== Model Explorer (Frontend API) ===\n");
            Console.WriteLine("Note: Using undocumented private APIs - may change without notice\n");

            using (var client = new OpenRouterFrontendClient())
            {
                try
                {
                    // 1. Find the best models for different use cases
                    await FindBestModelsForUseCase(client);

                    // 2. Compare model pricing
                    await CompareModelPricing(client);

                    // 3. Analyze provider capabilities
                    await AnalyzeProviderCapabilities(client);

                    // 4. Find models with specific features
                    await FindSpecializedModels(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Frontend API Error: {ex.Message}");
                    Console.WriteLine("The frontend API may be unavailable or changed.");
                }
            }
        }

        private static async Task FindBestModelsForUseCase(OpenRouterFrontendClient client)
        {
            Console.WriteLine("1. Finding Best Models for Different Use Cases");
            Console.WriteLine("=" + new string('=', 48) + "\n");

            // Get top weekly models
            var topModels = await client.FindModelsAsync("top-weekly");

            // Categorize models by use case
            var codingModels = topModels.Where(m =>
                m.Name.Contains("Code", StringComparison.OrdinalIgnoreCase) ||
                m.Name.Contains("Coder", StringComparison.OrdinalIgnoreCase) ||
                m.Description?.Contains("code", StringComparison.OrdinalIgnoreCase) == true)
                .Take(3).ToList();

            var reasoningModels = topModels.Where(m =>
                m.ReasoningConfig != null ||
                m.Endpoint?.SupportsReasoning == true ||
                m.Name.Contains("Reasoning", StringComparison.OrdinalIgnoreCase) ||
                m.Name.Contains("Think", StringComparison.OrdinalIgnoreCase))
                .Take(3).ToList();

            var visionModels = topModels.Where(m =>
                m.InputModalities?.Contains("image") == true)
                .Take(3).ToList();

            var longContextModels = topModels.Where(m =>
                m.ContextLength >= 100000)
                .OrderByDescending(m => m.ContextLength)
                .Take(3).ToList();

            // Display recommendations
            Console.WriteLine("📝 Best for Coding:");
            foreach (var model in codingModels)
            {
                Console.WriteLine($"   • {model.ShortName}");
                Console.WriteLine($"     Context: {model.ContextLength:N0} tokens");
                if (model.Endpoint?.Pricing != null)
                {
                    Console.WriteLine($"     Price: ${model.Endpoint.Pricing.Prompt}/1K input, ${model.Endpoint.Pricing.Completion}/1K output");
                }
            }

            Console.WriteLine("\n🧠 Best for Reasoning:");
            foreach (var model in reasoningModels)
            {
                Console.WriteLine($"   • {model.ShortName}");
                if (model.ReasoningConfig != null)
                {
                    Console.WriteLine($"     Thinking: {model.ReasoningConfig.StartToken} ... {model.ReasoningConfig.EndToken}");
                }
            }

            Console.WriteLine("\n👁️ Best for Vision:");
            foreach (var model in visionModels)
            {
                Console.WriteLine($"   • {model.ShortName}");
                var modalities = string.Join(", ", model.InputModalities ?? new List<string>());
                Console.WriteLine($"     Inputs: {modalities}");
            }

            Console.WriteLine("\n📚 Best for Long Context:");
            foreach (var model in longContextModels)
            {
                Console.WriteLine($"   • {model.ShortName}");
                Console.WriteLine($"     Context: {model.ContextLength:N0} tokens ({model.ContextLength / 1000}K)");
            }

            Console.WriteLine();
        }

        private static async Task CompareModelPricing(OpenRouterFrontendClient client)
        {
            Console.WriteLine("2. Model Pricing Comparison");
            Console.WriteLine("=" + new string('=', 48) + "\n");

            var models = await client.GetModelsAsync();

            // Find free models
            var freeModels = models.Where(m => m.Endpoint?.IsFree == true).Take(5).ToList();

            // Find cheapest paid models
            var paidModels = models
                .Where(m => m.Endpoint?.Pricing != null && !string.IsNullOrEmpty(m.Endpoint.Pricing.Prompt))
                .Select(m => new
                {
                    Model = m,
                    InputPrice = ParsePrice(m.Endpoint.Pricing.Prompt),
                    OutputPrice = ParsePrice(m.Endpoint.Pricing.Completion)
                })
                .Where(m => m.InputPrice > 0)
                .OrderBy(m => m.InputPrice + m.OutputPrice)
                .Take(5)
                .ToList();

            // Find premium models
            var premiumModels = models
                .Where(m => m.Endpoint?.Pricing != null && !string.IsNullOrEmpty(m.Endpoint.Pricing.Prompt))
                .Select(m => new
                {
                    Model = m,
                    InputPrice = ParsePrice(m.Endpoint.Pricing.Prompt),
                    OutputPrice = ParsePrice(m.Endpoint.Pricing.Completion)
                })
                .Where(m => m.InputPrice > 0)
                .OrderByDescending(m => m.InputPrice + m.OutputPrice)
                .Take(5)
                .ToList();

            Console.WriteLine("🆓 Free Models:");
            foreach (var model in freeModels)
            {
                Console.WriteLine($"   • {model.ShortName} ({model.Slug})");
            }

            Console.WriteLine("\n💰 Most Affordable (per 1M tokens):");
            foreach (var item in paidModels)
            {
                var totalCost = (item.InputPrice + item.OutputPrice) * 1000000;
                Console.WriteLine($"   • {item.Model.ShortName}");
                Console.WriteLine($"     Input: ${item.InputPrice * 1000000:F2}, Output: ${item.OutputPrice * 1000000:F2}");
                Console.WriteLine($"     Est. total for 1M tokens: ${totalCost:F2}");
            }

            Console.WriteLine("\n💎 Premium Models (per 1M tokens):");
            foreach (var item in premiumModels)
            {
                var totalCost = (item.InputPrice + item.OutputPrice) * 1000000;
                Console.WriteLine($"   • {item.Model.ShortName}");
                Console.WriteLine($"     Input: ${item.InputPrice * 1000000:F2}, Output: ${item.OutputPrice * 1000000:F2}");
                Console.WriteLine($"     Est. total for 1M tokens: ${totalCost:F2}");
            }

            Console.WriteLine();
        }

        private static async Task AnalyzeProviderCapabilities(OpenRouterFrontendClient client)
        {
            Console.WriteLine("3. Provider Analysis");
            Console.WriteLine("=" + new string('=', 48) + "\n");

            var providers = await client.GetProvidersAsync();

            // Privacy-focused providers
            var privacyProviders = await client.GetPrivacyFriendlyProvidersAsync();

            // BYOK providers
            var byokProviders = await client.GetByokProvidersAsync();

            // Providers with streaming
            var streamingProviders = providers.Where(p => p.IsAbortable).ToList();

            Console.WriteLine($"📊 Provider Statistics:");
            Console.WriteLine($"   • Total providers: {providers.Count}");
            Console.WriteLine($"   • Privacy-focused (no training/retention): {privacyProviders.Count}");
            Console.WriteLine($"   • Support BYOK: {byokProviders.Count}");
            Console.WriteLine($"   • Support streaming/abort: {streamingProviders.Count}");

            Console.WriteLine("\n🔒 Top Privacy-Focused Providers:");
            foreach (var provider in privacyProviders.Take(5))
            {
                Console.WriteLine($"   • {provider.DisplayName}");
                if (!string.IsNullOrEmpty(provider.Headquarters))
                {
                    Console.WriteLine($"     Location: {provider.Headquarters}");
                }
            }

            Console.WriteLine("\n🔑 Providers Supporting BYOK:");
            var majorByok = byokProviders.Where(p =>
                p.Name.Contains("openai", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Contains("anthropic", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Contains("google", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Contains("mistral", StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var provider in majorByok)
            {
                Console.WriteLine($"   • {provider.DisplayName}");
            }

            Console.WriteLine();
        }

        private static async Task FindSpecializedModels(OpenRouterFrontendClient client)
        {
            Console.WriteLine("4. Specialized Model Discovery");
            Console.WriteLine("=" + new string('=', 48) + "\n");

            var models = await client.GetModelsAsync();

            // Models with tool/function calling
            var toolModels = models.Where(m =>
                m.Endpoint?.SupportsToolParameters == true ||
                m.Endpoint?.Features?.SupportsToolChoice != null).Take(5).ToList();

            // Models with web search
            var searchModels = models.Where(m =>
                m.Endpoint?.Pricing?.WebSearch != null &&
                m.Endpoint.Pricing.WebSearch != "0").ToList();

            // Fast models (low latency)
            var fastModels = models
                .Where(m => m.Endpoint?.Stats != null && m.Endpoint.Stats.P50Latency > 0)
                .OrderBy(m => m.Endpoint.Stats.P50Latency)
                .Take(5).ToList();

            // High throughput models
            var throughputModels = models
                .Where(m => m.Endpoint?.Stats != null && m.Endpoint.Stats.P50Throughput > 0)
                .OrderByDescending(m => m.Endpoint.Stats.P50Throughput)
                .Take(5).ToList();

            Console.WriteLine("🔧 Models with Tool/Function Calling:");
            foreach (var model in toolModels)
            {
                Console.WriteLine($"   • {model.ShortName}");
                if (model.Endpoint?.Features?.SupportsToolChoice != null)
                {
                    var tc = model.Endpoint.Features.SupportsToolChoice;
                    var features = new List<string>();
                    if (tc.LiteralAuto) features.Add("auto");
                    if (tc.LiteralRequired) features.Add("required");
                    if (tc.TypeFunction) features.Add("function");
                    Console.WriteLine($"     Supports: {string.Join(", ", features)}");
                }
            }

            if (searchModels.Any())
            {
                Console.WriteLine("\n🔍 Models with Web Search:");
                foreach (var model in searchModels)
                {
                    Console.WriteLine($"   • {model.ShortName}");
                    Console.WriteLine($"     Search cost: ${model.Endpoint.Pricing.WebSearch} per search");
                }
            }

            Console.WriteLine("\n⚡ Fastest Models (lowest latency):");
            if (fastModels.Any())
            {
                foreach (var model in fastModels)
                {
                    Console.WriteLine($"   • {model.ShortName}");
                    Console.WriteLine($"     P50 latency: {model.Endpoint.Stats.P50Latency:F0}ms");
                    Console.WriteLine($"     P50 throughput: {model.Endpoint.Stats.P50Throughput:F1} tokens/sec");
                }
            }
            else
            {
                Console.WriteLine("   No latency statistics available");
            }

            Console.WriteLine("\n🚀 Highest Throughput Models:");
            if (throughputModels.Any())
            {
                foreach (var model in throughputModels)
                {
                    Console.WriteLine($"   • {model.ShortName}");
                    Console.WriteLine($"     P50 throughput: {model.Endpoint.Stats.P50Throughput:F1} tokens/sec");
                    Console.WriteLine($"     P50 latency: {model.Endpoint.Stats.P50Latency:F0}ms");
                }
            }
            else
            {
                Console.WriteLine("   No throughput statistics available");
            }

            Console.WriteLine();
        }

        private static double ParsePrice(string price)
        {
            if (string.IsNullOrEmpty(price)) return 0;
            if (double.TryParse(price, out var result))
                return result;
            return 0;
        }
    }
}