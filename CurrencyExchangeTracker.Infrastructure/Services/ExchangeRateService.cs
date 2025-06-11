using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CurrencyExchangeTracker.Core.Contracts;
using CurrencyExchangeTracker.Core.Models;
using CurrencyExchangeTracker.Infrastructure.Database;

namespace CurrencyExchangeTracker.Infrastructure.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly CurrencyDbContext _dbContext;
        private readonly ILogger<ExchangeRateService> _logger;

        public ExchangeRateService(CurrencyDbContext dbContext, ILogger<ExchangeRateService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<ExchangeRate>> GetAllRatesAsync()
        {
            return await _dbContext.ExchangeRates
                .OrderBy(r => r.CurrencyPair)
                .ToListAsync();
        }

        public async Task<ExchangeRate?> GetRateByPairAsync(string currencyPair)
        {
            return await _dbContext.ExchangeRates
                .FirstOrDefaultAsync(e => e.CurrencyPair.ToUpper() == currencyPair.ToUpper());
        }


        public async Task SaveRatesAsync(List<ExchangeRate> rates)
        {
            foreach (var rate in rates)
            {
                var existingRate = await GetRateByPairAsync(rate.CurrencyPair);

                if (existingRate != null)
                {
                    existingRate.CurrentRate = rate.CurrentRate;
                    existingRate.UpdatedAt = rate.UpdatedAt;
                    _dbContext.ExchangeRates.Update(existingRate);
                }
                else
                {
                    await _dbContext.ExchangeRates.AddAsync(rate);
                }
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Updated {Count} exchange rates in database", rates.Count);
        }

        public async Task UpdateRateAsync(ExchangeRate rate)
        {
            var existingRate = await GetRateByPairAsync(rate.CurrencyPair);

            if (existingRate != null)
            {
                existingRate.CurrentRate = rate.CurrentRate;
                existingRate.UpdatedAt = rate.UpdatedAt;
                _dbContext.ExchangeRates.Update(existingRate);
            }
            else
            {
                await _dbContext.ExchangeRates.AddAsync(rate);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}