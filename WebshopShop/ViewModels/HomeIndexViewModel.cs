using WebshopShop.Models;

namespace WebshopShop.ViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Item> Items { get; set; } = [];
        public IEnumerable<Review> Reviews { get; set; } = [];
        public IEnumerable<Category> Categories { get; set; } = [];
        public double AverageScore { get; set; }
        public int ReviewCount { get; set; }
        public int? ActiveCategoryId { get; set; }
    }
}