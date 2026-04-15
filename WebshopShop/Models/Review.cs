using Microsoft.AspNetCore.Identity;

namespace WebshopShop.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public decimal Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public IdentityUser User { get; set; } = null!;
        public Item Item { get; set; } = null!;
    }
}