/*
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TestMVC.Data;
using TestMVC.Utility;
using TestMVC.Models;

namespace TestMVC.Pages;
public class AdminUsersDeleteModel : PageModel
{
    private string connectionString = ConnectionString.CName;
    private readonly UserContext _userContext;

    public AdminUsersDeleteModel()
    {
        _userContext = new UserContext(connectionString);
    }

    [BindProperty]
    public CartUser? CUser { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        CUser = await _userContext.GetUserById(id);
        if (CUser == null){
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        _userContext.DeleteUser(id);
        
        return RedirectToPage("./Users");
    }

}
*/