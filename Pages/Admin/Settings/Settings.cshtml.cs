using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestMVC.Data;
using TestMVC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace TestMVC.Pages.AdminSettings
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class SettingsModel : PageModel
    {
        private readonly SettingsContext _settingsContext;

        public SettingsModel(SettingsContext settingsContext)
        {
            _settingsContext = settingsContext;
        }

        public IEnumerable<Settings> SettingsList { get; set; }

        public async Task OnGetAsync()
        {
            SettingsList = await _settingsContext.GetAllSettingsAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _settingsContext.DeleteSettingsAsync(id);
            return RedirectToPage();
        }
    }
}