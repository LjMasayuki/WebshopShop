using Microsoft.AspNetCore.Identity;

namespace WebshopShop.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;

        public IdentityUser User { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; } = [];
    }
}