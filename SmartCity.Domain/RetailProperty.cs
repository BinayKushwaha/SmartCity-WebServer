namespace SmartCity.Domain
{
    public class RetailProperty
    {
        public int Id { get; set; }
        public PropertyType Type { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Features { get; set; } = string.Empty;
        public string? BrokerId { get; set; }
        public int CommissionRateId { get; set; }
        public decimal CommissionAmount { get; set; }
        public ApplicationUser? Broker { get; set; }
        public CommissionRate CommissionRate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }
}
