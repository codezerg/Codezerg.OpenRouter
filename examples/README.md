# Codezerg.OpenRouter Examples

This directory contains examples demonstrating how to use the Codezerg.OpenRouter library.

## Prerequisites

1. **OpenRouter API Key**: Get your API key from [OpenRouter](https://openrouter.ai/keys)
2. **.NET 8.0 or later**: Required to run the examples

## Setting up your API Key

Set the `OPENROUTER_API_KEY` environment variable:

### Windows (Command Prompt)
```cmd
set OPENROUTER_API_KEY=your-api-key-here
```

### Windows (PowerShell)
```powershell
$env:OPENROUTER_API_KEY="your-api-key-here"
```

### Linux/Mac
```bash
export OPENROUTER_API_KEY=your-api-key-here
```

## Running the Examples

### Build the project
```bash
dotnet build
```

### Run the examples
```bash
dotnet run --project examples/Codezerg.OpenRouter.Examples.csproj
```

Or navigate to the examples directory:
```bash
cd examples
dotnet run
```

## Available Examples

### 1. Simple Chat
Basic chat completion example showing how to:
- Create a client with configuration
- Send a simple chat request
- Handle the response
- Display token usage

### 2. Streaming Chat
Demonstrates streaming responses for real-time output:
- Enable streaming mode
- Handle Server-Sent Events (SSE)
- Display content as it arrives

### 3. Multimodal Chat
Shows how to work with images:
- Create multimodal messages
- Add text and image content
- Use vision-capable models

## Example Models

The examples use various models including:
- **DeepSeek Chat V3 (Free)**: Default model for basic examples
- **Google Gemini 2.0 Flash**: For multimodal/vision tasks
- **GPT-4o**: Advanced reasoning tasks
- **Claude 3.5 Sonnet**: Complex conversations

You can change the model by modifying the `DefaultModel` in the configuration:

```csharp
var config = new OpenRouterConfig
{
    ApiKey = apiKey,
    DefaultModel = ModelConstants.OpenAI.Gpt4oMini
};
```

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