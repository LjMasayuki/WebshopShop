using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebshopShop.Data;
using WebshopShop.ViewModels.Admin;

namespace WebshopShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ─── Orders ───────────────────────────────────────────────────────────

        public async Task<IActionResult> Orders()
        {
            var invoices = await _db.Invoices
                .Include(i => i.User)
                .Include(i => i.ItemInvoices)
                .OrderByDescending(i => i.IssuedDate)
                .ToListAsync();

            var vm = invoices.Select(i => new OrderListViewModel
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                UserName = i.User?.UserName ?? "—",
                Email = i.User?.Email ?? "—",
                Total = i.Total,
                IssuedDate = i.IssuedDate,
                ItemCount = i.ItemInvoices.Count
            }).ToList();

            return View("Orders", vm);
        }

        public async Task<IActionResult> OrderDetail(int id)
        {
            var invoice = await _db.Invoices
                .Include(i => i.User)
                .Include(i => i.CustomerAddress)
                .Include(i => i.ItemInvoices)
                    .ThenInclude(ii => ii.Item)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null) return NotFound();

            var vm = new OrderDetailViewModel
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                UserName = invoice.User?.UserName ?? "—",
                Email = invoice.User?.Email ?? "—",
                Total = invoice.Total,
                IssuedDate = invoice.IssuedDate,
                CustomerAddress = new AddressViewModel
                {
                    FirstName = invoice.CustomerAddress.FirstName,
                    LastName = invoice.CustomerAddress.LastName,
                    AddressLine1 = invoice.CustomerAddress.AddressLine1,
                    AddressLine2 = invoice.CustomerAddress.AddressLine2,
                    City = invoice.CustomerAddress.City,
                    ZipCode = invoice.CustomerAddress.ZipCode,
                    Country = invoice.CustomerAddress.Country,
                    State = invoice.CustomerAddress.State
                },
                Items = invoice.ItemInvoices.Select(ii => new OrderItemViewModel
                {
                    Title = ii.Item?.Title ?? "—",
                    Price = ii.Price
                }).ToList()
            };

            return View("OrderDetail", vm);
        }

        // ─── Users ────────────────────────────────────────────────────────────

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();

            var vm = new List<UserListViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var orderCount = await _db.Invoices.CountAsync(i => i.UserId == user.Id);
                vm.Add(new UserListViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "—",
                    Email = user.Email ?? "—",
                    Roles = roles.ToList(),
                    OrderCount = orderCount
                });
            }

            return View("Users", vm);
        }

        public async Task<IActionResult> EditUserRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var allRoles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            var vm = new EditUserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName ?? "—",
                Email = user.Email ?? "—",
                Roles = allRoles.Select(r => new RoleToggleViewModel
                {
                    RoleName = r.Name ?? string.Empty,
                    IsAssigned = userRoles.Contains(r.Name ?? string.Empty)
                }).ToList()
            };

            return View("EditUserRoles", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserRoles(EditUserRolesViewModel vm)
        {
            var user = await _userManager.FindByIdAsync(vm.UserId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var selectedRoles = vm.Roles
                .Where(r => r.IsAssigned)
                .Select(r => r.RoleName)
                .ToList();

            if (selectedRoles.Any())
                await _userManager.AddToRolesAsync(user, selectedRoles);

            return RedirectToAction(nameof(Users));
        }

        // ─── Statistics ───────────────────────────────────────────────────────

        public async Task<IActionResult> Statistics()
        {
            var invoices = await _db.Invoices
                .Include(i => i.ItemInvoices)
                    .ThenInclude(ii => ii.Item)
                .ToListAsync();

            var topItems = await _db.ItemInvoices
                .Include(ii => ii.Item)
                .GroupBy(ii => new { ii.ItemId, ii.Item.Title })
                .Select(g => new TopItemViewModel
                {
                    Title = g.Key.Title,
                    SaleCount = g.Count(),
                    Revenue = g.Sum(ii => ii.Price)
                })
                .OrderByDescending(x => x.SaleCount)
                .Take(5)
                .ToListAsync();

            var revenueByDay = invoices
                .GroupBy(i => i.IssuedDate.Date)
                .OrderBy(g => g.Key)
                .TakeLast(30)
                .Select(g => new RevenueByDayViewModel
                {
                    Day = g.Key.ToString("MMM dd"),
                    Revenue = g.Sum(i => i.Total)
                })
                .ToList();

            var vm = new StatisticsViewModel
            {
                TotalOrders = invoices.Count,
                TotalRevenue = invoices.Sum(i => i.Total),
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalItems = await _db.Items.CountAsync(),
                TopItems = topItems,
                RevenueByDay = revenueByDay
            };

            return View("Statistics", vm);
        }
    }
}