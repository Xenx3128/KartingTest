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
    public class CircleResultsModel : PageModel
    {
        private readonly CircleResultsContext _circleResultsContext;

        public CircleResultsModel(CircleResultsContext circleResultsContext)
        {
            _circleResultsContext = circleResultsContext;
        }

        public UserRace UserRace { get; set; }
        public IEnumerable<CircleResults> CircleResults { get; set; }

        public async Task<IActionResult> OnGetAsync(int userRaceId)
        {
            UserRace = await _circleResultsContext.GetUserRaceByIdAsync(userRaceId);
            if (UserRace == null)
            {
                return NotFound();
            }

            CircleResults = await _circleResultsContext.GetCircleResultsByUserRaceIdAsync(userRaceId);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id, int userRaceId)
        {
            await _circleResultsContext.DeleteCircleResultAsync(id);
            return RedirectToPage(new { userRaceId });
        }
    }
}