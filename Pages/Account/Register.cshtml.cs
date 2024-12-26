using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TestMVC.Data;
using TestMVC.Utility;
using TestMVC.Models;

namespace TestMVC.Pages;
public class RegisterModel : PageModel
{
    private string connectionString = ConnectionString.CName;
    private readonly UserContext _userContext;

    public RegisterModel()
    {
        _userContext = new UserContext(connectionString);
    }

    [BindProperty]
    public CartUser MyUser { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        //var  _userContext = new UserContext(connectionString);
        await _userContext.RegisterUser(MyUser);
        return RedirectToPage("/Account/Login");
    }
}