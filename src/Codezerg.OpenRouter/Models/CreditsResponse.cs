using Newtonsoft.Json;

namespace Codezerg.OpenRouter.Models;

public class CreditsResponse
{
    [JsonProperty("data")]
    public Credits Data { get; set; } = new Credits();
}

public class Credits
{
    [JsonProperty("total_credits")]
    public double TotalCredits { get; set; }

    [JsonProperty("total_usage")]
    public double TotalUsage { get; set; }
}