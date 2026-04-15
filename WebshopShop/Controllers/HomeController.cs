using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebshopShop.Data;
using WebshopShop.ViewModels;

namespace WebshopShop.Controllers
{
    public class HomeController(ApplicationDbContext db) : Controller
    {
        private readonly ApplicationDbContext _db = db;

        public async Task<IActionResult> Index(int? categoryId)
        {
            var itemsQuery = _db.Items
                .Include(i => i.Category)
                .Include(i => i.Manufacturer)
                .Include(i => i.Reviews)
                .AsQueryable();

            if (categoryId.HasValue)
                itemsQuery = itemsQuery.Where(i => i.CategoryId == categoryId.Value);

            var items = await itemsQuery.ToListAsync();
            var categories = await _db.Categories.ToListAsync();
            var reviews = await _db.Reviews
                .Include(r => r.Item)
                .OrderByDescending(r => r.Id)
                .Take(3)
                .ToListAsync();

            var allReviews = await _db.Reviews.ToListAsync();

            var vm = new HomeIndexViewModel
            {
                Items = items,
                Categories = categories,
                Reviews = reviews,
                AverageScore = allReviews.Count > 0 ? Math.Round(allReviews.Average(r => (double)r.Score), 1) : 0,
                ReviewCount = allReviews.Count,
                ActiveCategoryId = categoryId
            };

            return View(vm);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}