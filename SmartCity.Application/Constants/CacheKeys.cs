namespace SmartCity.Application.Constants
{
    public static class CacheKeys
    {
        public const string AllProperties = "all_properties";

        public static string GetPropertyById(int id)
            => $"property_{id}";
    }
}
