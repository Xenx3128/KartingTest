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

namespace TestMVC.Pages.AdminRaces
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class EditModel : PageModel
    {
        private readonly RaceContext _raceContext;

        public EditModel(RaceContext raceContext)
        {
            _raceContext = raceContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<SelectListItem> Orders { get; set; }
        public List<SelectListItem> RaceCategories { get; set; }
        public List<SelectListItem> RaceStatuses { get; set; }

        public class InputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Заказ обязателен")]
            public int OrderId { get; set; }

            [Required(ErrorMessage = "Дата обязательна")]
            [DataType(DataType.Date)]
            public DateTime Date { get; set; }

            [Required(ErrorMessage = "Время обязательно")]
            public string Time { get; set; }

            [Required(ErrorMessage = "Категория заезда обязательна")]
            public int RaceCategoryId { get; set; }

            [Required(ErrorMessage = "Статус заезда обязателен")]
            public int RaceStatusId { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var race = await _raceContext.GetRaceByIdAsync(id);
            if (race == null)
            {
                return NotFound();
            }

            // Convert StartDate and FinishDate to Ekaterinburg time
            var ekaterinburgTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time");
            var startDateEkaterinburg = TimeZoneInfo.ConvertTimeFromUtc(race.StartDate, ekaterinburgTimeZone);
            var finishDateEkaterinburg = TimeZoneInfo.ConvertTimeFromUtc(race.FinishDate, ekaterinburgTimeZone);

            Input = new InputModel
            {
                Id = race.Id,
                OrderId = race.OrderId,
                Date = startDateEkaterinburg.Date, // Use Ekaterinburg date
                Time = startDateEkaterinburg.ToString("HH:mm"), // Use Ekaterinburg time
                RaceCategoryId = race.RaceCategoryId,
                RaceStatusId = race.RaceStatusId
            };

            // Pass StartTime, Date, and FinishTime to ViewData for display
            ViewData["StartTime"] = startDateEkaterinburg.ToString("HH:mm");
            ViewData["RaceDate"] = startDateEkaterinburg.ToString("dd-MM-yyyy");
            ViewData["StartTimeDisplay"] = startDateEkaterinburg.ToString("HH:mm");
            ViewData["FinishTimeDisplay"] = finishDateEkaterinburg.ToString("HH:mm");

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

            // Validate OrderId
            if (!await _raceContext.OrderExistsAsync(Input.OrderId))
            {
                ModelState.AddModelError("Input.OrderId", "Заказ не найден.");
                await LoadDropdowns();
                return Page();
            }

            // Validate Time format (HH:mm)
            if (!TimeSpan.TryParseExact(Input.Time, "hh\\:mm", null, out _))
            {
                ModelState.AddModelError("Input.Time", "Неверный формат времени. Используйте HH:mm.");
                await LoadDropdowns();
                return Page();
            }

            // Validate RaceCategoryId
            var categories = await _raceContext.GetRaceCategoriesAsync();
            if (!categories.Any(c => c.Id == Input.RaceCategoryId))
            {
                ModelState.AddModelError("Input.RaceCategoryId", "Недопустимая категория заезда.");
                await LoadDropdowns();
                return Page();
            }

            // Validate RaceStatusId
            var statuses = await _raceContext.GetRaceStatusesAsync();
            if (!statuses.Any(s => s.Id == Input.RaceStatusId))
            {
                ModelState.AddModelError("Input.RaceStatusId", "Недопустимый статус заезда.");
                await LoadDropdowns();
                return Page();
            }

            // Combine Date and Time, converting back to UTC for storage
            var ekaterinburgTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time");
            var localDateTime = DateTime.Parse($"{Input.Date:yyyy-MM-dd} {Input.Time}");
            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, ekaterinburgTimeZone);

            var race = new Races
            {
                Id = Input.Id,
                OrderId = Input.OrderId,
                StartDate = utcDateTime,
                FinishDate = utcDateTime.AddMinutes(15), // 15-minute duration
                RaceCategoryId = Input.RaceCategoryId,
                RaceStatusId = Input.RaceStatusId
            };

            try
            {
                await _raceContext.UpdateRaceAsync(race);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при обновлении заезда: {ex.Message}");
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

            var categories = await _raceContext.GetRaceCategoriesAsync();
            RaceCategories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Category
            }).ToList();

            var statuses = await _raceContext.GetRaceStatusesAsync();
            RaceStatuses = statuses.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Status
            }).ToList();
        }
    }
}
