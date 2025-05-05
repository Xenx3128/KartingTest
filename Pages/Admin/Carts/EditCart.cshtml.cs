using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestMVC.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace TestMVC.Pages.Admin
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class EditCartModel : PageModel
    {
        private readonly CartContext _cartContext;

        public EditCartModel(CartContext cartContext)
        {
            _cartContext = cartContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public int Id { get; set; }

            [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
            public string Name { get; set; }

            [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
            public string Desc { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cart = await _cartContext.GetCartByIdAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = cart.Id,
                Name = cart.Name,
                Desc = cart.Desc
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var cart = new Cart
            {
                Id = Input.Id,
                Name = Input.Name,
                Desc = Input.Desc
            };

            try
            {
                await _cartContext.UpdateCartAsync(cart);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при обновлении картинга: {ex.Message}");
                return Page();
            }

            return RedirectToPage("./Carts");
        }
    }
}