using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using TestMVC.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace TestMVC.Pages
{
    [Authorize]
    public class AccModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUser CurrentUser { get; set; }
        public int UserId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Use the provided ID or fall back to the authenticated user's ID
            UserId = id ?? int.Parse(_userManager.GetUserId(User));
            CurrentUser = await _userManager.FindByIdAsync(UserId.ToString());

            if (CurrentUser == null)
            {
                return NotFound($"Unable to load user with ID '{UserId}'.");
            }

            return Page();
        }
    }
}