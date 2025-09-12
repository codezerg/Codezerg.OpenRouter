using System;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;

namespace Codezerg.OpenRouter.Examples.Chat;

[Example("Streaming Chat", "Chat completion with streaming response")]
public class StreamingChatExample
{
    public static async Task RunAsync(OpenRouterClientOptions config)
    {
        Console.WriteLine("\n=== Streaming Chat Example ===\n");

        // Create client with provided configuration
        using var client = new OpenRouterClient(config);

        // Create a chat request
        var request = new ChatRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.System("You are a creative storyteller."),
                ChatMessage.User("Tell me a very short story about a robot learning to paint.")
            },
            MaxTokens = 200
        };

        try
        {
            Console.WriteLine("Streaming response from OpenRouter...\n");
            Console.Write("Assistant: ");

            // Stream the response
            await foreach (var chunk in client.StreamChatCompletionAsync(request))
            {
                if (chunk.Choices?.Count > 0)
                {
                    var choice = chunk.Choices[0];
                    var content = choice?.Delta?.Content;
                    if (!string.IsNullOrEmpty(content))
                    {
                        Console.Write(content);
                    }
                }
            }

            Console.WriteLine("\n\n[Streaming completed]");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }
    }
}