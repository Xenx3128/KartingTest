using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TestMVC.Data;
using TestMVC.Utility;
using TestMVC.Models;

namespace TestMVC.Pages;
public class AccEditModel : PageModel
{
    private string connectionString = ConnectionString.CName;
    private readonly UserContext _userContext;

    public AccEditModel()
    {
        _userContext = new UserContext(connectionString);
    }

    [BindProperty]
    public string Email { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userContext.GetUserByEmail(Email);

        if (user != null && user.Pwd == Password)
        {
            // Successful login logic here
            return RedirectToPage("/Index");
        }

        ModelState.AddModelError("", "Invalid email or password.");
        return Page();
    }
}