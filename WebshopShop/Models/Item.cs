namespace WebshopShop.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int ManufacturerId { get; set; }

        public Category Category { get; set; } = null!;
        public Manufacturer Manufacturer { get; set; } = null!;

        public ICollection<Picture> Pictures { get; set; } = [];
        public ICollection<CartItem> CartItems { get; set; } = [];
        public ICollection<ItemInvoice> ItemInvoices { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}