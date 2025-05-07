using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TestMVC.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System;
using System.Linq;

namespace TestMVC.Pages.AdminOrders
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CreateModel : PageModel
    {
        private readonly OrderContext _orderContext;
        private readonly AppointmentContext _appointmentContext;

        public CreateModel(OrderContext orderContext, AppointmentContext appointmentContext)
        {
            _orderContext = orderContext;
            _appointmentContext = appointmentContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<SelectListItem> Users { get; set; }
        public List<SelectListItem> OrderStatuses { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Пользователь обязателен")]
            public int UserId { get; set; }

            [Required(ErrorMessage = "Дата обязательна")]
            public string Date { get; set; } // yyyy-MM-dd

            [Required(ErrorMessage = "Выберите хотя бы одно время")]
            public List<string> Times { get; set; } // HH:mm

            [Required(ErrorMessage = "Выберите тип заезда")]
            public bool IsUniform { get; set; }

            [Required(ErrorMessage = "Выберите категории заездов")]
            public List<int> RaceCategoryIds { get; set; }

            [Required(ErrorMessage = "Необходимо принять технику безопасности")]
            public bool TermsAccepted { get; set; }
        }

        public async Task OnGetAsync()
        {
            
            await LoadDropdowns();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("//tick");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("tick");
                await LoadDropdowns();
                return Page();
            }

            if (!Input.TermsAccepted)
            {
                ModelState.AddModelError("Input.TermsAccepted", "Необходимо принять технику безопасности");
                await LoadDropdowns();
                return Page();
            }

            if (Input.Times == null || Input.Times.Count == 0)
            {
                ModelState.AddModelError("Input.Times", "Выберите хотя бы одно время");
                await LoadDropdowns();
                return Page();
            }

            if (Input.RaceCategoryIds == null || Input.RaceCategoryIds.Count != (Input.IsUniform ? 1 : Input.Times.Count))
            {
                ModelState.AddModelError("Input.RaceCategoryIds", "Недостаточно категорий заездов");
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

            // Parse Date to DateOnly
            if (!DateOnly.TryParse(Input.Date, out var dateOnly))
            {
                ModelState.AddModelError("Input.Date", "Недопустимый формат даты. Ожидается yyyy-MM-dd.");
                await LoadDropdowns();
                return Page();
            }

            // Parse Times to List<TimeOnly>
            var timeOnlyList = new List<TimeOnly>();
            foreach (var time in Input.Times)
            {
                if (!TimeOnly.TryParse(time, out var timeOnly))
                {
                    ModelState.AddModelError("Input.Times", $"Недопустимый формат времени: {time}. Ожидается HH:mm.");
                    await LoadDropdowns();
                    return Page();
                }
                timeOnlyList.Add(timeOnly);
            }

            Console.WriteLine("///tick");
            // Create order and races
            try
            {
                await _appointmentContext.CreateOrderAsync(
                    Input.UserId,
                    dateOnly,
                    timeOnlyList,
                    Input.IsUniform,
                    Input.RaceCategoryIds);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при создании заказа: {ex.Message}");
                await LoadDropdowns();
                return Page();
            }

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

            // OrderStatuses not needed but kept for compatibility
            var statuses = await _orderContext.GetAllOrderStatusesAsync();
            OrderStatuses = statuses.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Status
            }).ToList();
        }
    }
}