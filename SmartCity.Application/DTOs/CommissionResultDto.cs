namespace SmartCity.Application.DTOs
{
    public class CommissionResultDto
    {
        public int CommissionRateId { get; set; }
        public string SlabLabel { get; set; } = string.Empty;
        public decimal RatePercentage { get; set; }
        public decimal CommissionAmount { get; set; }
    }
}
