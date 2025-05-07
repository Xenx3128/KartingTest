using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TestMVC.Data;
using TestMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Serilog;

namespace TestMVC.Pages
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminUsersModel : PageModel
    {
        private readonly UserContext _userContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminUsersModel(
            UserContext userContext,
            UserManager<ApplicationUser> userManager)
        {
            _userContext = userContext;
            _userManager = userManager;
        }

        public IEnumerable<ApplicationUser> Users { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _userContext.GetAllUsersAsync();
        }

        public async Task<IList<string>> GetUserRolesAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null ? await _userManager.GetRolesAsync(user) : new List<string>();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var targetUser = await _userManager.FindByIdAsync(id.ToString());
            if (targetUser == null)
            {
                Log.Warning("Delete attempt for non-existent user ID {Id}", id);
                return RedirectToPage();
            }

            // Get current user's ID and roles
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var currentUserRoles = await _userManager.GetRolesAsync(await _userManager.GetUserAsync(User));
            var targetUserRoles = await _userManager.GetRolesAsync(targetUser);

            // Validate role hierarchy
            bool canDelete = false;
            if (currentUserRoles.Contains("SuperAdmin"))
            {
                // SuperAdmin can delete User or Admin, but not themselves or other SuperAdmins
                canDelete = id != currentUserId && !targetUserRoles.Contains("SuperAdmin");
            }
            else if (currentUserRoles.Contains("Admin"))
            {
                // Admin can delete only User, not themselves
                canDelete = id != currentUserId && targetUserRoles.Contains("User") && !targetUserRoles.Any(r => r == "Admin" || r == "SuperAdmin");
            }

            if (!canDelete)
            {
                Log.Warning("Unauthorized delete attempt by UserId {CurrentUserId} (Roles: {CurrentRoles}) on UserId {TargetUserId} (Roles: {TargetRoles})",
                    currentUserId, string.Join(", ", currentUserRoles), id, string.Join(", ", targetUserRoles));
                ModelState.AddModelError(string.Empty, "Недостаточно прав для удаления этого пользователя.");
                Users = await _userContext.GetAllUsersAsync();
                return Page();
            }

            var result = await _userManager.DeleteAsync(targetUser);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                Log.Error("Failed to delete UserId {Id}: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));
                Users = await _userContext.GetAllUsersAsync();
                return Page();
            }

            Log.Information("UserId {Id} deleted by UserId {CurrentUserId}", id, currentUserId);
            return RedirectToPage();
        }
    }
}