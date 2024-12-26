using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TestMVC.Data;
using TestMVC.Utility;
using TestMVC.Models;

namespace TestMVC.Pages;
public class AdminUsersModel : PageModel
{
    private string connectionString = ConnectionString.CName;
    private readonly UserContext _userContext;

    public AdminUsersModel()
    {
        _userContext = new UserContext(connectionString);
    }

    [BindProperty]
    public IEnumerable<CartUser> Users { get; set; }

    public async Task OnGetAsync()
    {
        Users = await _userContext.GetAllUsers();
    }

}
