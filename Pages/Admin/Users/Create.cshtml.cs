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
    public class AdminUsersCreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminUsersCreateModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public CreateUserViewModel Input { get; set; }

        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();

        public bool IsSuperAdmin { get; set; }

        public async Task OnGetAsync()
        {
            await InitializeRoles();
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

            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                FullName = Input.FullName,
                PhoneNumber = Input.PhoneNumber,
                BirthDate = Input.BirthDate.ToUniversalTime(),
                FromWhereFoundOut = Input.FromWhereFoundOut,
                Note = Input.Note,
                AcceptTerms = Input.AcceptSafetyRules,
                ReceivePromotions = Input.ReceivePromotions,
                RegistrationDate = DateTime.UtcNow,
                EmailConfirmed = true, // Adjust based on your requirements
                LockoutEnd = Input.Status == "Banned" ? DateTimeOffset.MaxValue : null,
                LockoutEnabled = true // Required for LockoutEnd to take effect
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                // Assign selected role
                if (!string.IsNullOrEmpty(Input.Role))
                {
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

                return RedirectToPage("./Users");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await InitializeRoles();
            return Page();
        }

        private async Task InitializeRoles()
        {
            // Check if current user is SuperAdmin
            var currentUser = await _userManager.GetUserAsync(User);
            IsSuperAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "SuperAdmin");

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