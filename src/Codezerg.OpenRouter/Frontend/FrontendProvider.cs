using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Frontend
{
    /// <summary>
    /// Represents a provider from the OpenRouter frontend API
    /// </summary>
    public class FrontendProvider
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("baseUrl")]
        public string BaseUrl { get; set; }

        [JsonProperty("dataPolicy")]
        public DataPolicy DataPolicy { get; set; }

        [JsonProperty("headquarters")]
        public string Headquarters { get; set; }

        [JsonProperty("datacenters")]
        public List<string> Datacenters { get; set; }

        [JsonProperty("hasChatCompletions")]
        public bool HasChatCompletions { get; set; }

        [JsonProperty("hasCompletions")]
        public bool HasCompletions { get; set; }

        [JsonProperty("isAbortable")]
        public bool IsAbortable { get; set; }

        [JsonProperty("moderationRequired")]
        public bool ModerationRequired { get; set; }

        [JsonProperty("adapterName")]
        public string AdapterName { get; set; }

        [JsonProperty("isMultipartSupported")]
        public bool IsMultipartSupported { get; set; }

        [JsonProperty("statusPageUrl")]
        public string StatusPageUrl { get; set; }

        [JsonProperty("byokEnabled")]
        public bool ByokEnabled { get; set; }

        [JsonProperty("icon")]
        public ProviderIcon Icon { get; set; }

        [JsonProperty("ignoredProviderModels")]
        public List<string> IgnoredProviderModels { get; set; }
    }

    public class DataPolicy
    {
        [JsonProperty("training")]
        public bool Training { get; set; }

        [JsonProperty("retainsPrompts")]
        public bool RetainsPrompts { get; set; }

        [JsonProperty("retentionDays")]
        public int? RetentionDays { get; set; }

        [JsonProperty("canPublish")]
        public bool CanPublish { get; set; }

        [JsonProperty("termsOfServiceURL")]
        public string TermsOfServiceUrl { get; set; }

        [JsonProperty("privacyPolicyURL")]
        public string PrivacyPolicyUrl { get; set; }

        [JsonProperty("requiresUserIDs")]
        public bool? RequiresUserIds { get; set; }
    }

    public class ProviderIcon
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("className")]
        public string ClassName { get; set; }
    }

    public class FrontendProviderResponse
    {
        [JsonProperty("data")]
        public List<FrontendProvider> Data { get; set; }
    }
}