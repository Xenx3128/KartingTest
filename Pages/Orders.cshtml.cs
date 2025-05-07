using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestMVC.Data;
using TestMVC.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Serilog;
using System.Text.Json;

namespace TestMVC.Pages
{
    public class OrdersModel : PageModel
    {
        private readonly AppointmentContext _appointmentContext;

        public OrdersModel(AppointmentContext appointmentContext)
        {
            _appointmentContext = appointmentContext;
        }

        [BindProperty]
        public OrderingModel Input { get; set; }

        public class OrderingModel
        {
            [Required(ErrorMessage = "Дата обязательна")]
            public DateOnly Date { get; set; }

            [Required(ErrorMessage = "Выберите хотя бы одно время")]
            public List<TimeOnly> Times { get; set; }

            [Required(ErrorMessage = "Выберите тип заезда")]
            public bool IsUniform { get; set; }

            [Required(ErrorMessage = "Выберите категории заездов")]
            public List<int> RaceCategoryIds { get; set; }

            [Required(ErrorMessage = "Необходимо принять технику безопасности")]
            public bool TermsAccepted { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Log.Error("ModelState invalid: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return Page();
            }

            if (!Input.TermsAccepted)
            {
                ModelState.AddModelError("Input.TermsAccepted", "Необходимо принять технику безопасности");
                return Page();
            }

            if (Input.Times == null || Input.Times.Count == 0)
            {
                ModelState.AddModelError("Input.Times", "Выберите хотя бы одно время");
                return Page();
            }

            if (Input.RaceCategoryIds == null || Input.RaceCategoryIds.Count != (Input.IsUniform ? 1 : Input.Times.Count))
            {
                ModelState.AddModelError("Input.RaceCategoryIds", "Недостаточно категорий заездов");
                return Page();
            }

            // Set UserId: Use authenticated user's ID or default to 10 for guest account
            int userId = User.Identity.IsAuthenticated
                ? int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
                : 10; // Guest account UserId

            try
            {
                int orderId = await _appointmentContext.CreateOrderAsync(
                    userId,
                    Input.Date,
                    Input.Times,
                    Input.IsUniform,
                    Input.RaceCategoryIds);

                // Store order details in TempData for confirmation page
                TempData["OrderConfirmation"] = JsonSerializer.Serialize(new
                {
                    OrderId = orderId,
                    Date = Input.Date.ToString("yyyy-MM-dd"),
                    Times = Input.Times.Select(t => t.ToString("HH:mm")).ToList(),
                    IsUniform = Input.IsUniform,
                    RaceCategoryIds = Input.RaceCategoryIds
                });

                return RedirectToPage("/OrderConfirmation");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create order for user {UserId} on date {Date}", userId, Input.Date);
                ModelState.AddModelError("", $"Ошибка при создании заказа: {ex.Message}");
                return Page();
            }
        }
    }
}