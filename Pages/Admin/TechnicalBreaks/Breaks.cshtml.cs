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
    public class BreaksModel : PageModel
    {
        private readonly BreakContext _breakContext;

        public BreaksModel(BreakContext breakContext)
        {
            _breakContext = breakContext;
        }

        public IEnumerable<TechnicalBreaks> Breaks { get; set; }

        public async Task OnGetAsync()
        {
            Breaks = await _breakContext.GetAllBreaksAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _breakContext.DeleteBreakAsync(id);
            return RedirectToPage();
        }
    }
}