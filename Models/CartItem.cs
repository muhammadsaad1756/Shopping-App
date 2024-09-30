using System.ComponentModel.DataAnnotations;  // For data annotations

namespace ShoppingApp.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int ShoppingCartId { get; set; }

        public int ItemId { get; set; }

        public int BuyerId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public Items Item { get; set; }
    }
}
