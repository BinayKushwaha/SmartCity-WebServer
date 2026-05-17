namespace SmartCity.Domain
{
    public class CommissionRate
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal RatePercentage { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }


        public ICollection<RetailProperty> Properties { get; set; }
            = new List<RetailProperty>();
    }
}
