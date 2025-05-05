using Microsoft.AspNetCore.Mvc;
using TestMVC.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace TestMVC.Controllers
{
    [Route("api/races")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RacesController : ControllerBase
    {
        private readonly RaceContext _raceContext;

        public RacesController(RaceContext raceContext)
        {
            _raceContext = raceContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Races>>> GetRaces([FromQuery] string status = null)
        {
            IEnumerable<Races> races;
            if (string.IsNullOrEmpty(status))
            {
                races = await _raceContext.GetAllRacesAsync();
            }
            else
            {
                races = await _raceContext.GetRacesByStatusAsync(status);
            }

            return Ok(races);
        }
    }
}
