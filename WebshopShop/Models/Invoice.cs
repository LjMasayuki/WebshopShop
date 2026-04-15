using Microsoft.AspNetCore.Identity;

namespace WebshopShop.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CustomerAddressId { get; set; }
        public int SellerAddressId { get; set; }
        public decimal Total { get; set; }
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
        public string InvoiceNumber { get; set; } = string.Empty;

        public IdentityUser User { get; set; } = null!;
        public Address CustomerAddress { get; set; } = null!;
        public Address SellerAddress { get; set; } = null!;

        public ICollection<ItemInvoice> ItemInvoices { get; set; } = [];
    }
}