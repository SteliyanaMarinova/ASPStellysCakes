using ASPStellysCake.Data;
using ASPStellysCake.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPStellysCake.Controllers
{
    [Authorize(Roles = "Customer")]

    public class FavoritesController : Controller
    {


        private readonly ApplicationDbContext _context;
        private readonly UserManager<Customer> _userManager;

        public FavoritesController(ApplicationDbContext context, UserManager<Customer> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Favorites
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var favorites = await _context.Favorites
                .Include(f => f.Product)
                    .ThenInclude(p => p.Categories)
                .Where(f => f.CustomerId == userId)
                .ToListAsync();
            return View(favorites);
        }

        // POST: Favorites/Toggle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int productId, string returnUrl = "/")
        {
            var userId = _userManager.GetUserId(User);
            var existing = await _context.Favorites
                .FirstOrDefaultAsync(f => f.CustomerId == userId && f.ProductId == productId);

            if (existing != null)
            {
                _context.Favorites.Remove(existing);
            }
            else
            {
                _context.Favorites.Add(new Favorite
                {
                    CustomerId = userId,
                    ProductId = productId,
                    AddedOn = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index");
        }

        // POST: Favorites/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int favoriteId)
        {
            var userId = _userManager.GetUserId(User);
            var fav = await _context.Favorites
                .FirstOrDefaultAsync(f => f.Id == favoriteId && f.CustomerId == userId);

            if (fav != null)
            {
                _context.Favorites.Remove(fav);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
