using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using TestMVC.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TestMVC.Pages
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminUsersEditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminUsersEditModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public EditUserViewModel Input { get; set; }

        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();

        public bool IsSuperAdmin { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Determine current status based on LockoutEnd
            var status = user.LockoutEnd.HasValue ? "Banned" : "Active";

            Input = new EditUserViewModel
            {
                Id = id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                FromWhereFoundOut = user.FromWhereFoundOut,
                Status = status,
                Note = user.Note,
                AcceptTerms = user.AcceptTerms,
                ReceivePromotions = user.ReceivePromotions
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            Input.Role = userRoles.FirstOrDefault();

            await InitializeRoles();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if current user is SuperAdmin
            var currentUser = await _userManager.GetUserAsync(User);
            IsSuperAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "SuperAdmin");

            if (!ModelState.IsValid)
            {
                await InitializeRoles();
                return Page();
            }

            Console.WriteLine(Input.Id);
            var user = await _userManager.FindByIdAsync(Input.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Validate role selection: Regular Admins cannot select "Admin"
            if (!IsSuperAdmin && Input.Role == "Admin")
            {
                ModelState.AddModelError("Input.Role", "Только SuperAdmin может назначать роль Admin.");
                await InitializeRoles();
                return Page();
            }

            // Validate status
            if (Input.Status != "Active" && Input.Status != "Banned")
            {
                ModelState.AddModelError("Input.Status", "Недопустимый статус.");
                await InitializeRoles();
                return Page();
            }

            // Update user properties
            user.FullName = Input.FullName;
            user.Email = Input.Email;
            user.UserName = Input.Email; // Sync UserName with Email
            user.PhoneNumber = Input.PhoneNumber;
            user.BirthDate = Input.BirthDate.ToUniversalTime();
            user.FromWhereFoundOut = Input.FromWhereFoundOut;
            user.Note = Input.Note;
            user.AcceptTerms = Input.AcceptTerms;
            user.ReceivePromotions = Input.ReceivePromotions;
            user.LockoutEnd = Input.Status == "Banned" ? DateTimeOffset.MaxValue : null;
            user.LockoutEnabled = true; // Ensure lockout is enabled

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                await InitializeRoles();
                return Page();
            }

            // Update role
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!string.IsNullOrEmpty(Input.Role) && !currentRoles.Contains(Input.Role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                var roleResult = await _userManager.AddToRoleAsync(user, Input.Role);
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    await InitializeRoles();
                    return Page();
                }
            }

            // Update password if provided
            if (!string.IsNullOrEmpty(Input.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, Input.Password);
                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    await InitializeRoles();
                    return Page();
                }
            }

            return RedirectToPage("/Admin/Users/Users");
        }

        private async Task InitializeRoles()
        {
            // Filter roles based on user role
            var availableRoles = IsSuperAdmin
                ? new[] { "User", "Admin" }
                : new[] { "User" };

            Roles = await _roleManager.Roles
                .Where(r => availableRoles.Contains(r.Name))
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                })
                .ToListAsync() ?? new List<SelectListItem>();
        }
    }
}