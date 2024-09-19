namespace ShoppingApp.Models
{
    public class CartItem 
    {
        public int Id { get; set; }
        public int ShoppingCartId { get; set; }

        public int ItemId { get; set; }
        public int BuyerId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        // Navigation property for the related Item
        public Item Item { get; set; } // This allows you to access the related Item
        // Navigation property for the ShoppingCart
        //public ShoppingCart ShoppingCart { get; set; }
    }
}
