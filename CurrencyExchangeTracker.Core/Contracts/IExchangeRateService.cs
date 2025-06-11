using CurrencyExchangeTracker.Core.Models;

namespace CurrencyExchangeTracker.Core.Contracts
{
    public interface IExchangeRateService
    {
        Task<List<ExchangeRate>> GetAllRatesAsync();
        Task<ExchangeRate?> GetRateByPairAsync(string currencyPair);
        Task SaveRatesAsync(List<ExchangeRate> rates);
        Task UpdateRateAsync(ExchangeRate rate);
    }
}