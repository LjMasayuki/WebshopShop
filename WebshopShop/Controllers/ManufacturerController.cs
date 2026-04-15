using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebshopShop.Data;
using WebshopShop.Models;
using WebshopShop.ViewModels.Admin;

namespace WebshopShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManufacturerController(ApplicationDbContext db) : Controller
    {
        private readonly ApplicationDbContext _db = db;

        public async Task<IActionResult> Index()
        {
            var manufacturers = await _db.Manufacturers
                .Include(m => m.Items)
                .ToListAsync();
            return View(manufacturers);
        }

        public IActionResult Create() => View(new ManufacturerViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ManufacturerViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            _db.Manufacturers.Add(new Manufacturer { Name = vm.Name });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var manufacturer = await _db.Manufacturers.FindAsync(id);
            if (manufacturer is null) return NotFound();
            return View(new ManufacturerViewModel { Id = manufacturer.Id, Name = manufacturer.Name });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ManufacturerViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var manufacturer = await _db.Manufacturers.FindAsync(vm.Id);
            if (manufacturer is null) return NotFound();

            manufacturer.Name = vm.Name;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var manufacturer = await _db.Manufacturers.FindAsync(id);
            if (manufacturer is not null)
            {
                _db.Manufacturers.Remove(manufacturer);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}