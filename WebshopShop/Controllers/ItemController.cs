using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebshopShop.Data;
using WebshopShop.Models;
using WebshopShop.ViewModels.Admin;

namespace WebshopShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ItemController(ApplicationDbContext db) : Controller
    {
        private readonly ApplicationDbContext _db = db;

        public async Task<IActionResult> Index()
        {
            var items = await _db.Items
                .Include(i => i.Category)
                .Include(i => i.Manufacturer)
                .ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> Create()
        {
            return View(await BuildItemViewModelAsync(new ItemViewModel()));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(await BuildItemViewModelAsync(vm));

            _db.Items.Add(new Item
            {
                Title = vm.Title,
                Description = vm.Description,
                Price = vm.Price,
                CategoryId = vm.CategoryId,
                ManufacturerId = vm.ManufacturerId
            });

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Items.FindAsync(id);
            if (item is null) return NotFound();

            var vm = new ItemViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Price = item.Price,
                CategoryId = item.CategoryId,
                ManufacturerId = item.ManufacturerId
            };

            return View(await BuildItemViewModelAsync(vm));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ItemViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(await BuildItemViewModelAsync(vm));

            var item = await _db.Items.FindAsync(vm.Id);
            if (item is null) return NotFound();

            item.Title = vm.Title;
            item.Description = vm.Description;
            item.Price = vm.Price;
            item.CategoryId = vm.CategoryId;
            item.ManufacturerId = vm.ManufacturerId;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Items.FindAsync(id);
            if (item is not null)
            {
                _db.Items.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<ItemViewModel> BuildItemViewModelAsync(ItemViewModel vm)
        {
            vm.Categories = await _db.Categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();

            vm.Manufacturers = await _db.Manufacturers
                .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name })
                .ToListAsync();

            return vm;
        }
    }
}