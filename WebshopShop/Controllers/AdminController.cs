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
    public class AdminController(ApplicationDbContext db) : Controller
    {
        private readonly ApplicationDbContext _db = db;

        // ─── Items ────────────────────────────────────────────────────────────

        public async Task<IActionResult> Items()
        {
            var items = await _db.Items
                .Include(i => i.Category)
                .Include(i => i.Manufacturer)
                .ToListAsync();
            return View("Item/Index", items);
        }

        public async Task<IActionResult> CreateItem()
        {
            return View("Item/Create", await BuildItemViewModelAsync(new ItemViewModel()));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem(ItemViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Item/Create", await BuildItemViewModelAsync(vm));

            _db.Items.Add(new Item
            {
                Title = vm.Title,
                Description = vm.Description,
                Price = vm.Price,
                CategoryId = vm.CategoryId,
                ManufacturerId = vm.ManufacturerId
            });

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Items));
        }

        public async Task<IActionResult> EditItem(int id)
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

            return View("Item/Edit", await BuildItemViewModelAsync(vm));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(ItemViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Item/Edit", await BuildItemViewModelAsync(vm));

            var item = await _db.Items.FindAsync(vm.Id);
            if (item is null) return NotFound();

            item.Title = vm.Title;
            item.Description = vm.Description;
            item.Price = vm.Price;
            item.CategoryId = vm.CategoryId;
            item.ManufacturerId = vm.ManufacturerId;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Items));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _db.Items.FindAsync(id);
            if (item is not null)
            {
                _db.Items.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Items));
        }

        // ─── Categories ───────────────────────────────────────────────────────

        public async Task<IActionResult> Categories()
        {
            var categories = await _db.Categories
                .Include(c => c.Items)
                .ToListAsync();
            return View("Category/Index", categories);
        }

        public IActionResult CreateCategory() => View("Category/Create", new CategoryViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryViewModel vm)
        {
            if (!ModelState.IsValid) return View("Category/Create", vm);

            _db.Categories.Add(new Category { Name = vm.Name });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Categories));
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category is null) return NotFound();
            return View("Category/Edit", new CategoryViewModel { Id = category.Id, Name = category.Name });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(CategoryViewModel vm)
        {
            if (!ModelState.IsValid) return View("Category/Edit", vm);

            var category = await _db.Categories.FindAsync(vm.Id);
            if (category is null) return NotFound();

            category.Name = vm.Name;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category is not null)
            {
                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Categories));
        }

        // ─── Manufacturers ────────────────────────────────────────────────────

        public async Task<IActionResult> Manufacturers()
        {
            var manufacturers = await _db.Manufacturers
                .Include(m => m.Items)
                .ToListAsync();
            return View("Manufacturer/Index", manufacturers);
        }

        public IActionResult CreateManufacturer() => View("Manufacturer/Create", new ManufacturerViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateManufacturer(ManufacturerViewModel vm)
        {
            if (!ModelState.IsValid) return View("Manufacturer/Create", vm);

            _db.Manufacturers.Add(new Manufacturer { Name = vm.Name });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Manufacturers));
        }

        public async Task<IActionResult> EditManufacturer(int id)
        {
            var manufacturer = await _db.Manufacturers.FindAsync(id);
            if (manufacturer is null) return NotFound();
            return View("Manufacturer/Edit", new ManufacturerViewModel { Id = manufacturer.Id, Name = manufacturer.Name });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditManufacturer(ManufacturerViewModel vm)
        {
            if (!ModelState.IsValid) return View("Manufacturer/Edit", vm);

            var manufacturer = await _db.Manufacturers.FindAsync(vm.Id);
            if (manufacturer is null) return NotFound();

            manufacturer.Name = vm.Name;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Manufacturers));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteManufacturer(int id)
        {
            var manufacturer = await _db.Manufacturers.FindAsync(id);
            if (manufacturer is not null)
            {
                _db.Manufacturers.Remove(manufacturer);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Manufacturers));
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

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