namespace CurrencyExchangeTracker.Api.Models
{
    public class CurrencyRateDto
    {
        public string CurrencyPair { get; set; } = string.Empty;
        public decimal CurrentRate { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string FormattedRate => $"{CurrentRate:N4}";
        public string TimeSinceUpdate => GetTimeSinceUpdate();

        private string GetTimeSinceUpdate()
        {
            var timeDiff = DateTime.UtcNow - UpdatedAt;
            if (timeDiff.TotalMinutes < 1)
                return "Just now";
            if (timeDiff.TotalHours < 1)
                return $"{(int)timeDiff.TotalMinutes} minutes ago";
            return $"{(int)timeDiff.TotalHours} hours ago";
        }
    }
}