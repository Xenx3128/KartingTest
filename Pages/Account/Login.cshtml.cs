using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestMVC.Data;
using TestMVC.Models;
using Microsoft.AspNetCore.Identity;

namespace TestMVC.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserContext _userContext;
        private readonly ILogger<LoginModel> _logger;

        [BindProperty]
        public LoginViewModel Input { get; set; }

        public LoginModel(
            UserContext userContext,
            ILogger<LoginModel> logger)
        {
            _userContext = userContext;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var result = await _userContext.LoginUserAsync(
                    Input.Email, 
                    Input.Password, 
                    Input.RememberMe);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToPage("/Index");
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { RememberMe = Input.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }

                ModelState.AddModelError(string.Empty, "Неверный email или пароль");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ModelState.AddModelError(string.Empty, "Произошла ошибка при входе");
            }

            return Page();
        }
    }
}