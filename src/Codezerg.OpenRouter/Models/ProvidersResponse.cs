using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class ProvidersResponse
{
    [JsonProperty("data")]
    public List<ProviderInfo> Data { get; set; } = new List<ProviderInfo>();
}

public class ProviderInfo
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("slug")]
    public string Slug { get; set; } = string.Empty;

    [JsonProperty("privacy_policy_url")]
    public string? PrivacyPolicyUrl { get; set; }

    [JsonProperty("terms_of_service_url")]
    public string? TermsOfServiceUrl { get; set; }

    [JsonProperty("status_page_url")]
    public string? StatusPageUrl { get; set; }
}