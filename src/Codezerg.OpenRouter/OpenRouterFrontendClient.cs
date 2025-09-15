using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Codezerg.OpenRouter.Frontend;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter
{
    /// <summary>
    /// Client for accessing OpenRouter's frontend/private API endpoints.
    /// WARNING: These are undocumented private APIs that may change without notice.
    /// Use at your own risk in production environments.
    /// </summary>
    public class OpenRouterFrontendClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly bool _ownsHttpClient;
        private readonly JsonSerializerSettings _jsonSettings;
        private bool _disposed;
        private const string BaseUrl = "https://openrouter.ai";

        /// <summary>
        /// Initializes a new instance of the OpenRouterFrontendClient.
        /// </summary>
        /// <param name="httpClient">Optional HttpClient instance. If not provided, a new one will be created.</param>
        public OpenRouterFrontendClient(HttpClient httpClient = null)
        {
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

            _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
        }

        /// <summary>
        /// Gets all available providers from the frontend API.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of providers</returns>
        public async Task<List<FrontendProvider>> GetProvidersAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetAsync<FrontendProviderResponse>("/api/frontend/all-providers", cancellationToken);
            return response?.Data ?? new List<FrontendProvider>();
        }

        /// <summary>
        /// Gets all available models from the frontend API.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of models</returns>
        public async Task<List<FrontendModel>> GetModelsAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetAsync<FrontendModelResponse>("/api/frontend/models", cancellationToken);
            if (response?.Data == null) return new List<FrontendModel>();

            // Handle both response formats
            if (response.Data is Newtonsoft.Json.Linq.JArray array)
            {
                return array.ToObject<List<FrontendModel>>();
            }
            else if (response.Data is FrontendModelData modelData)
            {
                return modelData.Models ?? new List<FrontendModel>();
            }

            return new List<FrontendModel>();
        }

        /// <summary>
        /// Finds models based on specific criteria.
        /// </summary>
        /// <param name="order">Order criteria (e.g., "top-weekly", "top-daily")</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of models</returns>
        public async Task<List<FrontendModel>> FindModelsAsync(string order = "top-weekly", CancellationToken cancellationToken = default)
        {
            var endpoint = $"/api/frontend/models/find?order={Uri.EscapeDataString(order)}";
            var response = await GetAsync<FrontendModelResponse>(endpoint, cancellationToken);
            if (response?.Data == null) return new List<FrontendModel>();

            // Handle both response formats
            if (response.Data is Newtonsoft.Json.Linq.JArray array)
            {
                return array.ToObject<List<FrontendModel>>();
            }
            else if (response.Data is Newtonsoft.Json.Linq.JObject obj)
            {
                var modelData = obj.ToObject<FrontendModelData>();
                return modelData?.Models ?? new List<FrontendModel>();
            }

            return new List<FrontendModel>();
        }

        /// <summary>
        /// Gets statistics for a specific model endpoint.
        /// </summary>
        /// <param name="permaslug">Model permaslug (e.g., "openai/gpt-4")</param>
        /// <param name="variant">Variant type (default: "standard")</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Model endpoint statistics</returns>
        public async Task<ModelEndpointStats> GetModelStatsAsync(string permaslug, string variant = "standard", CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(permaslug))
                throw new ArgumentException("Permaslug is required", nameof(permaslug));

            var endpoint = $"/api/frontend/stats/endpoint?permaslug={Uri.EscapeDataString(permaslug)}&variant={Uri.EscapeDataString(variant)}";
            var response = await GetAsync<FrontendModelStatsResponse>(endpoint, cancellationToken);
            return response?.Data?.FirstOrDefault();
        }

        /// <summary>
        /// Gets models by provider.
        /// </summary>
        /// <param name="providerSlug">Provider slug (e.g., "openai", "anthropic")</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of models from the specified provider</returns>
        public async Task<List<FrontendModel>> GetModelsByProviderAsync(string providerSlug, CancellationToken cancellationToken = default)
        {
            var allModels = await GetModelsAsync(cancellationToken);
            return allModels.Where(m => m.Endpoint?.ProviderSlug == providerSlug).ToList();
        }

        /// <summary>
        /// Gets models that support specific modalities.
        /// </summary>
        /// <param name="inputModality">Required input modality (e.g., "image", "text")</param>
        /// <param name="outputModality">Required output modality (optional)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of models supporting the specified modalities</returns>
        public async Task<List<FrontendModel>> GetModelsByModalityAsync(
            string inputModality = null,
            string outputModality = null,
            CancellationToken cancellationToken = default)
        {
            var allModels = await GetModelsAsync(cancellationToken);

            var filtered = allModels.AsEnumerable();

            if (!string.IsNullOrEmpty(inputModality))
                filtered = filtered.Where(m => m.InputModalities?.Contains(inputModality) == true);

            if (!string.IsNullOrEmpty(outputModality))
                filtered = filtered.Where(m => m.OutputModalities?.Contains(outputModality) == true);

            return filtered.ToList();
        }

        /// <summary>
        /// Gets models that support reasoning (chain-of-thought).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of reasoning-capable models</returns>
        public async Task<List<FrontendModel>> GetReasoningModelsAsync(CancellationToken cancellationToken = default)
        {
            var allModels = await GetModelsAsync(cancellationToken);
            return allModels.Where(m => m.ReasoningConfig != null || m.Endpoint?.SupportsReasoning == true).ToList();
        }

        /// <summary>
        /// Gets free models.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of free models</returns>
        public async Task<List<FrontendModel>> GetFreeModelsAsync(CancellationToken cancellationToken = default)
        {
            var allModels = await GetModelsAsync(cancellationToken);
            return allModels.Where(m => m.Endpoint?.IsFree == true).ToList();
        }

        /// <summary>
        /// Gets providers that support bring-your-own-key (BYOK).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of BYOK-enabled providers</returns>
        public async Task<List<FrontendProvider>> GetByokProvidersAsync(CancellationToken cancellationToken = default)
        {
            var providers = await GetProvidersAsync(cancellationToken);
            return providers.Where(p => p.ByokEnabled).ToList();
        }

        /// <summary>
        /// Gets providers with privacy-friendly policies (no training, no retention).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of privacy-focused providers</returns>
        public async Task<List<FrontendProvider>> GetPrivacyFriendlyProvidersAsync(CancellationToken cancellationToken = default)
        {
            var providers = await GetProvidersAsync(cancellationToken);
            return providers.Where(p =>
                p.DataPolicy != null &&
                !p.DataPolicy.Training &&
                !p.DataPolicy.RetainsPrompts).ToList();
        }

        private async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            var url = $"{BaseUrl}{endpoint}";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                // Add common headers for frontend API
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "Codezerg.OpenRouter/1.0");

                using (var response = await _httpClient.SendAsync(request, cancellationToken))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Frontend API request failed with status {response.StatusCode}: {content}");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(OpenRouterFrontendClient));
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="OpenRouterFrontendClient"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_ownsHttpClient)
                    {
                        _httpClient?.Dispose();
                    }
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}