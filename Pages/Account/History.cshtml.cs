using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using TestMVC.Models;
using TestMVC.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TestMVC.Data;

namespace TestMVC.Pages
{
    [Authorize]
    public class AccHistoryModel : PageModel
    {
        private readonly UserContext _userContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _dbContext;

        public List<RaceHistoryViewModel> RaceHistory { get; set; }

        public AccHistoryModel(
            UserContext userContext,
            UserManager<ApplicationUser> userManager,
            AppDbContext dbContext)
        {
            _userContext = userContext;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            RaceHistory = await _userContext.GetUserRaceHistoryAsync(user.Id);
        }
    }


}