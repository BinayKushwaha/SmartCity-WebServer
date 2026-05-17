namespace SmartCity.Application.Settings
{
    public class CacheSettings
    {
        public const string SectionName = "CacheSettings";

        public int CommissionRatesExpiryMinutes { get; set; } = 30;
        public int SlidingExpiryMinutes { get; set; } = 2;
        public int DefaultAbsoluteExpiryMinutes { get; set; } = 15;
    }
}
