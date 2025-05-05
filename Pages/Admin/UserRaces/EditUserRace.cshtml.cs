using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TestMVC.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;

namespace TestMVC.Pages.Admin
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class EditUserRaceModel : PageModel
    {
        private readonly RaceContext _raceContext;

        public EditUserRaceModel(RaceContext raceContext)
        {
            _raceContext = raceContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public Races Race { get; set; }
        public List<SelectListItem> Users { get; set; }
        public List<SelectListItem> Carts { get; set; }

        public class InputModel
        {
            public int Id { get; set; }
            public int RaceId { get; set; }

            [Required(ErrorMessage = "Пользователь обязателен")]
            public int UserId { get; set; }

            [Required(ErrorMessage = "Картинг обязателен")]
            public int CartId { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "Позиция должна быть положительным числом")]
            public int? Position { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id, int raceId)
        {
            var userRace = await _raceContext.GetUserRaceByIdAsync(id);
            if (userRace == null)
            {
                return NotFound();
            }

            Race = await _raceContext.GetRaceByIdAsync(raceId);
            if (Race == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = userRace.Id,
                RaceId = userRace.RaceId,
                UserId = userRace.UserId,
                CartId = userRace.CartId ?? 0,
                Position = userRace.Position
            };

            await LoadDropdowns();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int raceId)
        {
            if (!ModelState.IsValid)
            {
                Race = await _raceContext.GetRaceByIdAsync(raceId);
                await LoadDropdowns();
                return Page();
            }

            // Validate UserId
            var users = await _raceContext.GetAllUsersAsync();
            if (!users.Any(u => u.Id == Input.UserId))
            {
                ModelState.AddModelError("Input.UserId", "Пользователь не найден.");
                Race = await _raceContext.GetRaceByIdAsync(raceId);
                await LoadDropdowns();
                return Page();
            }

            // Validate CartId
            var carts = await _raceContext.GetAllCartsAsync();
            if (!carts.Any(c => c.Id == Input.CartId))
            {
                ModelState.AddModelError("Input.CartId", "Картинг не найден.");
                Race = await _raceContext.GetRaceByIdAsync(raceId);
                await LoadDropdowns();
                return Page();
            }

            var userRace = new UserRace
            {
                Id = Input.Id,
                RaceId = Input.RaceId,
                UserId = Input.UserId,
                CartId = Input.CartId,
                Position = Input.Position
            };

            try
            {
                await _raceContext.UpdateUserRaceAsync(userRace);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при обновлении участника: {ex.Message}");
                Race = await _raceContext.GetRaceByIdAsync(raceId);
                await LoadDropdowns();
                return Page();
            }

            return RedirectToPage("./UserRaces", new { raceId });
        }

        private async Task LoadDropdowns()
        {
            var users = await _raceContext.GetAllUsersAsync();
            Users = users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = $"{u.FullName} ({u.Email})"
            }).ToList();

            var carts = await _raceContext.GetAllCartsAsync();
            Carts = carts.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
        }
    }
}