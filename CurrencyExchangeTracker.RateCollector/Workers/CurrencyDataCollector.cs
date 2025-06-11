using Microsoft.Extensions.Options;
using CurrencyExchangeTracker.Core.Contracts;

namespace CurrencyExchangeTracker.RateCollector.Workers
{
    public class CurrencyDataCollector : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CurrencyDataCollector> _logger;
        private readonly CollectorSettings _settings;

        public CurrencyDataCollector(
            IServiceProvider serviceProvider,
            ILogger<CurrencyDataCollector> logger,
            IOptions<CollectorSettings> settings)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Currency rate collector service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var apiClient = scope.ServiceProvider.GetRequiredService<ICurrencyApiClient>();
                    var exchangeRateService = scope.ServiceProvider.GetRequiredService<IExchangeRateService>();

                    var latestRates = await apiClient.FetchCurrentRatesAsync(_settings.CurrencyPairs);
                    await exchangeRateService.SaveRatesAsync(latestRates);

                    _logger.LogInformation("Successfully updated {Count} exchange rates at {Time}",
                        latestRates.Count, DateTime.Now.ToString("HH:mm:ss"));
                }
                catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Service cancellation requested - shutting down gracefully");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating exchange rates - continuing...");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_settings.UpdateIntervalSeconds), stoppingToken);
                }
                catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Service shutdown requested during delay");
                    break;
                }
            }

            _logger.LogInformation("Currency rate collector service stopped");
        }
    }

    public class CollectorSettings
    {
        public List<string> CurrencyPairs { get; set; } = new();
        public int UpdateIntervalSeconds { get; set; } = 10;
    }
}