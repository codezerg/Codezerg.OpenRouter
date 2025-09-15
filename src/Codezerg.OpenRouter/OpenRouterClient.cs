using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Codezerg.OpenRouter.Models;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter;

/// <summary>
/// Client for interacting with the OpenRouter API.
/// Provides methods for sending chat completion requests to various LLM models.
/// </summary>
public class OpenRouterClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly OpenRouterClientOptions _config;
    private readonly bool _ownsHttpClient;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenRouterClient"/> class with the specified configuration.
    /// </summary>
    /// <param name="config">The configuration options for the client.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="config"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the configuration is invalid.</exception>
    public OpenRouterClient(OpenRouterClientOptions config) : this(config, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenRouterClient"/> class with the specified configuration and HTTP client.
    /// </summary>
    /// <param name="config">The configuration options for the client.</param>
    /// <param name="httpClient">An optional HTTP client instance. If null, a new instance will be created.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="config"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the configuration is invalid.</exception>
    public OpenRouterClient(OpenRouterClientOptions config, HttpClient httpClient)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        // Validate the configuration
        config.Validate();

        // Clone the config to prevent external modifications
        _config = config.Clone();

        // Set up HttpClient
        if (httpClient == null)
        {
            _httpClient = new HttpClient();
            _ownsHttpClient = true;
        }
        else
        {
            _httpClient = httpClient;
            _ownsHttpClient = false;
        }

        // Configure HttpClient
        ConfigureHttpClient();
    }

    /// <summary>
    /// Gets the configuration used by this client instance.
    /// </summary>
    public OpenRouterClientOptions Configuration => _config.Clone();

    private void ConfigureHttpClient()
    {
        // Set timeout
        _httpClient.Timeout = _config.Timeout;

        // Set required headers
        _httpClient.DefaultRequestHeaders.Clear();

        // Authorization header
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");

        if (!string.IsNullOrWhiteSpace(_config.UserAgent))
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _config.UserAgent);

        if (!string.IsNullOrWhiteSpace(_config.Referer))
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", _config.Referer);

        // Content type for JSON
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    /// <summary>
    /// Sends a chat completion request to the OpenRouter API.
    /// </summary>
    /// <param name="request">The chat completion request.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<ChatResponse> SendChatCompletionAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Set default model if not specified
        if (string.IsNullOrWhiteSpace(request.Model))
            request.Model = _config.DefaultModel;

        // Ensure stream is false for non-streaming
        request.Stream = false;

        var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(
            $"{_config.Endpoint}/chat/completions",
            content,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"OpenRouter API error ({response.StatusCode}): {errorContent}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ChatResponse>(responseJson);

        if (result == null)
            throw new InvalidOperationException("Failed to deserialize response from OpenRouter API.");

        return result;
    }

    /// <summary>
    /// Sends a streaming chat completion request to the OpenRouter API.
    /// </summary>
    /// <param name="request">The chat completion request.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An async enumerable of chat completion chunks.</returns>
    public async IAsyncEnumerable<ChatResponse> StreamChatCompletionAsync(
        ChatRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Set default model if not specified
        if (string.IsNullOrWhiteSpace(request.Model))
            request.Model = _config.DefaultModel;

        // Ensure stream is true for streaming
        request.Stream = true;

        var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_config.Endpoint}/chat/completions")
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"OpenRouter API error ({response.StatusCode}): {errorContent}");
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.StartsWith("data: "))
            {
                var data = line.Substring(6);
                if (data == "[DONE]")
                    break;

                ChatResponse? chunk = null;
                try
                {
                    chunk = JsonConvert.DeserializeObject<ChatResponse>(data);
                }
                catch (JsonException)
                {
                    // Skip invalid JSON
                    continue;
                }

                if (chunk != null)
                    yield return chunk;
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(OpenRouterClient));
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="OpenRouterClient"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                if (_ownsHttpClient)
                {
                    _httpClient?.Dispose();
                }
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Releases all resources used by the <see cref="OpenRouterClient"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}