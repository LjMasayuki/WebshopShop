namespace WebshopShop.ViewModels
{
    public class OrderHistoryViewModel
    {
        public List<OrderHistoryItemViewModel> Orders { get; set; } = [];
    }

    public class OrderHistoryItemViewModel
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime IssuedDate { get; set; }
        public decimal Total { get; set; }
        public int ItemCount { get; set; }
        public List<string> ItemTitles { get; set; } = [];
    }
}