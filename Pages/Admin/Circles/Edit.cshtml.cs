using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestMVC.Data;
using TestMVC.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace TestMVC.Pages.AdminCircleResults
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class EditModel : PageModel
    {
        private readonly CircleResultsContext _circleResultsContext;

        public EditModel(CircleResultsContext circleResultsContext)
        {
            _circleResultsContext = circleResultsContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public int UserRaceId { get; set; }

        public class InputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Номер круга обязателен")]
            [Range(1, int.MaxValue, ErrorMessage = "Номер круга должен быть больше 0")]
            public int CircleNum { get; set; }

            [Required(ErrorMessage = "Время круга обязательно")]
            public string CircleTime { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id, int userRaceId)
        {
            var circleResult = await _circleResultsContext.GetCircleResultByIdAsync(id);
            if (circleResult == null || circleResult.UserRaceId != userRaceId)
            {
                return NotFound();
            }

            var userRace = await _circleResultsContext.GetUserRaceByIdAsync(userRaceId);
            if (userRace == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = circleResult.Id,
                CircleNum = circleResult.CircleNum,
                CircleTime = circleResult.CircleTime.ToString(@"mm\:ss")
            };

            UserRaceId = userRaceId;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, int userRaceId)
        {
            UserRaceId = userRaceId;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validate CircleTime format (mm:ss)
            if (!TimeSpan.TryParseExact(Input.CircleTime, @"mm\:ss", null, out var circleTimeSpan))
            {
                ModelState.AddModelError("Input.CircleTime", "Неверный формат времени. Используйте mm:ss (например, 01:30).");
                return Page();
            }

            // Verify UserRaceId exists
            var userRace = await _circleResultsContext.GetUserRaceByIdAsync(userRaceId);
            if (userRace == null)
            {
                ModelState.AddModelError("", "Указанный участник заезда не найден.");
                return Page();
            }

            var circleResult = new CircleResults
            {
                Id = Input.Id,
                UserRaceId = userRaceId,
                CircleNum = Input.CircleNum,
                CircleTime = circleTimeSpan
            };

            try
            {
                await _circleResultsContext.UpdateCircleResultAsync(circleResult);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при обновлении результата: {ex.Message}");
                return Page();
            }

            return RedirectToPage("./Circles", new { userRaceId });
        }
    }
}