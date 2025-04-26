using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using TestMVC.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;

namespace TestMVC.Pages
{
    [Authorize(Roles = "Admin")]
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

        public List<SelectListItem> Roles { get; set; }

        public async Task OnGetAsync()
        {
            await InitializeRoles();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await InitializeRoles();
                return Page();
            }

            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                FullName = Input.FullName,
                PhoneNumber = Input.PhoneNumber,
                BirthDate = Input.BirthDate,
                FromWhereFoundOut = Input.FromWhereFoundOut,
                Note = Input.Note,
                EmailConfirmed = Input.Status != "Pending",
                LockoutEnd = Input.Status == "Blocked" ? DateTimeOffset.MaxValue : null
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                // Assign selected role
                if (!string.IsNullOrEmpty(Input.Role))
                {
                    await _userManager.AddToRoleAsync(user, Input.Role);
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
            Roles = (await _roleManager.Roles.ToListAsync())
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList();
        }
    }

}