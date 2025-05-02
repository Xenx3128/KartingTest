using Microsoft.AspNetCore.Mvc;
using TestMVC.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestMVC.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppointmentContext _appointmentContext;
        private static readonly TimeZoneInfo EkaterinburgTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time");

        public AppointmentsController(AppointmentContext appointmentContext)
        {
            _appointmentContext = appointmentContext;
        }

        [HttpGet("day")]
        public async Task<ActionResult<IEnumerable<object>>> GetUnavailableTimes(DateTime querydate)
        {
            var races = await _appointmentContext.GetPlannedRaces(querydate);
            var breaks = await _appointmentContext.GetTechBreaks(querydate);
            var unavailable = new List<DateTime>();
            unavailable.AddRange(races);
            unavailable.AddRange(breaks);

            var formattedTimes = unavailable
                .Select(dt => new
                {
                    date = dt.ToString("yyyy-MM-dd"),
                    time = TimeZoneInfo.ConvertTime(dt, EkaterinburgTimeZone).ToString("HH:mm")
                })
                .Distinct()
                .OrderBy(x => x.date)
                .ThenBy(x => x.time)
                .ToList();

            return Ok(formattedTimes);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<RaceCategory>>> GetRaceCategories()
        {
            var categories = await _appointmentContext.GetRaceCategoriesAsync();
            return Ok(categories);
        }

        [HttpPost("order")]
        public async Task<ActionResult<int>> CreateOrder([FromBody] OrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.Times == null || !request.Times.Any())
            {
                return BadRequest(new { message = "At least one time slot is required." });
            }

            var times = new List<TimeOnly>();
            foreach (var time in request.Times)
            {
                if (!TimeOnly.TryParse(time, out var parsedTime))
                {
                    return BadRequest(new { message = $"Invalid time format: {time}. Expected format: HH:mm (e.g., 10:00)." });
                }
                times.Add(parsedTime);
            }

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var date = DateOnly.Parse(request.Date);

            var orderId = await _appointmentContext.CreateOrderAsync(
                userId,
                date,
                times,
                request.IsUniform,
                request.RaceCategoryIds);

            return Ok(orderId);
        }

        public class OrderRequest
        {
            public string Date { get; set; }
            public List<string> Times { get; set; }
            public bool IsUniform { get; set; }
            public List<int> RaceCategoryIds { get; set; }
            public bool TermsAccepted { get; set; }
        }
    }
}