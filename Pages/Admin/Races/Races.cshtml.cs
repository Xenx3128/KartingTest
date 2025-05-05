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
    public class RacesModel : PageModel
    {
        private readonly RaceContext _raceContext;

        public RacesModel(RaceContext raceContext)
        {
            _raceContext = raceContext;
        }

        public IEnumerable<Races> Races { get; set; }

        public async Task OnGetAsync()
        {
            Races = await _raceContext.GetAllRacesAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _raceContext.DeleteRaceAsync(id);
            return RedirectToPage();
        }
    }
}