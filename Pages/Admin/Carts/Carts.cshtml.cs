using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestMVC.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace TestMVC.Pages.Admin
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CartsModel : PageModel
    {
        private readonly CartContext _cartContext;

        public CartsModel(CartContext cartContext)
        {
            _cartContext = cartContext;
        }

        public IEnumerable<Cart> Carts { get; set; }

        public async Task OnGetAsync()
        {
            Carts = await _cartContext.GetAllCartsAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _cartContext.DeleteCartAsync(id);
            return RedirectToPage();
        }
    }
}