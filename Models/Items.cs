using System.ComponentModel.DataAnnotations;  // For data annotations

namespace ShoppingApp.Models
{
    public class Items
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        [StringLength(100, ErrorMessage = "Item name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int QuantityAvailable { get; set; }

        public int SellerId { get; set; }
        //public Users Seller { get; set; }
    }
}
