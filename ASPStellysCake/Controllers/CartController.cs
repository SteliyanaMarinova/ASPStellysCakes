using ASPStellysCake.Data;
using ASPStellysCake.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPStellysCake.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Customer> _userManager;

        public CartController(ApplicationDbContext context, UserManager<Customer> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                    .ThenInclude(p => p.Categories)
                .Where(c => c.CustomerId == userId)
                .ToListAsync();
            return View(cartItems);
        }

        // POST: Cart/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            var userId = _userManager.GetUserId(User);
            var existing = await _context.CartItems
                .FirstOrDefaultAsync(c => c.CustomerId == userId && c.ProductId == productId);

            if (existing != null)
            {
                existing.Quantity += quantity;
                _context.Update(existing);
            }
            else
            {
                var item = new CartItem
                {
                    CustomerId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedOn = DateTime.Now
                };
                _context.CartItems.Add(item);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.CustomerId == userId);

            if (item == null) return NotFound();

            if (quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
                _context.Update(item);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Cart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var userId = _userManager.GetUserId(User);
            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.CustomerId == userId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // POST: Cart/Checkout — converts cart to orders
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CustomerId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Количката е празна.";
                return RedirectToAction("Index");
            }

            foreach (var item in cartItems)
            {
                var order = new Order
                {
                    CustomerId = userId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    RegisterOn = DateTime.Now
                };
                _context.Orders.Add(order);
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Поръчката е направена успешно!";
            return RedirectToAction("Index", "Orders");
        }
    }
}
