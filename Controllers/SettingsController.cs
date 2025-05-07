using Microsoft.AspNetCore.Mvc;
using TestMVC.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace TestMVC.Controllers
{
    [Route("api/settings")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly SettingsContext _settingsContext;

        public SettingsController(SettingsContext settingsContext)
        {
            _settingsContext = settingsContext;
        }

        [HttpGet("selected")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<Settings>> GetSelectedSettings()
        {
            var settings = await _settingsContext.GetSelectedSettingsAsync();
            if (settings == null)
            {
                return NotFound(new { message = "No selected settings found." });
            }

            return Ok(new
            {
                id = settings.Id,
                dayStart = settings.DayStart.TotalHours,
                dayFinish = settings.DayFinish.TotalHours,
                raceDuration = settings.RaceDuration.TotalMinutes,
                isSelected = settings.IsSelected
            });
        }
    }
}