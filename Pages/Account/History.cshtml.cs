using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using TestMVC.Models;
using TestMVC.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace TestMVC.Pages
{
    [Authorize]
    public class AccHistoryModel : PageModel
    {
        private readonly UserContext _userContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccHistoryModel(
            UserContext userContext,
            UserManager<ApplicationUser> userManager)
        {
            _userContext = userContext;
            _userManager = userManager;
        }

        public List<RaceHistoryViewModel> RaceHistory { get; set; }
        public int UserId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Use the provided ID or fall back to the authenticated user's ID
            UserId = id ?? int.Parse(_userManager.GetUserId(User));
            var user = await _userManager.FindByIdAsync(UserId.ToString());

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{UserId}'.");
            }

            // Optional: Restrict history viewing to the authenticated user's own account
            /*
            var authUserId = _userManager.GetUserId(User);
            if (UserId.ToString() != authUserId)
            {
                return Forbid();
            }
            */

            RaceHistory = await _userContext.GetUserRaceHistoryAsync(user.Id);
            return Page();
        }
    }
}