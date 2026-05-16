using SmartCity.Domain;
using System.ComponentModel.DataAnnotations;

namespace SmartCity.Application.DTOs
{
    public class RetailPropertyDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        [StringLength(100, ErrorMessage = "Type cannot exceed 100 characters.")]
        public PropertyType Type { get; set; } 

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [StringLength(500, ErrorMessage = "Features cannot exceed 500 characters.")]
        public string? Features { get; set; }
    };
}
