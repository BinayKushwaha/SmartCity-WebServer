using SmartCity.Domain;
using System.ComponentModel.DataAnnotations;

namespace SmartCity.Application.DTOs
{
    public class RetailPropertyRequestDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        [StringLength(100)]
        public PropertyType Type { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string? Features { get; set; }
    }
}
