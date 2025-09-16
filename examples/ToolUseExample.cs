using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;
using Newtonsoft.Json.Linq;

namespace Codezerg.OpenRouter.Examples
{
    public class ToolUseExample
    {
        public static async Task RunAsync()
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Please set OPENROUTER_API_KEY environment variable");
                return;
            }

            var client = new OpenRouterClient(new OpenRouterClientOptions
            {
                ApiKey = apiKey,
                DefaultModel = "openai/gpt-4o-mini"
            });

            await Console.Out.WriteLineAsync("=== Tool Use Example ===\n");
            await DemoWeatherToolAsync(client);
            await Console.Out.WriteLineAsync("\n=== Calculator Tool Example ===\n");
            await DemoCalculatorToolAsync(client);
            await Console.Out.WriteLineAsync("\n=== Multiple Tools Example ===\n");
            await DemoMultipleToolsAsync(client);
        }

        private static async Task DemoWeatherToolAsync(OpenRouterClient client)
        {
            var weatherTool = new ToolDefinition
            {
                Type = ToolType.Function,
                Function = new FunctionDescription
                {
                    Name = "get_weather",
                    Description = "Get the current weather for a location",
                    Parameters = JObject.FromObject(new
                    {
                        type = "object",
                        properties = new
                        {
                            location = new
                            {
                                type = "string",
                                description = "The city and state, e.g. San Francisco, CA"
                            },
                            unit = new
                            {
                                type = "string",
                                @enum = new[] { "celsius", "fahrenheit" },
                                description = "Temperature unit"
                            }
                        },
                        required = new[] { "location" }
                    })
                }
            };

            var request = new ChatRequest
            {
                Model = "openai/gpt-4o-mini",
                Messages = new List<ChatMessage>
                {
                    ChatMessage.System("You are a helpful weather assistant. Use the weather tool to answer questions."),
                    ChatMessage.User("What's the weather like in New York and Los Angeles?")
                },
                Tools = new List<ToolDefinition> { weatherTool },
                ToolChoice = "auto"
            };

            Console.WriteLine("User: What's the weather like in New York and Los Angeles?\n");

            var response = await client.SendChatCompletionAsync(request);

            if (response.Choices?[0].Message?.ToolCalls != null)
            {
                Console.WriteLine("Assistant is calling tools...\n");

                var toolCallMessages = new List<ChatMessage>();

                foreach (var toolCall in response.Choices[0].Message.ToolCalls)
                {
                    Console.WriteLine($"Calling function: {toolCall.Function.Name}");
                    Console.WriteLine($"Arguments: {toolCall.Function.Arguments}\n");

                    var weatherData = SimulateWeatherApi(toolCall.Function.Arguments);

                    toolCallMessages.Add(new ChatMessage(ChatRole.Tool)
                        .AddText(weatherData)
                        .WithToolCallId(toolCall.Id));
                }

                request.Messages.Add(response.Choices[0].Message);
                request.Messages.AddRange(toolCallMessages);

                var finalResponse = await client.SendChatCompletionAsync(request);
                Console.WriteLine($"Assistant: {finalResponse.Choices?[0].Message?.FirstTextContent}");
            }
            else
            {
                Console.WriteLine($"Assistant: {response.Choices?[0].Message?.FirstTextContent}");
            }
        }

        private static async Task DemoCalculatorToolAsync(OpenRouterClient client)
        {
            var calculatorTool = new ToolDefinition
            {
                Type = ToolType.Function,
                Function = new FunctionDescription
                {
                    Name = "calculate",
                    Description = "Perform mathematical calculations",
                    Parameters = JObject.FromObject(new
                    {
                        type = "object",
                        properties = new
                        {
                            expression = new
                            {
                                type = "string",
                                description = "A mathematical expression to evaluate, e.g., '2 + 2', '10 * 5', 'sqrt(16)'"
                            }
                        },
                        required = new[] { "expression" }
                    })
                }
            };

            var request = new ChatRequest
            {
                Model = "openai/gpt-4o-mini",
                Messages = new List<ChatMessage>
                {
                    ChatMessage.System("You are a helpful math assistant. Use the calculator tool for calculations."),
                    ChatMessage.User("What is 235 * 47 + 128 / 4?")
                },
                Tools = new List<ToolDefinition> { calculatorTool },
                ToolChoice = "auto"
            };

            Console.WriteLine("User: What is 235 * 47 + 128 / 4?\n");

            var response = await client.SendChatCompletionAsync(request);

            if (response.Choices?[0].Message?.ToolCalls != null)
            {
                Console.WriteLine("Assistant is calculating...\n");

                foreach (var toolCall in response.Choices[0].Message.ToolCalls)
                {
                    Console.WriteLine($"Expression: {toolCall.Function.Arguments}");

                    var result = SimulateCalculator(toolCall.Function.Arguments);
                    Console.WriteLine($"Result: {result}\n");

                    request.Messages.Add(response.Choices[0].Message);
                    request.Messages.Add(new ChatMessage(ChatRole.Tool)
                        .AddText(result)
                        .WithToolCallId(toolCall.Id));
                }

                var finalResponse = await client.SendChatCompletionAsync(request);
                Console.WriteLine($"Assistant: {finalResponse.Choices?[0].Message?.FirstTextContent}");
            }
        }

        private static async Task DemoMultipleToolsAsync(OpenRouterClient client)
        {
            var tools = new List<ToolDefinition>
            {
                new ToolDefinition
                {
                    Type = ToolType.Function,
                    Function = new FunctionDescription
                    {
                        Name = "get_stock_price",
                        Description = "Get the current stock price for a ticker symbol",
                        Parameters = JObject.FromObject(new
                        {
                            type = "object",
                            properties = new
                            {
                                symbol = new
                                {
                                    type = "string",
                                    description = "Stock ticker symbol, e.g., AAPL, GOOGL"
                                }
                            },
                            required = new[] { "symbol" }
                        })
                    }
                },
                new ToolDefinition
                {
                    Type = ToolType.Function,
                    Function = new FunctionDescription
                    {
                        Name = "get_exchange_rate",
                        Description = "Get exchange rate between two currencies",
                        Parameters = JObject.FromObject(new
                        {
                            type = "object",
                            properties = new
                            {
                                from_currency = new
                                {
                                    type = "string",
                                    description = "Source currency code, e.g., USD, EUR"
                                },
                                to_currency = new
                                {
                                    type = "string",
                                    description = "Target currency code"
                                }
                            },
                            required = new[] { "from_currency", "to_currency" }
                        })
                    }
                }
            };

            var request = new ChatRequest
            {
                Model = "openai/gpt-4o-mini",
                Messages = new List<ChatMessage>
                {
                    ChatMessage.System("You are a financial assistant with access to stock prices and exchange rates."),
                    ChatMessage.User("What's the current price of Apple stock? And if I buy 10 shares with euros, how much would that cost me in USD?")
                },
                Tools = tools,
                ToolChoice = "auto",
                ParallelToolCalls = true
            };

            Console.WriteLine("User: What's the current price of Apple stock? And if I buy 10 shares with euros, how much would that cost me in USD?\n");

            var response = await client.SendChatCompletionAsync(request);

            if (response.Choices?[0].Message?.ToolCalls != null)
            {
                Console.WriteLine("Assistant is gathering information...\n");

                var toolResponses = new List<ChatMessage>();

                foreach (var toolCall in response.Choices[0].Message.ToolCalls)
                {
                    Console.WriteLine($"Calling: {toolCall.Function.Name}");
                    Console.WriteLine($"Args: {toolCall.Function.Arguments}");

                    string result = toolCall.Function.Name switch
                    {
                        "get_stock_price" => SimulateStockApi(toolCall.Function.Arguments),
                        "get_exchange_rate" => SimulateExchangeApi(toolCall.Function.Arguments),
                        _ => "Unknown function"
                    };

                    Console.WriteLine($"Result: {result}\n");

                    toolResponses.Add(new ChatMessage(ChatRole.Tool)
                        .AddText(result)
                        .WithToolCallId(toolCall.Id));
                }

                request.Messages.Add(response.Choices[0].Message);
                request.Messages.AddRange(toolResponses);

                var finalResponse = await client.SendChatCompletionAsync(request);
                Console.WriteLine($"Assistant: {finalResponse.Choices?[0].Message?.FirstTextContent}");
            }
        }

        private static string SimulateWeatherApi(string arguments)
        {
            try
            {
                var args = JsonSerializer.Deserialize<Dictionary<string, object>>(arguments);
                if (args == null) return JsonSerializer.Serialize(new { error = "Invalid arguments" });

                var location = args.ContainsKey("location") ? args["location"].ToString() : "Unknown";
                var unit = args.ContainsKey("unit") ? args["unit"].ToString() : "fahrenheit";

                var random = new Random();
                var temp = random.Next(60, 85);
                var conditions = new[] { "sunny", "partly cloudy", "cloudy", "rainy" };
                var condition = conditions[random.Next(conditions.Length)];

                return JsonSerializer.Serialize(new
                {
                    location,
                    temperature = temp,
                    unit,
                    condition,
                    humidity = random.Next(40, 70),
                    wind_speed = random.Next(5, 20)
                });
            }
            catch
            {
                return JsonSerializer.Serialize(new { error = "Failed to get weather data" });
            }
        }

        private static string SimulateCalculator(string arguments)
        {
            try
            {
                var args = JsonSerializer.Deserialize<Dictionary<string, object>>(arguments);
                if (args == null || !args.ContainsKey("expression"))
                    return JsonSerializer.Serialize(new { error = "Invalid arguments" });

                var expression = args["expression"].ToString();

                var dataTable = new System.Data.DataTable();
                var result = dataTable.Compute(expression, null);

                return JsonSerializer.Serialize(new
                {
                    expression,
                    result = result.ToString()
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    error = $"Calculation error: {ex.Message}"
                });
            }
        }

        private static string SimulateStockApi(string arguments)
        {
            try
            {
                var args = JsonSerializer.Deserialize<Dictionary<string, object>>(arguments);
                if (args == null || !args.ContainsKey("symbol"))
                    return JsonSerializer.Serialize(new { error = "Invalid arguments" });

                var symbol = args["symbol"].ToString()?.ToUpper() ?? "UNKNOWN";

                var prices = new Dictionary<string, decimal>
                {
                    ["AAPL"] = 189.45m,
                    ["GOOGL"] = 142.73m,
                    ["MSFT"] = 405.21m,
                    ["AMZN"] = 178.92m
                };

                var price = prices.ContainsKey(symbol) ? prices[symbol] : 100.00m;

                return JsonSerializer.Serialize(new
                {
                    symbol,
                    price,
                    currency = "USD",
                    timestamp = DateTime.UtcNow
                });
            }
            catch
            {
                return JsonSerializer.Serialize(new { error = "Failed to get stock price" });
            }
        }

        private static string SimulateExchangeApi(string arguments)
        {
            try
            {
                var args = JsonSerializer.Deserialize<Dictionary<string, object>>(arguments);
                if (args == null || !args.ContainsKey("from_currency") || !args.ContainsKey("to_currency"))
                    return JsonSerializer.Serialize(new { error = "Invalid arguments" });

                var from = args["from_currency"].ToString()?.ToUpper() ?? "USD";
                var to = args["to_currency"].ToString()?.ToUpper() ?? "USD";

                var rates = new Dictionary<string, decimal>
                {
                    ["EUR_USD"] = 1.09m,
                    ["USD_EUR"] = 0.92m,
                    ["GBP_USD"] = 1.27m,
                    ["USD_GBP"] = 0.79m
                };

                var key = $"{from}_{to}";
                var rate = rates.ContainsKey(key) ? rates[key] : 1.0m;

                return JsonSerializer.Serialize(new
                {
                    from_currency = from,
                    to_currency = to,
                    exchange_rate = rate,
                    timestamp = DateTime.UtcNow
                });
            }
            catch
            {
                return JsonSerializer.Serialize(new { error = "Failed to get exchange rate" });
            }
        }
    }
}