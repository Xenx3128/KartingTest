using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TestMVC.Pages;
public class OrdersModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public OrdersModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }

}
