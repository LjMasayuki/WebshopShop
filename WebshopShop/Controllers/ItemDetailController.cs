using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebshopShop.Data;
using WebshopShop.Models;
using WebshopShop.ViewModels;

namespace WebshopShop.Controllers
{
    public class ItemDetailController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ItemDetailController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int id)
        {
            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Manufacturer)
                .Include(i => i.Pictures)
                .Include(i => i.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null) return NotFound();

            var reviews = item.Reviews.Select(r => new ReviewViewModel
            {
                UserName = r.User?.UserName ?? "Anonymous",
                Score = r.Score,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).OrderByDescending(r => r.CreatedAt).ToList();

            var vm = new ItemDetailViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Price = item.Price,
                CategoryName = item.Category?.Name ?? string.Empty,
                ManufacturerName = item.Manufacturer?.Name ?? string.Empty,
                PictureIds = item.Pictures.Select(p => p.Id).ToList(),
                Reviews = reviews,
                AverageScore = reviews.Count > 0 ? Math.Round(reviews.Average(r => r.Score), 1) : 0,
                ReviewCount = reviews.Count,
                NewReview = new ReviewSubmitViewModel { ItemId = id }
            };

            return View(vm);
        }

        public async Task<IActionResult> GetImage(int id)
        {
            var picture = await _context.Pictures.FindAsync(id);
            if (picture == null) return NotFound();

            return File(picture.Data, picture.ContentType);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReview(ReviewSubmitViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", new { id = model.ItemId });

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.ItemId == model.ItemId && r.UserId == user.Id);

            if (!alreadyReviewed)
            {
                var review = new Review
                {
                    ItemId = model.ItemId,
                    UserId = user.Id,
                    Score = Math.Clamp(model.Score, 1, 5),
                    Comment = model.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { id = model.ItemId });
        }
    }
}