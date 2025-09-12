using System;
using System.IO;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;

namespace Codezerg.OpenRouter.Examples.Chat;

[Example("Multimodal Chat", "Chat with images")]
public class MultimodalChatExample
{
    public static async Task RunAsync(OpenRouterConfig config)
    {
        Console.WriteLine("\n=== Multimodal Chat Example ===\n");

        // Clone config and set vision-capable model for this example
        var visionConfig = config.Clone();
        visionConfig.DefaultModel = ModelConstants.Google.Gemini20Flash;

        // Create client with vision-capable model
        using var client = new OpenRouterClient(visionConfig);

        // Create a multimodal chat request with an image
        var message = new ChatMessage("user")
            .AddText("What do you see in this image? Please describe it in detail.")
            .AddImage("https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Cat03.jpg/1200px-Cat03.jpg");

        var request = new ChatCompletionRequest
        {
            Messages = new List<ChatMessage> { message }
        };

        try
        {
            Console.WriteLine("Sending multimodal request to OpenRouter...");
            Console.WriteLine("Image: https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Cat03.jpg/1200px-Cat03.jpg\n");
            
            // Send the request
            var response = await client.SendChatCompletionAsync(request);

            // Display the response
            if (response.Choices?.Count > 0)
            {
                var choice = response.Choices[0];
                Console.WriteLine($"Assistant: {choice?.Message?.FirstTextContent}");
            }
            else
            {
                Console.WriteLine("No response received.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}