using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestMVC.Data;
using TestMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Serilog;

namespace TestMVC.Pages
{
    public class OrderConfirmationModel : PageModel
    {
        private readonly AppointmentContext _appointmentContext;

        public OrderConfirmationModel(AppointmentContext appointmentContext)
        {
            _appointmentContext = appointmentContext;
        }

        public int OrderId { get; set; }
        public DateOnly Date { get; set; }
        public List<TimeOnly> Times { get; set; }
        public bool IsUniform { get; set; }
        public List<string> RaceCategories { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve order details from TempData
            var orderJson = TempData["OrderConfirmation"]?.ToString();
            if (string.IsNullOrEmpty(orderJson))
            {
                Log.Warning("No order confirmation data found in TempData");
                return RedirectToPage("/Orders");
            }

            try
            {
                var orderData = JsonSerializer.Deserialize<OrderConfirmationData>(orderJson);

                OrderId = orderData.OrderId;
                Date = DateOnly.Parse(orderData.Date);
                Times = orderData.Times.Select(t => TimeOnly.Parse(t)).ToList();
                IsUniform = orderData.IsUniform;

                // Fetch race category names
                var categories = await _appointmentContext.GetRaceCategoriesAsync();
                RaceCategories = orderData.RaceCategoryIds
                    .Select(id => categories.FirstOrDefault(c => c.Id == id)?.Category ?? "Неизвестная категория")
                    .ToList();

                if (RaceCategories.Any(c => c == "Неизвестная категория"))
                {
                    Log.Warning("Unknown race category IDs: {Ids}", string.Join(", ", orderData.RaceCategoryIds));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to process order confirmation data");
                return RedirectToPage("/Orders");
            }

            return Page();
        }

        private class OrderConfirmationData
        {
            public int OrderId { get; set; }
            public string Date { get; set; }
            public List<string> Times { get; set; }
            public bool IsUniform { get; set; }
            public List<int> RaceCategoryIds { get; set; }
        }
    }
}