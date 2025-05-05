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
    public class CreateCartModel : PageModel
    {
        private readonly CartContext _cartContext;

        public CreateCartModel(CartContext cartContext)
        {
            _cartContext = cartContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
            public string Name { get; set; }

            [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
            public string Desc { get; set; }

        }

        public void OnGet()
        {
            Input = new InputModel();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var cart = new Cart
            {
                Name = Input.Name,
                Desc = Input.Desc
            };

            try
            {
                await _cartContext.CreateCartAsync(cart);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при создании картинга: {ex.Message}");
                return Page();
            }

            return RedirectToPage("./Carts");
        }
    }
}