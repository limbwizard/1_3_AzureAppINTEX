using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzureAppINTEX.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Please enter a product name")]
        public string Name { get; set; }
        public int? Year { get; set; }
        public int? NumParts { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price")]
        [Column(TypeName = "decimal(8, 2)")]
        public decimal Price { get; set; }
        public string? ImgLink { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public ICollection<ProductRecommendation>? Recommendations { get; set; }
        public ICollection<LineItem>? LineItems { get; set; }
    }

}
