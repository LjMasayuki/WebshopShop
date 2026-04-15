namespace WebshopShop.Models
{
    public class ItemInvoice
    {
        public int ItemId { get; set; }
        public int InvoiceId { get; set; }
        public decimal Price { get; set; }

        public Item Item { get; set; } = null!;
        public Invoice Invoice { get; set; } = null!;
    }
}