using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestMVC.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace TestMVC.Pages.Admin
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class OrdersModel : PageModel
    {
        private readonly OrderContext _orderContext;

        public OrdersModel(OrderContext orderContext)
        {
            _orderContext = orderContext;
        }

        public IEnumerable<Order> Orders { get; set; }
        public Dictionary<int, List<Races>> OrderRaces { get; set; }

        public async Task OnGetAsync()
        {
            Orders = await _orderContext.GetAllOrdersAsync();
            OrderRaces = new Dictionary<int, List<Races>>();
            foreach (var order in Orders)
            {
                var races = await _orderContext.GetRacesByOrderIdAsync(order.Id);
                OrderRaces[order.Id] = races.ToList();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _orderContext.DeleteOrderAsync(id);
            return RedirectToPage();
        }
    }
}