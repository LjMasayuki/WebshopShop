namespace WebshopShop.ViewModels
{
    public class ItemDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string ManufacturerName { get; set; } = string.Empty;
        public List<int> PictureIds { get; set; } = new();

        // Reviews
        public List<ReviewViewModel> Reviews { get; set; } = new();
        public decimal AverageScore { get; set; }
        public int ReviewCount { get; set; }

        // Submission form
        public ReviewSubmitViewModel NewReview { get; set; } = new();
    }

    public class ReviewViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ReviewSubmitViewModel
    {
        public int ItemId { get; set; }
        public decimal Score { get; set; }
        public string? Comment { get; set; }
    }
}