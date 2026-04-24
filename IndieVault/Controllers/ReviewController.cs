using IndieVault.Data;
using IndieVault.Models;
using IndieVault.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace IndieVault.Controllers
{
    public class ReviewController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles ="Player")]
        public async Task<IActionResult> Create(int id)
        {
            bool gameExists = await _context.Games.AnyAsync(game => game.Id == id);
            if(!gameExists)
            {
                return NotFound();
            }

            // _userManager.GetUserId(User) -> This returns the GUID string directly from the claims — no database call needed.
            bool alreadyReviewed = await _context.Reviews.AnyAsync(r => r.GameId == id && r.UserId == _userManager.GetUserId(User));
            if (alreadyReviewed)
            {
                TempData["ErrorMessage"] = "You have already reviewed this game.";
                return RedirectToAction("Details", "Game", new { id });
            }

            var viewModel = new ReviewViewModel
            {
                GameId = id
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Player")]
        public async Task<IActionResult> Create(ReviewViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            bool alreadyReviewed = await _context.Reviews.AnyAsync(r => r.GameId == viewModel.GameId && r.UserId == _userManager.GetUserId(User));

            if (alreadyReviewed)
            {
                TempData["ErrorMessage"] = "You have already reviewed this game.";
                return RedirectToAction("Details", "Game", new { id = viewModel.GameId });
            }

            await _context.Reviews.AddAsync(new Review
            {
                GameId = viewModel.GameId,
                UserId = _userManager.GetUserId(User),
                Rating = viewModel.Rating,
                Comment = viewModel.Comment,
                ReviewDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Game", new { id = viewModel.GameId });
        }

    }
}
