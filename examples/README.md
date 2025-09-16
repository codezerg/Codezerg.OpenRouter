# Codezerg.OpenRouter Examples

A comprehensive set of examples demonstrating the Codezerg.OpenRouter library capabilities.

## Prerequisites

1. **OpenRouter API Key**: Get your API key from [OpenRouter](https://openrouter.ai/keys)
2. **.NET 6.0 or later**: Required to run the examples

## Setting up your API Key

Set the `OPENROUTER_API_KEY` environment variable:

### Windows (PowerShell)
```powershell
$env:OPENROUTER_API_KEY="your-api-key-here"
```

### Linux/Mac
```bash
export OPENROUTER_API_KEY=your-api-key-here
```

## Running the Examples

### Interactive Menu (Default)
```bash
dotnet run
```
This launches an interactive menu where you can select and run individual examples.

### Run Specific Example
```bash
dotnet run "Simple Chat"
```

### List Available Examples
```bash
dotnet run --list
```

### Debug Modes
```bash
# Enable verbose mode (shows raw JSON requests/responses)
dotnet run --verbose

# Enable step mode (pauses between operations)
dotnet run --step

# Combine modes
dotnet run --verbose --step
```

### Command-Line Options
```
Options:
  --help, -h      Show help message
  --list, -l      List all available examples
  --verbose, -v   Enable verbose mode (show raw requests/responses)
  --step, -s      Enable step mode (pause between operations)
```

## Project Structure

```
examples/
│
├── Core/                    # Framework infrastructure
│   ├── IExample.cs         # Base interface for all examples
│   ├── ExampleAttribute.cs # Metadata for examples
│   ├── OpenRouterFactory.cs # Configuration factory
│   ├── ConsoleHelper.cs    # Standardized console output
│   ├── ResponsePrinter.cs  # Response formatting utilities
│   └── ExampleRunner.cs    # Reflection-based example discovery
│
├── Debug/                   # Debugging utilities
│   ├── RequestLogger.cs    # JSON request/response logging
│   └── ErrorHandler.cs     # Common error handling patterns
│
├── Chat/                    # Chat examples
│   ├── SimpleChatExample.cs       # Basic chat completion
│   ├── StreamingChatExample.cs    # Streaming responses
│   ├── MultimodalChatExample.cs   # Chat with images
│   └── ChatReplExample.cs         # Interactive REPL mode
│
├── Image/                   # Image processing examples
│   ├── ImageAnalysisExample.cs    # Analyze images
│   └── ImageGenerationExample.cs  # Generate images
│
├── Frontend/                # Frontend API examples
│   ├── FrontendApiExample.cs      # Frontend API demo
│   └── ModelExplorerExample.cs    # Model exploration
│
├── Tools/                   # Function calling examples
│   └── ToolUseExample.cs          # Tool/function calling
│
├── Costs/                   # Cost tracking examples
│   └── CostCalculationExample.cs  # Cost calculations
│
└── Endpoints/               # API endpoint examples
    └── ApiEndpointsExample.cs     # Various endpoints demo
```

## Available Examples

### Chat Examples
- **Simple Chat**: Basic chat completion
- **Streaming Chat**: Real-time streaming responses
- **Multimodal Chat**: Chat with text and images
- **Interactive REPL**: Interactive chat session with history

### Image Examples
- **Image Analysis**: Analyze local or remote images
- **Image Generation**: Generate images using AI models

### Frontend API Examples
- **Frontend API Demo**: Explore undocumented frontend APIs
- **Model Explorer**: Browse and compare available models

### Tool Examples
- **Tool Use**: Function calling with weather, calculator examples

### Cost Examples
- **Cost Calculation**: Calculate and track API usage costs

### Endpoint Examples
- **API Endpoints Demo**: Various OpenRouter API endpoints

## Example Models

The examples use various models including:
- **DeepSeek Chat V3 (Free)**: Default model for basic examples
- **Google Gemini 2.0 Flash**: For multimodal/vision tasks
- **GPT-4o**: Advanced reasoning tasks
- **Claude 3.5 Sonnet**: Complex conversations

You can change the model by modifying the `DefaultModel` in the configuration:

```csharp
var config = new OpenRouterClientOptions
{
    ApiKey = apiKey,
    DefaultModel = ModelConstants.OpenAI.Gpt4oMini
};
```

## Adding New Examples

1. Create a new class in the appropriate category folder
2. Implement the `IExample` interface
3. Add the `[Example("Name", "Description")]` attribute
4. The example will automatically appear in the menu

### Example Template

```csharp
using System;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;
using Codezerg.OpenRouter.Examples.Core;

namespace Codezerg.OpenRouter.Examples.YourCategory
{
    [Example("Your Example", "Brief description")]
    public class YourExample : IExample
    {
        public async Task RunAsync(OpenRouterClientOptions config)
        {
            ConsoleHelper.Section("Your Example");

            using var client = new OpenRouterClient(config);

            try
            {
                // Your example implementation
                ConsoleHelper.Info("Doing something...");

                // Make API calls
                var response = await client.SendChatCompletionAsync(...);

                // Display results
                ResponsePrinter.PrintChatResponse(response);

                ConsoleHelper.Success("Example completed!");
            }
            catch (Exception ex)
            {
                ConsoleHelper.Error($"Example failed: {ex.Message}");
            }
        }
    }
}
```

## Debugging Tips

### Verbose Mode

Use `--verbose` to see:
- Raw JSON requests being sent
- Raw JSON responses received
- Token estimates
- Timing information

### Step Mode

Use `--step` to:
- Pause between each operation
- Review output before continuing
- Debug complex multi-step examples

### Error Handling

The framework provides user-friendly error messages for common issues:
- Missing or invalid API keys
- Rate limiting (with automatic retry)
- Network connectivity issues
- Model access restrictions

## Troubleshooting

### API Key Not Found
If you see "Please set the OPENROUTER_API_KEY environment variable", make sure you've set the environment variable correctly in your current session.

### Rate Limits
OpenRouter has rate limits based on your account type. If you encounter rate limit errors, wait a moment before retrying.

### Model Availability
Some models may require specific account permissions or credits. Check your OpenRouter dashboard for available models.

## Additional Resources

- [OpenRouter Documentation](https://openrouter.ai/docs)
- [Supported Models](https://openrouter.ai/models)
- [API Reference](https://openrouter.ai/docs/api-reference)
- [Pricing Information](https://openrouter.ai/docs/pricing)