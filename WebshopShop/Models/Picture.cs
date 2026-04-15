namespace WebshopShop.Models
{
    public class Picture
    {
        public int Id { get; set; }
        public byte[] Data { get; set; } = [];
        public string ContentType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ItemId { get; set; }

        public Item Item { get; set; } = null!;
    }
}