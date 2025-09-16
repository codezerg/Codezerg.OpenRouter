using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;
using Codezerg.OpenRouter.Examples.Core;

namespace Codezerg.OpenRouter.Examples.Chat
{
    [Example("Interactive REPL", "Interactive chat session with conversation history")]
    public class ChatReplExample : IExample
    {
        public async Task RunAsync(OpenRouterClientOptions config)
        {
            ConsoleHelper.Section("Interactive REPL Mode");
            Console.WriteLine("Chat with the AI assistant. Type 'exit', 'quit', or 'bye' to end the session.");
            Console.WriteLine("Type 'clear' to clear conversation history.");
            Console.WriteLine("Type 'model' to change the model.");
            Console.WriteLine("Type 'stream' to toggle streaming mode.\n");

            using var client = new OpenRouterClient(config);
            var messages = new List<ChatMessage>
            {
                ChatMessage.System("You are a helpful AI assistant. Be concise and helpful.")
            };

            var streamingMode = false;
            var currentModel = config.DefaultModel;

            ConsoleHelper.Info($"Current model: {currentModel}");
            ConsoleHelper.Info($"Streaming: {(streamingMode ? "ON" : "OFF")}");

            while (true)
            {
                Console.Write("\nYou: ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                var lowerInput = input.ToLower().Trim();

                // Check for exit commands
                if (lowerInput == "exit" || lowerInput == "quit" || lowerInput == "bye")
                {
                    Console.WriteLine("\nGoodbye! Thanks for chatting.");
                    break;
                }

                // Check for clear command
                if (lowerInput == "clear")
                {
                    messages.Clear();
                    messages.Add(ChatMessage.System("You are a helpful AI assistant. Be concise and helpful."));
                    ConsoleHelper.Success("Conversation history cleared.");
                    continue;
                }

                // Check for model change command
                if (lowerInput == "model")
                {
                    Console.Write("Enter new model ID (or press Enter to keep current): ");
                    var newModel = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newModel))
                    {
                        currentModel = newModel;
                        ConsoleHelper.Success($"Model changed to: {currentModel}");
                    }
                    continue;
                }

                // Check for streaming toggle command
                if (lowerInput == "stream")
                {
                    streamingMode = !streamingMode;
                    ConsoleHelper.Success($"Streaming mode: {(streamingMode ? "ON" : "OFF")}");
                    continue;
                }

                // Add user message to history
                messages.Add(ChatMessage.User(input));

                // Create request
                var request = new ChatRequest
                {
                    Model = currentModel,
                    Messages = messages,
                    MaxTokens = 500
                };

                try
                {
                    Console.Write("\nAssistant: ");

                    if (streamingMode)
                    {
                        // Streaming response
                        var responseContent = "";
                        await foreach (var chunk in client.StreamChatCompletionAsync(request))
                        {
                            var token = chunk?.Choices?[0]?.Delta?.Content;
                            if (!string.IsNullOrEmpty(token))
                            {
                                Console.Write(token);
                                responseContent += token;
                            }
                        }
                        Console.WriteLine();

                        // Add assistant's response to history
                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            messages.Add(ChatMessage.Assistant(responseContent));
                        }
                    }
                    else
                    {
                        // Non-streaming response
                        var response = await client.SendChatCompletionAsync(request);
                        var content = response?.Choices?[0]?.Message?.FirstTextContent;

                        if (!string.IsNullOrEmpty(content))
                        {
                            Console.WriteLine(content);
                            messages.Add(ChatMessage.Assistant(content));
                        }
                        else
                        {
                            ConsoleHelper.Warn("No response received from the model.");
                        }

                        // Show token usage
                        if (response?.Usage != null)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine($"  [Tokens: {response.Usage.TotalTokens}]");
                            Console.ResetColor();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ConsoleHelper.Error($"Failed to get response: {ex.Message}");
                }
            }
        }
    }
}