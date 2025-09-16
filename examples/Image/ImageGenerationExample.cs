using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Codezerg.OpenRouter;
using Codezerg.OpenRouter.Models;
using Codezerg.OpenRouter.Examples.Core;

namespace Codezerg.OpenRouter.Examples.Image
{
    [Example("Image Generation", "Generate images using AI models")]
    public class ImageGenerationExample : IExample
{
        public async Task RunAsync(OpenRouterClientOptions config)
    {
        Console.WriteLine("\n=== Image Generation Example ===\n");

        // Clone config and set image generation model for this example
        var imageGenConfig = config.Clone();
        imageGenConfig.DefaultModel = "google/gemini-2.5-flash-image-preview"; // OpenRouter's image generation model

        // Create client with image generation model
        using var client = new OpenRouterClient(imageGenConfig);

        // Use a predefined prompt for non-interactive example
        var prompt = "A beautiful sunset over mountains with a lake in the foreground, painted in impressionist style";
        Console.WriteLine($"Prompt: {prompt}");

        // Create an image generation request
        var request = new ChatRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.User(prompt)
            },
            Modalities = new List<Modality> { Modality.Text, Modality.Image } // Request image output
        };

        try
        {
            Console.WriteLine($"\nGenerating image for: \"{prompt}\"");
            Console.WriteLine("Please wait, this may take a moment...\n");
            
            // Send the request
            var response = await client.SendChatCompletionAsync(request);

            // Check for generated images in the response
            if (response.Choices?.Count > 0)
            {
                var choice = response.Choices[0];
                
                // Check for generated images
                if (choice?.Message?.Images != null && choice.Message.Images.Count > 0)
                {
                    Console.WriteLine("Image(s) generated successfully!");
                    
                    // Create output directory if it doesn't exist
                    var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "generated-images");
                    Directory.CreateDirectory(outputDir);
                    
                    using var httpClient = new HttpClient();
                    int imageIndex = 0;
                    
                    foreach (var image in choice.Message.Images)
                    {
                        if (!string.IsNullOrEmpty(image.ImageUrl?.Url))
                        {
                            Console.WriteLine($"Image URL: {image.ImageUrl.Url}");
                            
                            try
                            {
                                // Check if it's a base64 image
                                if (image.ImageUrl.Url.StartsWith("data:image/"))
                                {
                                    // Extract base64 data
                                    var base64Data = image.ImageUrl.Url.Substring(image.ImageUrl.Url.IndexOf("base64,") + 7);
                                    var imageBytes = Convert.FromBase64String(base64Data);
                                    
                                    // Determine file extension from data URL
                                    var mimeType = image.ImageUrl.Url.Substring(5, image.ImageUrl.Url.IndexOf(';') - 5);
                                    var extension = mimeType.Split('/')[1];
                                    if (extension == "jpeg") extension = "jpg";
                                    
                                    var fileName = $"generated_{DateTime.Now:yyyyMMdd_HHmmss}_{imageIndex}.{extension}";
                                    var filePath = Path.Combine(outputDir, fileName);
                                    
                                    await File.WriteAllBytesAsync(filePath, imageBytes);
                                    Console.WriteLine($"Saved image to: {filePath}");
                                }
                                else
                                {
                                    // Download image from URL
                                    var imageBytes = await httpClient.GetByteArrayAsync(image.ImageUrl.Url);
                                    
                                    // Try to determine extension from URL or default to jpg
                                    var extension = Path.GetExtension(new Uri(image.ImageUrl.Url).AbsolutePath);
                                    if (string.IsNullOrEmpty(extension)) extension = ".jpg";
                                    
                                    var fileName = $"generated_{DateTime.Now:yyyyMMdd_HHmmss}_{imageIndex}{extension}";
                                    var filePath = Path.Combine(outputDir, fileName);
                                    
                                    await File.WriteAllBytesAsync(filePath, imageBytes);
                                    Console.WriteLine($"Saved image to: {filePath}");
                                }
                                
                                imageIndex++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Failed to save image: {ex.Message}");
                            }
                        }
                    }
                }
                
                // Also check for text response
                if (!string.IsNullOrEmpty(choice?.Message?.FirstTextContent))
                {
                    Console.WriteLine($"\nResponse: {choice.Message.FirstTextContent}");
                }
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
            if (ex.Message.Contains("modalities") || ex.Message.Contains("image"))
            {
                Console.WriteLine("\nNote: OpenRouter currently supports image generation with:");
                Console.WriteLine("  - google/gemini-2.5-flash-image-preview");
            }
        }
    }
    }
}