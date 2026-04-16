namespace WebshopShop.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.Price * i.Quantity);
    }

    public class CartItemViewModel
    {
        public int ItemId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int? PictureId { get; set; }
    }
}