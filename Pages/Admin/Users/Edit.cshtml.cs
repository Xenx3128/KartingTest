using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TestMVC.Data;
using TestMVC.Utility;
using TestMVC.Models;

namespace TestMVC.Pages;
public class AdminUsersEditModel : PageModel
{
    private string connectionString = ConnectionString.CName;
    private readonly UserContext _userContext;

    public AdminUsersEditModel()
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

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _userContext.UpdateUser(CUser);

        return RedirectToPage("./Users");
    }

}
