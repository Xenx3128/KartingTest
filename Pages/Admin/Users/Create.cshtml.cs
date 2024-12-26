using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TestMVC.Data;
using TestMVC.Utility;
using TestMVC.Models;

namespace TestMVC.Pages;
public class AdminUsersCreateModel : PageModel
{
    private string connectionString = ConnectionString.CName;
    private readonly UserContext _userContext;

    public AdminUsersCreateModel()
    {
        _userContext = new UserContext(connectionString);
    }

    [BindProperty]
    public CartUser? CUser { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _userContext.RegisterUser(CUser);

        return RedirectToPage("./Users");
    }

}
