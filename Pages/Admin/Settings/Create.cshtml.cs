using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestMVC.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace TestMVC.Pages.AdminSettings
{
    [Authorize(Roles = "SuperAdmin")]
    public class CreateModel : PageModel
    {
        private readonly SettingsContext _settingsContext;

        public CreateModel(SettingsContext settingsContext)
        {
            _settingsContext = settingsContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
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

        public void OnGet()
        {
            Input = new InputModel();
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
                DayStart = Input.DayStart,
                DayFinish = Input.DayFinish,
                RaceDuration = Input.RaceDuration,
                IsSelected = false
            };

            await _settingsContext.CreateSettingsAsync(settings);
            return RedirectToPage("./Settings");
        }
    }
}