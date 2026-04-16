using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebshopShop.Data;
using WebshopShop.Models;
using WebshopShop.ViewModels;
using WebshopShop.Documents;

namespace WebshopShop.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public InvoiceController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null || !await _context.CartItems.AnyAsync(ci => ci.CartId == cart.Id))
                return RedirectToAction("Index", "Cart");

            var cartItems = await _context.CartItems
                .Include(ci => ci.Item)
                    .ThenInclude(i => i.Pictures)
                .Include(ci => ci.Item)
                    .ThenInclude(i => i.Category)
                .Where(ci => ci.CartId == cart.Id)
                .ToListAsync();

            var vm = new CheckoutViewModel
            {
                Items = cartItems.Select(ci => new CartItemViewModel
                {
                    ItemId = ci.ItemId,
                    Title = ci.Item.Title,
                    Price = ci.Item.Price,
                    Quantity = ci.Quantity,
                    CategoryName = ci.Item.Category?.Name ?? string.Empty,
                    PictureId = ci.Item.Pictures.FirstOrDefault()?.Id
                }).ToList(),
                Total = cartItems.Sum(ci => ci.Item.Price * ci.Quantity)
            };

            return View("Checkout", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null) return RedirectToAction("Index", "Cart");

            var cartItems = await _context.CartItems
                .Include(ci => ci.Item)
                .Where(ci => ci.CartId == cart.Id)
                .ToListAsync();

            if (!cartItems.Any()) return RedirectToAction("Index", "Cart");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });

                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Field: {error.Key} — Errors: {string.Join(", ", error.Errors)}");
                }

                vm.Items = cartItems.Select(ci => new CartItemViewModel
                {
                    ItemId = ci.ItemId,
                    Title = ci.Item.Title,
                    Price = ci.Item.Price,
                    Quantity = ci.Quantity
                }).ToList();
                vm.Total = cartItems.Sum(ci => ci.Item.Price * ci.Quantity);
                return View("Checkout", vm);
            }

            // Create address
            var address = new Address
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Country = vm.Country,
                ZipCode = vm.ZipCode,
                State = vm.State ?? string.Empty,
                City = vm.City,
                AddressLine1 = vm.AddressLine1,
                AddressLine2 = vm.AddressLine2
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            // Create invoice
            var invoice = new Invoice
            {
                UserId = user.Id,
                CustomerAddressId = address.Id,
                SellerAddressId = address.Id,
                Total = cartItems.Sum(ci => ci.Item.Price * ci.Quantity),
                IssuedDate = DateTime.UtcNow,
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}"
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Create invoice items
            foreach (var ci in cartItems)
            {
                _context.ItemInvoices.Add(new ItemInvoice
                {
                    InvoiceId = invoice.Id,
                    ItemId = ci.ItemId,
                    Price = ci.Item.Price
                });
            }

            // Clear cart
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("Confirmation", new { invoiceId = invoice.Id });
        }

        public async Task<IActionResult> Confirmation(int invoiceId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var invoice = await _context.Invoices
                .Include(i => i.ItemInvoices)
                    .ThenInclude(ii => ii.Item)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.UserId == user.Id);

            if (invoice == null) return NotFound();

            return View("Confirmation", invoice);
        }

        public async Task<IActionResult> DownloadPdf(int invoiceId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var invoice = await _context.Invoices
                .Include(i => i.ItemInvoices)
                    .ThenInclude(ii => ii.Item)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.UserId == user.Id);

            if (invoice == null) return NotFound();

            var pdf = InvoiceDocument.Generate(invoice);
            return File(pdf, "application/pdf", $"invoice-{invoiceId}.pdf");
        }
    }
}