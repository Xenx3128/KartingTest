/*
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TestMVC.Data;
using TestMVC.Utility;
using TestMVC.Models;

namespace TestMVC.Pages;
public class AdminUsersFindModel : PageModel
{
    private string connectionString = ConnectionString.CName;
    private readonly UserContext _userContext;

    public AdminUsersFindModel()
    {
        _userContext = new UserContext(connectionString);
    }

    [BindProperty]
    public string SearchEmail { get; set; }

    public CartUser? FoundUser { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!string.IsNullOrEmpty(SearchEmail))
        {
            Console.WriteLine(SearchEmail);
            FoundUser = await _userContext.GetUserByEmail(SearchEmail);
            if (FoundUser == null){
                return NotFound();
            }
            return RedirectToPage("./Details", new { Id = FoundUser.Id});

        }
        return Page();
    }

}
*/