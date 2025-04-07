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
            // Iterate through the ModelState to find errors
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                if (state.Errors.Count > 0)
                {
                    foreach (var error in state.Errors)
                    {
                        // Log or handle the error as needed
                        var errorMessage = error.ErrorMessage;
                        var exception = error.Exception; // If there's an exception associated with the error

                        // Example: Log the error
                        Console.WriteLine($"Error in {key}: {errorMessage}");
                    }
                }
            }
            return Page();
        }

        await _userContext.RegisterUser(CUser);

        return RedirectToPage("./Users");
    }

}
