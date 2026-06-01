using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebshopShop.Data;
using WebshopShop.Models;
using WebshopShop.ViewModels;

namespace WebshopShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NewsController(ApplicationDbContext db) : Controller
    {
        private readonly ApplicationDbContext _db = db;

        // List
        public async Task<IActionResult> Index()
        {
            var news = await _db.News
                .OrderByDescending(n => n.Created)
                .ToListAsync();

            var vm = new NewsListViewModel
            {
                Items = news.Select(n => new NewsItemViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Body = n.Body,
                    Created = n.Created
                }).ToList()
            };

            return View(vm);
        }

        // Create
        public IActionResult Create() => View(new NewsFormViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            _db.News.Add(new News
            {
                Title = vm.Title,
                Body = vm.Body,
                Created = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Edit
        public async Task<IActionResult> Edit(int id)
        {
            var news = await _db.News.FindAsync(id);
            if (news is null) return NotFound();

            var vm = new NewsFormViewModel
            {
                Id = news.Id,
                Title = news.Title,
                Body = news.Body
            };

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NewsFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var news = await _db.News.FindAsync(vm.Id);
            if (news is null) return NotFound();

            news.Title = vm.Title;
            news.Body = vm.Body;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Delete
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _db.News.FindAsync(id);
            if (news is not null)
            {
                _db.News.Remove(news);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
