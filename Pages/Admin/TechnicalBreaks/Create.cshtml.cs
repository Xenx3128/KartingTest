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
using System.Linq;

namespace TestMVC.Pages.AdminBreaks
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CreateModel : PageModel
    {
        private readonly BreakContext _breakContext;

        public CreateModel(BreakContext breakContext)
        {
            _breakContext = breakContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<SelectListItem> BreakStatuses { get; set; }

        public class InputModel
        {
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

        public async Task OnGetAsync()
        {
            await LoadDropdowns();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
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

            // Split and validate times (format HH:mm)
            var times = Input.Time.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .ToList();
            if (!times.Any())
            {
                ModelState.AddModelError("Input.Time", "Необходимо выбрать хотя бы одно время.");
                await LoadDropdowns();
                return Page();
            }

            foreach (var time in times)
            {
                if (!TimeSpan.TryParseExact(time, "hh\\:mm", null, out _))
                {
                    ModelState.AddModelError("Input.Time", $"Неверный формат времени: {time}. Используйте HH:mm.");
                    await LoadDropdowns();
                    return Page();
                }
            }

            // Convert Ekaterinburg times to UTC and create breaks
            var ekaterinburgTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time");
            var createdBreaks = new List<TechnicalBreaks>();

            try
            {
                foreach (var time in times)
                {
                    var localDateTime = DateTime.Parse($"{Input.Date:yyyy-MM-dd} {time}");
                    var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, ekaterinburgTimeZone);

                    var breakItem = new TechnicalBreaks
                    {
                        DateStart = utcDateTime,
                        DateFinish = utcDateTime.AddMinutes(15), // 15-minute duration
                        Desc = Input.Desc,
                        BreakStatusId = Input.BreakStatusId
                    };

                    var breakId = await _breakContext.CreateBreakAsync(breakItem);
                    createdBreaks.Add(breakItem);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при создании перерывов: {ex.Message}");
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