using CurrencyExchangeTracker.Core.Models;

namespace CurrencyExchangeTracker.Core.Contracts
{
    public interface ICurrencyApiClient
    {
        Task<List<ExchangeRate>> FetchCurrentRatesAsync(List<string> currencyPairs);
    }
}