namespace ShoppingApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string ProfilePictureUrl { get; set; }
        public bool IsAdmin { get; set; }
        public string Role { get; set; } // Added to distinguish between Seller and Buyer

        public ICollection<Item> ItemsForSale { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
