using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;
using Codezerg.OpenRouter.Examples.Core;

namespace Codezerg.OpenRouter.Examples.Chat
{
    [Example("Streaming Chat", "Chat completion with streaming response")]
    public class StreamingChatExample : IExample
{
        public async Task RunAsync(OpenRouterClientOptions config)
    {
        ConsoleHelper.Section("Executing Streaming Chat");

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
            ConsoleHelper.Info("Streaming response from OpenRouter...");
            Console.Write("\nAssistant: ");

            // Stream the response
            await foreach (var chunk in client.StreamChatCompletionAsync(request))
            {
                ResponsePrinter.PrintStreamToken(chunk);
            }

            Console.WriteLine();
            ConsoleHelper.Success("Streaming completed");
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error($"Streaming failed: {ex.Message}");
        }
    }
    }
}