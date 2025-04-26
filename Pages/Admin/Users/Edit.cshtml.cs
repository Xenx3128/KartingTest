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
using System.ComponentModel.DataAnnotations;

namespace TestMVC.Pages
{
    [Authorize(Roles = "Admin")]
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

        public List<SelectListItem> Roles { get; set; }
        public List<SelectListItem> Statuses { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            Input = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                FromWhereFoundOut = user.FromWhereFoundOut,
                Status = user.Status ?? "Active",
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
            if (!ModelState.IsValid)
            {
                await InitializeRoles();
                return Page();
            }

            var user = await _userManager.FindByIdAsync(Input.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Update user properties
            user.FullName = Input.FullName;
            user.Email = Input.Email;
            user.UserName = Input.Email; // Sync UserName with Email
            user.PhoneNumber = Input.PhoneNumber;
            user.BirthDate = Input.BirthDate;
            user.FromWhereFoundOut = Input.FromWhereFoundOut;
            user.Status = Input.Status;
            user.Note = Input.Note;
            user.AcceptTerms = Input.AcceptTerms;
            user.ReceivePromotions = Input.ReceivePromotions;
            user.EmailConfirmed = Input.Status != "Pending";
            user.LockoutEnd = Input.Status == "Blocked" ? DateTimeOffset.MaxValue : null;

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
                await _userManager.AddToRoleAsync(user, Input.Role);
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

            return RedirectToPage("./Users");
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