using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TestMVC.Data;
using TestMVC.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;

namespace TestMVC.Pages.AdminBreaks
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class EditModel : PageModel
    {
        private readonly BreakContext _breakContext;

        public EditModel(BreakContext breakContext)
        {
            _breakContext = breakContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<SelectListItem> BreakStatuses { get; set; }

        public class InputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Дата обязательна")]
            [DataType(DataType.Date)]
            public DateTime Date { get; set; }

            [Required(ErrorMessage = "Время обязательно")]
            public string Time { get; set; }

            [StringLength(50, ErrorMessage = "Описание не должно превышать 50 символов")]
            public string Desc { get; set; }

            [Required(ErrorMessage = "Статус обязателен")]
            public int BreakStatusId { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var breakItem = await _breakContext.GetBreakByIdAsync(id);
            if (breakItem == null)
            {
                return NotFound();
            }

            // Convert DateStart to Ekaterinburg time
            var ekaterinburgTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time");
            var startDateEkaterinburg = TimeZoneInfo.ConvertTimeFromUtc(breakItem.DateStart, ekaterinburgTimeZone);

            Input = new InputModel
            {
                Id = breakItem.Id,
                Date = startDateEkaterinburg.Date,
                Time = startDateEkaterinburg.ToString("HH:mm"),
                Desc = breakItem.Desc,
                BreakStatusId = breakItem.BreakStatusId
            };

            // Pass StartTime to ViewData for JavaScript
            ViewData["StartTime"] = startDateEkaterinburg.ToString("HH:mm");

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

            // Validate Time format (HH:mm)
            if (!TimeSpan.TryParseExact(Input.Time, "hh\\:mm", null, out _))
            {
                ModelState.AddModelError("Input.Time", "Неверный формат времени. Используйте HH:mm.");
                await LoadDropdowns();
                return Page();
            }

            // Validate BreakStatusId
            var statuses = await _breakContext.GetBreakStatusesAsync();
            if (!statuses.Any(s => s.Id == Input.BreakStatusId))
            {
                ModelState.AddModelError("Input.BreakStatusId", "Недопустимый статус.");
                await LoadDropdowns();
                return Page();
            }

            // Convert Ekaterinburg time to UTC
            var ekaterinburgTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time");
            var localDateTime = DateTime.Parse($"{Input.Date:yyyy-MM-dd} {Input.Time}");
            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, ekaterinburgTimeZone);

            var breakItem = new TechnicalBreaks
            {
                Id = Input.Id,
                DateStart = utcDateTime,
                DateFinish = utcDateTime.AddMinutes(15), // 15-minute duration
                Desc = Input.Desc,
                BreakStatusId = Input.BreakStatusId
            };

            try
            {
                await _breakContext.UpdateBreakAsync(breakItem);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при обновлении перерыва: {ex.Message}");
                await LoadDropdowns();
                return Page();
            }

            return RedirectToPage("./Breaks");
        }

        private async Task LoadDropdowns()
        {
            var statuses = await _breakContext.GetBreakStatusesAsync();
            BreakStatuses = statuses.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Status
            }).ToList();
        }
    }
}