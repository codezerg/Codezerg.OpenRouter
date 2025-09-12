using System;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;

namespace Codezerg.OpenRouter.Examples.Chat;

[Example("Simple Chat", "Basic chat completion example")]
public class SimpleChatExample
{
    public static async Task RunAsync(OpenRouterClientOptions config)
    {
        Console.WriteLine("\n=== Simple Chat Example ===\n");

        // Create client with provided configuration
        using var client = new OpenRouterClient(config);

        // Create a simple chat request
        var request = new ChatRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.System("You are a helpful assistant."),
                ChatMessage.User("What is the capital of France?")
            }
        };

        try
        {
            Console.WriteLine("Sending request to OpenRouter...");
            
            // Send the request
            var response = await client.SendChatCompletionAsync(request);

            // Display the response
            if (response.Choices?.Count > 0)
            {
                var message = response.Choices[0];
                Console.WriteLine($"\nAssistant: {message?.Message?.FirstTextContent}");
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
        }
    }
}