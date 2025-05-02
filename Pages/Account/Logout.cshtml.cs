using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using TestMVC.Models;
using System.Threading.Tasks;

namespace TestMVC.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            returnUrl ??= Url.Page("/Index");
            return Redirect(returnUrl);
        }
    }
}