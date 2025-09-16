using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;
using Codezerg.OpenRouter.Examples.Core;

namespace Codezerg.OpenRouter.Examples.Chat
{
    [Example("Simple Chat", "Basic chat completion example")]
    public class SimpleChatExample : IExample
{
        public async Task RunAsync(OpenRouterClientOptions config)
    {
        ConsoleHelper.Section("Executing Simple Chat");

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
            ConsoleHelper.Info("Sending request to OpenRouter...");
            
            // Send the request
            var response = await client.SendChatCompletionAsync(request);

            // Display the response
            Console.WriteLine("\nAssistant:");
            ResponsePrinter.PrintChatResponse(response);
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error($"Chat failed: {ex.Message}");
        }
    }
    }
}