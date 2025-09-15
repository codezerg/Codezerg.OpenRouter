using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;
using Codezerg.OpenRouter.Examples.Chat;
using Codezerg.OpenRouter.Examples.Frontend;
using Codezerg.OpenRouter.Examples.Image;

namespace Codezerg.OpenRouter.Examples;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("========================================");
        Console.WriteLine("      Codezerg.OpenRouter Examples");
        Console.WriteLine("========================================");

        // Check for API key - first try user variable, then system variable
        var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY", EnvironmentVariableTarget.User) 
                     ?? Environment.GetEnvironmentVariable("OPENROUTER_API_KEY", EnvironmentVariableTarget.Machine)
                     ?? Environment.GetEnvironmentVariable("OPENROUTER_API_KEY", EnvironmentVariableTarget.Process);
        
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("\nWARNING: OPENROUTER_API_KEY environment variable is not set!");
            Console.WriteLine("Please set it to run the examples:");
            Console.WriteLine("  Windows (User): setx OPENROUTER_API_KEY \"your-api-key-here\"");
            Console.WriteLine("  Windows (System): setx OPENROUTER_API_KEY \"your-api-key-here\" /M");
            Console.WriteLine("  Linux/Mac: export OPENROUTER_API_KEY=your-api-key-here");
            Console.WriteLine("\nGet your API key from: https://openrouter.ai/keys");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return;
        }
        
        Console.WriteLine("✓ API key found");

        // Create shared configuration
        var config = new OpenRouterClientOptions
        {
            ApiKey = apiKey,
            DefaultModel = "deepseek/deepseek-chat-v3:free",
            UserAgent = "Codezerg.OpenRouter.Examples/1.0",
            Referer = "https://github.com/codezerg/Codezerg.OpenRouter"
        };

        await RunExamplesMenu(config);
    }

    static async Task RunExamplesMenu(OpenRouterClientOptions config)
    {
        var examples = new List<(string Name, string Description, Func<OpenRouterClientOptions, Task> Run)>
        {
            //("Simple Chat", "Basic chat completion example", SimpleChatExample.RunAsync),
            //("Streaming Chat", "Chat with streaming response", StreamingChatExample.RunAsync),
            //("Multimodal Chat", "Chat with images", MultimodalChatExample.RunAsync),
            //("Image Analysis", "Analyze local or remote images", ImageAnalysisExample.RunAsync),
            //("Image Generation", "Generate images using AI", ImageGenerationExample.RunAsync),
            //("Model Explorer", "Explore and compare models using frontend API", ModelExplorerExample.RunAsync),
            //("Frontend API Demo", "Full frontend API demonstration", async (cfg) => await FrontendApiExample.RunAsync()),
            //("API Endpoints Demo", "Demonstrate various OpenRouter API endpoints", async (cfg) => await ApiEndpointsExample.RunAsync()),
            ("Cost calculation Demo", "Demonstrating cost calculation for OpenRouter API usage", async (cfg) => await CostCalculationExample.RunAsync()),
        };

        Console.WriteLine("\n========================================");
        Console.WriteLine("Running All Examples");
        Console.WriteLine("========================================\n");

        for (int i = 0; i < examples.Count; i++)
        {
            Console.WriteLine($"\n[{i + 1}/{examples.Count}] Running: {examples[i].Name}");
            Console.WriteLine($"Description: {examples[i].Description}");
            Console.WriteLine("----------------------------------------");
            
            try
            {
                await examples[i].Run(config);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nExample failed with error: {ex.Message}");
            }
            
            if (i < examples.Count - 1)
            {
                Console.WriteLine("\n========================================");
                await Task.Delay(1000); // Brief pause between examples
            }
        }

        Console.WriteLine("\n========================================");
        Console.WriteLine("All examples completed!");
        Console.WriteLine("========================================");
    }
}
