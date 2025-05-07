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

namespace TestMVC.Pages.AdminRaces
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CreateModel : PageModel
    {
        private readonly RaceContext _raceContext;

        public CreateModel(RaceContext raceContext)
        {
            _raceContext = raceContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<SelectListItem> Orders { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Заказ обязателен")]
            public int OrderId { get; set; }

            [Required(ErrorMessage = "Дата обязательна")]
            [DataType(DataType.Date)]
            public DateTime Date { get; set; }

            [Required(ErrorMessage = "Выберите хотя бы одно время")]
            public List<string> Times { get; set; }

            [Required(ErrorMessage = "Выберите тип заезда")]
            public bool IsUniform { get; set; } = true;

            [Required(ErrorMessage = "Выберите категорию заезда")]
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
            Console.WriteLine("////////////////////////////////////test");
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return Page();
            }

            // Validate OrderId
            if (!await _raceContext.OrderExistsAsync(Input.OrderId))
            {
                ModelState.AddModelError("Input.OrderId", "Заказ не найден.");
                await LoadDropdowns();
                return Page();
            }

            // Validate TermsAccepted
            if (!Input.TermsAccepted)
            {
                ModelState.AddModelError("Input.TermsAccepted", "Необходимо принять технику безопасности");
                await LoadDropdowns();
                return Page();
            }

            // Validate Times
            if (Input.Times == null || Input.Times.Count == 0)
            {
                ModelState.AddModelError("Input.Times", "Выберите хотя бы одно время");
                await LoadDropdowns();
                return Page();
            }

            // Validate RaceCategoryIds
            var categories = await _raceContext.GetRaceCategoriesAsync();
            if (Input.RaceCategoryIds == null || Input.RaceCategoryIds.Any(id => !categories.Any(c => c.Id == id)))
            {
                ModelState.AddModelError("Input.RaceCategoryIds", "Недопустимая категория заезда");
                await LoadDropdowns();
                return Page();
            }

            if (Input.RaceCategoryIds.Count != (Input.IsUniform ? 1 : Input.Times.Count))
            {
                ModelState.AddModelError("Input.RaceCategoryIds", "Количество категорий не соответствует количеству слотов");
                await LoadDropdowns();
                return Page();
            }

            // Get default RaceStatusId for "Planned"
            var statuses = await _raceContext.GetRaceStatusesAsync();
            var plannedStatus = statuses.FirstOrDefault(s => s.Status == "Planned");
            if (plannedStatus == null)
            {
                ModelState.AddModelError("", "Статус 'Planned' не найден в базе данных");
                await LoadDropdowns();
                return Page();
            }

            try
            {
                // Create races for each time slot
                for (int i = 0; i < Input.Times.Count; i++)
                {
                    var time = Input.Times[i];
                    var startDate = DateTime.Parse($"{Input.Date:yyyy-MM-dd} {time}");
                    var race = new Races
                    {
                        OrderId = Input.OrderId,
                        StartDate = startDate,
                        FinishDate = startDate.AddMinutes(15),
                        RaceCategoryId = Input.IsUniform ? Input.RaceCategoryIds[0] : Input.RaceCategoryIds[i],
                        RaceStatusId = plannedStatus.Id
                    };
                    await _raceContext.CreateRaceAsync(race);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при создании заездов: {ex.Message}");
                await LoadDropdowns();
                return Page();
            }

            return RedirectToPage("./Races");
        }

        private async Task LoadDropdowns()
        {
            var orders = await _raceContext.GetOrdersForDropdownAsync();
            Orders = orders.Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = $"{o.Id} - {o.Email ?? "Unknown"}"
            }).ToList();
        }
    }
}