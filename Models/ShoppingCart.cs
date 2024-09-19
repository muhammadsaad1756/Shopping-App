namespace ShoppingApp.Models
{
    public class ShoppingCart 
    {
        public int Id { get; set; }
        public int UserId { get; set; }
       // public int CartItemId { get; set; }
        //public int BuyerId { get; set; }
        public ICollection<CartItem> CartItems { get; set; }

    }
}
