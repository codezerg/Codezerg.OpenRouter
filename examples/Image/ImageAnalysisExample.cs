using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;
using Codezerg.OpenRouter.Examples.Core;

namespace Codezerg.OpenRouter.Examples.Image
{
    [Example("Image Analysis", "Analyze local or remote images")]
    public class ImageAnalysisExample : IExample
{
        public async Task RunAsync(OpenRouterClientOptions config)
    {
        Console.WriteLine("\n=== Image Analysis Example ===\n");

        // Clone config and set vision-capable model for this example
        var visionConfig = config.Clone();
        visionConfig.DefaultModel = "openai/gpt-5-mini";

        // Create client with vision-capable model
        using var client = new OpenRouterClient(visionConfig);

        // Use a sample image for non-interactive example
        string imageUrl = "https://images.unsplash.com/photo-1619507938536-39981994f4e9?w=800&auto=webp";
        Console.WriteLine($"Using sample image: {imageUrl}");

        // Use a predefined question
        var question = "Please describe this image in detail, including the scenery, colors, and atmosphere.";
        Console.WriteLine($"\nQuestion: {question}");

        // Create a multimodal message
        var message = new ChatMessage(ChatRole.User)
            .AddText(question)
            .AddImage(imageUrl);

        var request = new ChatRequest
        {
            Messages = new List<ChatMessage> { message },
            MaxTokens = 500
        };

        try
        {
            Console.WriteLine("\nAnalyzing image...\n");
            
            // Send the request
            var response = await client.SendChatCompletionAsync(request);

            // Display the response
            if (response.Choices?.Count > 0)
            {
                var responseChoice = response.Choices[0];
                Console.WriteLine($"Analysis: {responseChoice?.Message?.FirstTextContent}");
            }
            else
            {
                Console.WriteLine("No response received.");
            }

            // Display usage information
            if (response.Usage != null)
            {
                Console.WriteLine($"\nTokens used:");
                Console.WriteLine($"  Prompt: {response.Usage.PromptTokens}");
                Console.WriteLine($"  Completion: {response.Usage.CompletionTokens}");
                Console.WriteLine($"  Total: {response.Usage.TotalTokens}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.Message.Contains("vision") || ex.Message.Contains("image") || ex.Message.Contains("404"))
            {
                Console.WriteLine("\nNote: Make sure you're using a vision-capable model");
            }
        }
    }
    }
}