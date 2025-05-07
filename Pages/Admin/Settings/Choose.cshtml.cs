using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TestMVC.Data;
using TestMVC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Serilog;

namespace TestMVC.Pages.AdminSettings
{
    [Authorize(Roles = "SuperAdmin")]
    public class ChooseModel : PageModel
    {
        private readonly SettingsContext _settingsContext;

        public ChooseModel(SettingsContext settingsContext)
        {
            _settingsContext = settingsContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<SelectListItem> SettingsOptions { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Необходимо выбрать набор настроек")]
            public int SettingsId { get; set; }
        }

        public async Task OnGetAsync()
        {
            await LoadSettingsDropdown();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                await LoadSettingsDropdown();
                return Page();
            }

            if (Input.SettingsId <= 0)
            {
                ModelState.AddModelError("Input.SettingsId", "Недопустимый набор настроек.");
                await LoadSettingsDropdown();
                return Page();
            }

            var settings = await _settingsContext.GetSettingsByIdAsync(Input.SettingsId);
            if (settings == null)
            {
                ModelState.AddModelError("Input.SettingsId", "Набор настроек не найден.");
                await LoadSettingsDropdown();
                return Page();
            }

            await _settingsContext.SelectSettingsAsync(Input.SettingsId);
            return RedirectToPage("./Settings");
        }

        private async Task LoadSettingsDropdown()
        {
            try
            {
                var settingsList = await _settingsContext.GetAllSettingsAsync();
                SettingsOptions = settingsList.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"ID {s.Id}: {s.DayStart.ToString("hh\\:mm")}-{s.DayFinish.ToString("hh\\:mm")}, {s.RaceDuration.ToString("hh\\:mm")}{(s.IsSelected ? " (Выбрано)" : "")}"
                }).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}