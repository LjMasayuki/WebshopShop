using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebshopShop.ViewModels.Admin
{
    public class ItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int ManufacturerId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } = [];
        public IEnumerable<SelectListItem> Manufacturers { get; set; } = [];
    }
}