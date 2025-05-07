using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestMVC.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace TestMVC.Pages.AdminSettings
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class EditModel : PageModel
    {
        private readonly SettingsContext _settingsContext;

        public EditModel(SettingsContext settingsContext)
        {
            _settingsContext = settingsContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Начало дня обязательно")]
            [DataType(DataType.Time)]
            public TimeSpan DayStart { get; set; }

            [Required(ErrorMessage = "Конец дня обязателен")]
            [DataType(DataType.Time)]
            public TimeSpan DayFinish { get; set; }

            [Required(ErrorMessage = "Длительность заезда обязательна")]
            [DataType(DataType.Time)]
            public TimeSpan RaceDuration { get; set; }

            public bool IsSelected { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var settings = await _settingsContext.GetSettingsByIdAsync(id);
            if (settings == null)
            {
                return NotFound($"Unable to load settings with ID '{id}'.");
            }

            Input = new InputModel
            {
                Id = settings.Id,
                DayStart = settings.DayStart,
                DayFinish = settings.DayFinish,
                RaceDuration = settings.RaceDuration,
                IsSelected = settings.IsSelected
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.DayFinish <= Input.DayStart)
            {
                ModelState.AddModelError("Input.DayFinish", "Конец дня должен быть позже начала дня.");
                return Page();
            }

            if (Input.RaceDuration <= TimeSpan.Zero)
            {
                ModelState.AddModelError("Input.RaceDuration", "Длительность заезда должна быть больше нуля.");
                return Page();
            }

            var settings = new Settings
            {
                Id = Input.Id,
                DayStart = Input.DayStart,
                DayFinish = Input.DayFinish,
                RaceDuration = Input.RaceDuration,
                IsSelected = Input.IsSelected
            };

            await _settingsContext.UpdateSettingsAsync(settings);
            return RedirectToPage("./Settings");
        }
    }
}