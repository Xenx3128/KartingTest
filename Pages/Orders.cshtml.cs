using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestMVC.Data;
using TestMVC.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;

namespace TestMVC.Pages
{
    [Authorize]
    public class OrdersModel : PageModel
    {
        private readonly AppointmentContext _appointmentContext;

        public OrdersModel(AppointmentContext appointmentContext)
        {
            _appointmentContext = appointmentContext;
        }

        [BindProperty]
        public OrderingModel Input { get; set; }

        

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
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

            // Get current user's ID
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            await _appointmentContext.CreateOrderAsync(
                userId,
                Input.Date,
                Input.Times,
                Input.IsUniform,
                Input.RaceCategoryIds);

            return RedirectToPage("/Index");
        }
    }
}