using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebshopShop.Data;
using WebshopShop.Models;
using WebshopShop.ViewModels;

namespace WebshopShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var cart = await GetOrCreateCartAsync(user.Id);

            var cartItems = await _context.CartItems
                .Include(ci => ci.Item)
                    .ThenInclude(i => i.Pictures)
                .Include(ci => ci.Item)
                    .ThenInclude(i => i.Category)
                .Where(ci => ci.CartId == cart.Id)
                .ToListAsync();

            var vm = new CartViewModel
            {
                Items = cartItems.Select(ci => new CartItemViewModel
                {
                    ItemId = ci.ItemId,
                    Title = ci.Item.Title,
                    Price = ci.Item.Price,
                    Quantity = ci.Quantity,
                    CategoryName = ci.Item.Category?.Name ?? string.Empty,
                    PictureId = ci.Item.Pictures.FirstOrDefault()?.Id
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int itemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var item = await _context.Items.FindAsync(itemId);
            if (item == null) return NotFound();

            var cart = await GetOrCreateCartAsync(user.Id);

            var existing = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ItemId == itemId);

            if (existing == null)
            {
                _context.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    ItemId = itemId,
                    Quantity = 1
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int itemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var cart = await GetOrCreateCartAsync(user.Id);

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ItemId == itemId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        private async Task<Cart> GetOrCreateCartAsync(string userId)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }
    }
}