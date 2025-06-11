using System.ComponentModel.DataAnnotations;

namespace CurrencyExchangeTracker.Core.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(7)]
        public string CurrencyPair { get; set; } = string.Empty;

        [Required]
        public decimal CurrentRate { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}