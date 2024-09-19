namespace ShoppingApp.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int QuantityAvailable { get; set; }

        public int SellerId { get; set; }
        //public int BuyerId { get; set; }
        public Users Seller { get; set; }

    }
}
