using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TestMVC.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace TestMVC.Pages.AdminOrders
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class EditModel : PageModel
    {
        private readonly OrderContext _orderContext;

        public EditModel(OrderContext orderContext)
        {
            _orderContext = orderContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<SelectListItem> Users { get; set; }
        public List<SelectListItem> OrderStatuses { get; set; }

        public class InputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Пользователь обязателен")]
            public int UserId { get; set; }

            [Required(ErrorMessage = "Дата заказа обязательна")]
            [DataType(DataType.DateTime)]
            public DateTime OrderDate { get; set; }

            [Required(ErrorMessage = "Цена обязательна")]
            [Range(0, 999999.99, ErrorMessage = "Цена должна быть от 0 до 999,999.99")]
            public decimal Price { get; set; }

            [Required(ErrorMessage = "Статус обязателен")]
            public int OrderStatusId { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var order = await _orderContext.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                Price = order.Price,
                OrderStatusId = order.OrderStatusId
            };

            await LoadDropdowns();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return Page();
            }

            // Validate UserId
            if (!await _orderContext.UserExistsAsync(Input.UserId))
            {
                ModelState.AddModelError("Input.UserId", "Пользователь не найден.");
                await LoadDropdowns();
                return Page();
            }

            // Validate OrderStatusId
            var statuses = await _orderContext.GetAllOrderStatusesAsync();
            if (!statuses.Any(s => s.Id == Input.OrderStatusId))
            {
                ModelState.AddModelError("Input.OrderStatusId", "Недопустимый статус заказа.");
                await LoadDropdowns();
                return Page();
            }

            var order = new Order
            {
                Id = Input.Id,
                UserId = Input.UserId,
                OrderDate = Input.OrderDate,
                Price = Input.Price,
                OrderStatusId = Input.OrderStatusId
            };

            await _orderContext.UpdateOrderAsync(order);
            return RedirectToPage("./Orders");
        }

        private async Task LoadDropdowns()
        {
            var users = await _orderContext.GetUsersForDropdownAsync();
            Users = users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Email
            }).ToList();

            var statuses = await _orderContext.GetAllOrderStatusesAsync();
            OrderStatuses = statuses.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Status
            }).ToList();
        }
    }
}