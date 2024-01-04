using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace E_Commerce.Models.Domain
{
        public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            public string Name { get; set; }
        // Original price of the product before any promotion
            public float OriginalPrice { get; set; }

        // Discounted price during the promotion period
            public float? DiscountedPrice { get; set; }
            public string Description { get; set; }
            public int Stock { get; set; }
            public DateTime Date { get; set; } = DateTime.Now; // Initialize with the current date
            // Foreign key referencing the SubCategory
            public int SubCategoryId { get; set; }
        // Navigation property to the SubCategory
            [JsonIgnore]

            public SubCategory? SubCategory { get; set; }
            public string Color { get; set; } // Color of the clothing
            public List<string>  Size { get; set; } // Size of the clothing (S, M, L, XL, etc.)
            public string Material { get; set; } 
            public string Composition { get; set; }
            public string Col { get; set; }
            public bool Promo { get; set; }
           // public bool Onsale { get; set; }
            public List<string> ImageUrls { get; set; } = new List<string>();

 }
    }
