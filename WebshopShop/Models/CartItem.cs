namespace WebshopShop.Models
{
    public class CartItem
    {
        public int CartId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; } = 1;

        public Cart Cart { get; set; } = null!;
        public Item Item { get; set; } = null!;
    }
}