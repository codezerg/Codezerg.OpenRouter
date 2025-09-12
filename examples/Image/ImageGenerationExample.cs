using System;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;

namespace Codezerg.OpenRouter.Examples.Image;

[Example("Image Generation", "Generate images using AI models")]
public class ImageGenerationExample
{
    public static async Task RunAsync(OpenRouterConfig config)
    {
        Console.WriteLine("\n=== Image Generation Example ===\n");

        // Clone config and set image generation model for this example
        var imageGenConfig = config.Clone();
        imageGenConfig.DefaultModel = ModelConstants.Google.Gemini25FlashImagePreview; // OpenRouter's image generation model

        // Create client with image generation model
        using var client = new OpenRouterClient(imageGenConfig);

        // Use a predefined prompt for non-interactive example
        var prompt = "A beautiful sunset over mountains with a lake in the foreground, painted in impressionist style";
        Console.WriteLine($"Prompt: {prompt}");

        // Create an image generation request
        var request = new ChatCompletionRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.User(prompt)
            },
            Modalities = new List<string> { "text", "image" } // Request image output
        };

        try
        {
            Console.WriteLine($"\nGenerating image for: \"{prompt}\"");
            Console.WriteLine("Please wait, this may take a moment...\n");
            
            // Send the request
            var response = await client.SendChatCompletionAsync(request);

            // Check for generated images in the response
            if (response.Choices?.Count > 0)
            {
                var choice = response.Choices[0];
                
                // Check for generated images
                if (choice?.Message?.Images != null && choice.Message.Images.Count > 0)
                {
                    Console.WriteLine("Image(s) generated successfully!");
                    foreach (var image in choice.Message.Images)
                    {
                        Console.WriteLine($"Image URL: {image.ImageUrl?.Url}");
                    }
                }
                
                // Also check for text response
                if (!string.IsNullOrEmpty(choice?.Message?.FirstTextContent))
                {
                    Console.WriteLine($"\nResponse: {choice.Message.FirstTextContent}");
                }
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
            if (ex.Message.Contains("modalities") || ex.Message.Contains("image"))
            {
                Console.WriteLine("\nNote: OpenRouter currently supports image generation with:");
                Console.WriteLine("  - google/gemini-2.5-flash-image-preview");
            }
        }
    }
}