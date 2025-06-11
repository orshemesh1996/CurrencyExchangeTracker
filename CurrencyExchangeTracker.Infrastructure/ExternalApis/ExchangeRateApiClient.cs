using System.Text.Json;
using Microsoft.Extensions.Logging;
using CurrencyExchangeTracker.Core.Contracts;
using CurrencyExchangeTracker.Core.Models;

namespace CurrencyExchangeTracker.Infrastructure.ExternalApis
{
    public class ExchangeRateApiClient : ICurrencyApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExchangeRateApiClient> _logger;

        public ExchangeRateApiClient(HttpClient httpClient, ILogger<ExchangeRateApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<ExchangeRate>> FetchCurrentRatesAsync(List<string> currencyPairs)
        {
            var exchangeRates = new List<ExchangeRate>();
            var fetchTime = DateTime.UtcNow;

            try
            {
                var baseCurrencies = currencyPairs.Select(p => p.Split('/')[0]).Distinct();

                foreach (var baseCurrency in baseCurrencies)
                {
                    var apiUrl = $"https://api.exchangerate-api.com/v4/latest/{baseCurrency}";

                    _logger.LogInformation("Fetching rates for base currency: {BaseCurrency}", baseCurrency);

                    var response = await _httpClient.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();

                    var jsonData = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<CurrencyApiResponse>(jsonData);

                    if (apiResponse?.CurrencyRates != null)
                    {
                        var relevantPairs = currencyPairs.Where(p => p.StartsWith($"{baseCurrency}/"));

                        foreach (var pair in relevantPairs)
                        {
                            var targetCurrency = pair.Split('/')[1];

                            if (apiResponse.CurrencyRates.TryGetValue(targetCurrency, out var rate))
                            {
                                exchangeRates.Add(new ExchangeRate
                                {
                                    CurrencyPair = pair,
                                    CurrentRate = rate,
                                    UpdatedAt = fetchTime,
                                    RecordedAt = fetchTime
                                });
                            }
                        }
                    }
                }

                _logger.LogInformation("Successfully fetched {Count} exchange rates", exchangeRates.Count);

        
                _logger.LogInformation("Rates fetched from API:");
                foreach (var rate in exchangeRates)
                {
                    _logger.LogInformation("CurrencyPair: {Pair}, Rate: {Rate}, Time: {Time}",
                        rate.CurrencyPair, rate.CurrentRate, rate.UpdatedAt.ToString("HH:mm:ss"));
                }

                return exchangeRates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching exchange rates");
                throw;
            }
        }
    }
}
