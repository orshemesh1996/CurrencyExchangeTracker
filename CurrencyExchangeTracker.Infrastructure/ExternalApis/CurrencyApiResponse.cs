using System.Text.Json.Serialization;

namespace CurrencyExchangeTracker.Infrastructure.ExternalApis
{
    public class CurrencyApiResponse
    {
        [JsonPropertyName("result")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> CurrencyRates { get; set; } = new();

        [JsonPropertyName("base")]
        public string BaseCurrency { get; set; } = string.Empty;
    }
}