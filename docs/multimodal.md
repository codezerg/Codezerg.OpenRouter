# Multimodal Capabilities

This guide covers how to work with multimodal content (text, images, and audio) using Codezerg.OpenRouter.

## Overview

Multimodal AI models can process and understand multiple types of content in a single conversation. Codezerg.OpenRouter provides a fluent API for constructing multimodal messages that combine:

- **Text**: Natural language content
- **Images**: Photos, diagrams, screenshots
- **Audio**: Speech, music, sound effects

## Supported Models

Not all models support multimodal input. Common vision-capable models include:

- **OpenAI**: GPT-4o, GPT-4 Vision
- **Anthropic**: Claude 3 (Opus, Sonnet, Haiku)
- **Google**: Gemini Pro Vision, Gemini 2.0 Flash
- **Other**: Various specialized vision models

Check [OpenRouter's model list](https://openrouter.ai/models) for current multimodal support.

## Working with Images

### Image from URL

```csharp
var message = new ChatMessage(ChatRole.User)
    .AddText("What's in this image?")
    .AddImage("https://example.com/sunset.jpg");

var request = new ChatRequest
{
    Model = "google/gemini-2.0-flash-exp",
    Messages = new List<ChatMessage> { message }
};

var response = await client.SendChatCompletionAsync(request);
Console.WriteLine(response.Choices[0].Message.FirstTextContent);
```

### Image from Local File

```csharp
// Read image file
byte[] imageBytes = File.ReadAllBytes("path/to/image.jpg");

var message = new ChatMessage(ChatRole.User)
    .AddText("Analyze this chart")
    .AddImage(imageBytes, "image/jpeg");

var request = new ChatRequest
{
    Model = "openai/gpt-4o",
    Messages = new List<ChatMessage> { message }
};
```

### Base64 Encoded Images

```csharp
string base64Image = Convert.ToBase64String(imageBytes);
string dataUri = $"data:image/jpeg;base64,{base64Image}";

var message = new ChatMessage(ChatRole.User)
    .AddText("Describe this image")
    .AddImage(dataUri);
```

### Multiple Images

```csharp
var message = new ChatMessage(ChatRole.User)
    .AddText("Compare these two images:")
    .AddImage("https://example.com/image1.jpg")
    .AddImage("https://example.com/image2.jpg")
    .AddText("What are the main differences?");

var request = new ChatRequest
{
    Model = "anthropic/claude-3-sonnet",
    Messages = new List<ChatMessage> { message }
};
```

## Image Processing Examples

### OCR (Optical Character Recognition)

```csharp
public async Task<string> ExtractTextFromImageAsync(string imagePath)
{
    var imageBytes = File.ReadAllBytes(imagePath);
    
    var message = new ChatMessage(ChatRole.User)
        .AddText("Extract all text from this image. Preserve formatting and structure.")
        .AddImage(imageBytes, "image/png");
    
    var request = new ChatRequest
    {
        Model = "google/gemini-2.0-flash-exp",
        Messages = new List<ChatMessage> { message },
        Temperature = 0.1  // Low temperature for accuracy
    };
    
    var response = await client.SendChatCompletionAsync(request);
    return response.Choices[0].Message.FirstTextContent;
}
```

### Image Analysis

```csharp
public async Task<ImageAnalysis> AnalyzeImageAsync(string imageUrl)
{
    var message = new ChatMessage(ChatRole.User)
        .AddText(@"Analyze this image and provide:
            1. Main subjects/objects
            2. Setting/location
            3. Colors and composition
            4. Mood or atmosphere
            5. Any text visible
            Format as JSON.")
        .AddImage(imageUrl);
    
    var request = new ChatRequest
    {
        Model = "openai/gpt-4o",
        Messages = new List<ChatMessage> { message },
        ResponseFormat = new { type = "json_object" }
    };
    
    var response = await client.SendChatCompletionAsync(request);
    return JsonConvert.DeserializeObject<ImageAnalysis>(
        response.Choices[0].Message.FirstTextContent);
}
```

### Screenshot Understanding

```csharp
public async Task<string> DescribeUIScreenshotAsync(byte[] screenshot)
{
    var message = new ChatMessage(ChatRole.User)
        .AddText("Describe the UI elements in this screenshot. Include buttons, text fields, menus, and their layout.")
        .AddImage(screenshot, "image/png");
    
    var request = new ChatRequest
    {
        Model = "anthropic/claude-3-sonnet",
        Messages = new List<ChatMessage> { message }
    };
    
    var response = await client.SendChatCompletionAsync(request);
    return response.Choices[0].Message.FirstTextContent;
}
```

### Diagram Interpretation

```csharp
public async Task<string> ExplainDiagramAsync(string diagramPath)
{
    var imageBytes = File.ReadAllBytes(diagramPath);
    
    var message = new ChatMessage(ChatRole.User)
        .AddText("Explain this technical diagram in detail. Describe the components, their relationships, and the overall process or system being illustrated.")
        .AddImage(imageBytes, "image/png");
    
    var request = new ChatRequest
    {
        Model = "google/gemini-2.0-flash-exp",
        Messages = new List<ChatMessage> { message },
        MaxTokens = 1000
    };
    
    var response = await client.SendChatCompletionAsync(request);
    return response.Choices[0].Message.FirstTextContent;
}
```

## Working with Audio

### Audio from URL

```csharp
var message = new ChatMessage(ChatRole.User)
    .AddText("Transcribe this audio:")
    .AddAudio("https://example.com/speech.mp3");

var request = new ChatRequest
{
    Model = "openai/whisper-1",  // Or appropriate audio model
    Messages = new List<ChatMessage> { message }
};
```

### Audio from Local File

```csharp
byte[] audioBytes = File.ReadAllBytes("path/to/audio.wav");

var message = new ChatMessage(ChatRole.User)
    .AddText("What is being said in this recording?")
    .AddAudio(audioBytes, "audio/wav");
```

## Complex Multimodal Scenarios

### Document Analysis with Multiple Pages

```csharp
public async Task<string> AnalyzeDocumentAsync(List<string> pageImages)
{
    var message = new ChatMessage(ChatRole.User)
        .AddText("Analyze this multi-page document. Provide a summary of the key points.");
    
    foreach (var pageImage in pageImages)
    {
        message.AddImage(pageImage);
    }
    
    var request = new ChatRequest
    {
        Model = "google/gemini-2.0-flash-exp",
        Messages = new List<ChatMessage> { message },
        MaxTokens = 2000
    };
    
    var response = await client.SendChatCompletionAsync(request);
    return response.Choices[0].Message.FirstTextContent;
}
```

### Visual Question Answering

```csharp
public async Task<string> AnswerVisualQuestionAsync(
    string imageUrl, 
    string question)
{
    var messages = new List<ChatMessage>
    {
        new ChatMessage(ChatRole.System)
        {
            Content = "You are a helpful assistant that answers questions about images accurately and concisely."
        },
        new ChatMessage(ChatRole.User)
            .AddImage(imageUrl)
            .AddText(question)
    };
    
    var request = new ChatRequest
    {
        Model = "openai/gpt-4o",
        Messages = messages,
        Temperature = 0.3
    };
    
    var response = await client.SendChatCompletionAsync(request);
    return response.Choices[0].Message.FirstTextContent;
}
```

### Image Comparison

```csharp
public async Task<ComparisonResult> CompareImagesAsync(
    string image1Path, 
    string image2Path)
{
    var image1Bytes = File.ReadAllBytes(image1Path);
    var image2Bytes = File.ReadAllBytes(image2Path);
    
    var message = new ChatMessage(ChatRole.User)
        .AddText("Compare these two images. Identify:")
        .AddText("1. Similarities")
        .AddText("2. Differences")
        .AddText("3. Which appears to be higher quality")
        .AddText("\nFirst image:")
        .AddImage(image1Bytes, "image/jpeg")
        .AddText("\nSecond image:")
        .AddImage(image2Bytes, "image/jpeg");
    
    var request = new ChatRequest
    {
        Model = "anthropic/claude-3-opus",
        Messages = new List<ChatMessage> { message }
    };
    
    var response = await client.SendChatCompletionAsync(request);
    // Parse response into ComparisonResult
    return ParseComparisonResult(response.Choices[0].Message.FirstTextContent);
}
```

## Streaming with Multimodal Content

```csharp
var message = new ChatMessage(ChatRole.User)
    .AddText("Describe this image in detail:")
    .AddImage("https://example.com/complex-scene.jpg");

var request = new ChatRequest
{
    Model = "google/gemini-2.0-flash-exp",
    Messages = new List<ChatMessage> { message },
    Stream = true
};

await foreach (var chunk in client.StreamChatCompletionAsync(request))
{
    if (chunk.Choices?[0].Delta?.Content != null)
    {
        Console.Write(chunk.Choices[0].Delta.Content);
    }
}
```

## Best Practices

### 1. Image Optimization

```csharp
public byte[] OptimizeImageForApi(string imagePath, int maxWidth = 1024)
{
    using var image = Image.Load(imagePath);
    
    // Resize if needed
    if (image.Width > maxWidth)
    {
        var ratio = (float)maxWidth / image.Width;
        image.Mutate(x => x.Resize((int)(image.Width * ratio), 
                                   (int)(image.Height * ratio)));
    }
    
    // Convert to JPEG for smaller size
    using var ms = new MemoryStream();
    image.SaveAsJpeg(ms, new JpegEncoder { Quality = 85 });
    return ms.ToArray();
}
```

### 2. Content Validation

```csharp
public void ValidateMultimodalMessage(ChatMessage message)
{
    if (message.Parts == null || message.Parts.Count == 0)
    {
        throw new InvalidOperationException("Message has no content");
    }
    
    var imageCount = message.Parts.Count(p => p.Type == MessageContentType.ImageUrl);
    if (imageCount > 10)
    {
        throw new InvalidOperationException("Too many images (max 10)");
    }
    
    foreach (var part in message.Parts.Where(p => p.Type == MessageContentType.ImageUrl))
    {
        if (part.ImageUrl?.Url?.StartsWith("data:") == true)
        {
            // Check base64 size
            var base64Length = part.ImageUrl.Url.Length - part.ImageUrl.Url.IndexOf(',') - 1;
            var estimatedSize = base64Length * 3 / 4;
            if (estimatedSize > 20 * 1024 * 1024) // 20MB limit
            {
                throw new InvalidOperationException("Image too large (max 20MB)");
            }
        }
    }
}
```

### 3. Error Handling

```csharp
public async Task<string> SafeImageAnalysisAsync(string imageUrl)
{
    try
    {
        var message = new ChatMessage(ChatRole.User)
            .AddText("Analyze this image")
            .AddImage(imageUrl);
        
        var request = new ChatRequest
        {
            Model = "openai/gpt-4o",
            Messages = new List<ChatMessage> { message }
        };
        
        return await client.SendChatCompletionAsync(request)
            .Choices[0].Message.FirstTextContent;
    }
    catch (HttpRequestException ex) when (ex.Message.Contains("image"))
    {
        // Image-specific error
        return "Error: Unable to process image. It may be too large, corrupted, or in an unsupported format.";
    }
    catch (Exception ex)
    {
        return $"Error analyzing image: {ex.Message}";
    }
}
```

## Performance Considerations

### Image Size Guidelines

| Format | Recommended Max Size | Use Case |
|--------|---------------------|----------|
| JPEG | 5MB | Photos, complex scenes |
| PNG | 5MB | Screenshots, diagrams |
| WebP | 5MB | General purpose |
| GIF | 10MB | Animated content (first frame) |

### Preprocessing Tips

1. **Resize large images**: Reduce to 1024x1024 or smaller
2. **Compress**: Use 85% JPEG quality for photos
3. **Convert formats**: Use JPEG for photos, PNG for screenshots
4. **Cache processed images**: Avoid reprocessing

### Batching Strategies

```csharp
public async Task<List<string>> BatchAnalyzeImagesAsync(List<string> imageUrls)
{
    var tasks = imageUrls.Select(async url =>
    {
        var message = new ChatMessage(ChatRole.User)
            .AddText("Briefly describe this image in one sentence:")
            .AddImage(url);
        
        var request = new ChatRequest
        {
            Model = "google/gemini-2.0-flash-exp",
            Messages = new List<ChatMessage> { message },
            MaxTokens = 50
        };
        
        var response = await client.SendChatCompletionAsync(request);
        return response.Choices[0].Message.FirstTextContent;
    });
    
    return (await Task.WhenAll(tasks)).ToList();
}
```

## Limitations

- **File size limits**: Usually 5-20MB per image
- **Format support**: JPEG, PNG, GIF, WebP typically supported
- **Number of images**: Often limited to 10-20 per request
- **Audio support**: Limited to specific models
- **Video support**: Not directly supported (extract frames instead)

## Troubleshooting

### Image Not Processing

- Verify URL is accessible
- Check image format is supported
- Ensure image size is within limits
- Try converting to JPEG

### Poor Quality Results

- Use higher resolution images
- Ensure good lighting/contrast
- Try different models
- Provide more specific prompts

### Token Limit Exceeded

- Reduce image resolution
- Use fewer images
- Decrease MaxTokens
- Split into multiple requests

## Further Reading

- [API Reference](api-reference.md#chatmessage)
- [Examples](examples.md#multimodal-examples)
- [Model Documentation](models.md)