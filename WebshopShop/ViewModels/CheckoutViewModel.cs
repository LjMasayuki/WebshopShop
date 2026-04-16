using System.ComponentModel.DataAnnotations;

namespace WebshopShop.ViewModels
{
    public class CheckoutViewModel
    {
        // Cart summary
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal Total { get; set; }

        // Address fields
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        public string ZipCode { get; set; } = string.Empty;

        public string? State { get; set; }

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string AddressLine1 { get; set; } = string.Empty;

        public string? AddressLine2 { get; set; }
    }
}