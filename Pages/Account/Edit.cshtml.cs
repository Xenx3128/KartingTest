using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using TestMVC.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace TestMVC.Pages
{
    [Authorize]
    public class AccEditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        [BindProperty]
        public EditProfileViewModel Input { get; set; }

        public AccEditModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Use the provided ID or fall back to the authenticated user's ID
            var userId = id ?? int.Parse(_userManager.GetUserId(User));
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            Input = new EditProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            // Use the provided ID or fall back to the authenticated user's ID
            var userId = id ?? int.Parse(_userManager.GetUserId(User));
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            // Restrict editing to the authenticated user's own account
            var authUserId = _userManager.GetUserId(User);
            if (userId.ToString() != authUserId)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Update profile information
            user.FullName = Input.FullName;
            user.PhoneNumber = Input.PhoneNumber;
            user.BirthDate = Input.BirthDate.ToUniversalTime();

            // Update email if changed
            if (Input.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var error in setEmailResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }
                user.UserName = Input.Email;
            }

            // Update password if provided
            if (!string.IsNullOrEmpty(Input.NewPassword))
            {
                if (string.IsNullOrEmpty(Input.OldPassword))
                {
                    ModelState.AddModelError("Input.OldPassword", "Текущий пароль обязателен для смены пароля.");
                    return Page();
                }

                if (Input.NewPassword != Input.ConfirmPassword)
                {
                    ModelState.AddModelError("Input.ConfirmPassword", "Пароли не совпадают.");
                    return Page();
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(
                    user,
                    Input.OldPassword,
                    Input.NewPassword);

                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                await _signInManager.RefreshSignInAsync(user);
            }

            // Save profile changes
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            return RedirectToPage("./Acc", new { id = userId });
        }
    }
}