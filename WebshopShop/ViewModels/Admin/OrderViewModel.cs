namespace WebshopShop.ViewModels.Admin
{
    public class OrderListViewModel
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime IssuedDate { get; set; }
        public int ItemCount { get; set; }
    }

    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime IssuedDate { get; set; }
        public AddressViewModel CustomerAddress { get; set; } = new();
        public List<OrderItemViewModel> Items { get; set; } = [];
    }

    public class OrderItemViewModel
    {
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class AddressViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }
}