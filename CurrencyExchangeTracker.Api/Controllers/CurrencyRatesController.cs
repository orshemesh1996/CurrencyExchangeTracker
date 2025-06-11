using Microsoft.AspNetCore.Mvc;
using CurrencyExchangeTracker.Core.Contracts;
using CurrencyExchangeTracker.Api.Models;

namespace CurrencyExchangeTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyRatesController : ControllerBase
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ILogger<CurrencyRatesController> _logger;

        public CurrencyRatesController(
            IExchangeRateService exchangeRateService,
            ILogger<CurrencyRatesController> logger)
        {
            _exchangeRateService = exchangeRateService;
            _logger = logger;
        }

        /// <summary>
        /// Get all available exchange rates
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<CurrencyRateDto>>> GetAllRates()
        {
            try
            {
                var rates = await _exchangeRateService.GetAllRatesAsync();
                var ratesDtos = rates.Select(r => new CurrencyRateDto
                {
                    CurrencyPair = r.CurrencyPair,
                    CurrentRate = r.CurrentRate,
                    UpdatedAt = r.UpdatedAt
                }).ToList();

                return Ok(ratesDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all exchange rates");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get exchange rate for a specific currency pair
        /// </summary>
        /// <param name="currencyPair">Currency pair (e.g., USD/ILS)</param>
        [HttpGet("{currencyPair}")]
        public async Task<ActionResult<CurrencyRateDto>> GetRateByPair(string currencyPair)
        {
            try
            {
                var formattedPair = currencyPair.Replace("-", "/").ToUpper();
                var rate = await _exchangeRateService.GetRateByPairAsync(formattedPair);

                if (rate == null)
                {
                    return NotFound($"Exchange rate for pair '{currencyPair}' not found");
                }

                var rateDto = new CurrencyRateDto
                {
                    CurrencyPair = rate.CurrencyPair,
                    CurrentRate = rate.CurrentRate,
                    UpdatedAt = rate.UpdatedAt
                };

                return Ok(rateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exchange rate for pair {CurrencyPair}", currencyPair);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                Status = "Healthy",
                ServerTime = DateTime.Now,
                Message = "Currency exchange tracker service is running"
            });
        }
    }
}