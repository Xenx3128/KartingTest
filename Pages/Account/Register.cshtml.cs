using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestMVC.Data;
using TestMVC.Models;

namespace TestMVC.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserContext _userContext;
        private readonly ILogger<RegisterModel> _logger;

        [BindProperty]
        public RegisterViewModel Input { get; set; }

        public RegisterModel(
            UserContext userContext,
            ILogger<RegisterModel> logger)
        {
            _userContext = userContext;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            Input = new RegisterViewModel 
            {
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,  // Important for Identity
                    Email = Input.Email,
                    FullName = Input.FullName,
                    BirthDate = Input.BirthDate,
                    PhoneNumber = Input.PhoneNum,  // Using Identity's PhoneNumber
                    FromWhereFoundOut = Input.FromWhereFoundOut,
                    AcceptTerms = Input.AcceptTerms,
                    ReceivePromotions = Input.ReceivePromotions,
                    EmailConfirmed = true  // Set to false if using email confirmation
                };

                var result = await _userContext.RegisterUserAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    
                    // Optionally sign in the user
                    await _userContext.LoginUserAsync(Input.Email, Input.Password, false);
                    
                    return RedirectToPage("/Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                ModelState.AddModelError(string.Empty, "Произошла ошибка при регистрации");
            }

            return Page();
        }
    }
}