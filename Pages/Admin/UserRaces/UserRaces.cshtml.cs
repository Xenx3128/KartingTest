using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestMVC.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace TestMVC.Pages.Admin
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class UserRacesModel : PageModel
    {
        private readonly RaceContext _raceContext;

        public UserRacesModel(RaceContext raceContext)
        {
            _raceContext = raceContext;
        }

        public Races Race { get; set; }
        public IEnumerable<UserRace> UserRaces { get; set; }

        public async Task<IActionResult> OnGetAsync(int raceId)
        {
            Race = await _raceContext.GetRaceByIdAsync(raceId);
            if (Race == null)
            {
                return NotFound();
            }

            UserRaces = await _raceContext.GetUserRacesByRaceIdAsync(raceId);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id, int raceId)
        {
            await _raceContext.DeleteUserRaceAsync(id);
            return RedirectToPage(new { raceId });
        }
    }
}