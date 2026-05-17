namespace SmartCity.Application.Constants
{
    public static class CacheKeys
    {
        public const string AllProperties = "all_properties";
        
        public const string CommissionRates = "commission_rates";
        public static string GetPropertyById(int id)
            => $"property_{id}";
    }
}
