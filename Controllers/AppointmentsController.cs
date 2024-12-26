using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestMVC.Models;
using TestMVC.Data;
using TestMVC.Utility;
using TestMVC.Service;


namespace TestMVC.Controllers;

[Route("api/appointments")]
[ApiController]
public class AppointmentController : Controller
{
    private string connectionString = ConnectionString.CName;

    [HttpGet]
    public async Task<IEnumerable<AppointmentSlot>> GetAppointments([FromQuery] DateTime start, [FromQuery] DateTime end){
        var _context = new AppointmentContext(connectionString); 
        var data = await _context.GetSlots(start, end);
        return data;
    }

    [HttpGet("day")]
    public async Task<IEnumerable<DateTime>> GetDailyAppointments([FromQuery] DateTime querydate){
        var _context = new AppointmentContext(connectionString); 

        var data1 = await _context.GetPlannedRaces(querydate);
        var data2 = await _context.GetPlannedRaces(querydate);
        var data = data1.Concat(data2);
        return data;
    }

    [HttpPost("order")]
    public async Task<ActionResult<AppointmentSlot>> PostOrder([FromForm] object data){
        DateOnly date = new DateOnly();
        List<TimeOnly> times = new List<TimeOnly>();
        string raceTypeMode = "unknown";
        List<string> modes = new List<string>();

        foreach (var pair in Request.Form){
            var key = pair.Key.ToString();
            var val = pair.Value.ToString();
            //Console.WriteLine($"{key}: {val}");
            switch (key){
                case "date":
                    date = DateOnly.FromDateTime(DateTime.Parse(val));
                    break;
                case "time":
                    string[] timesStr = val.Split("; ");
                    foreach (var t in timesStr){
                        var time = TimeOnly.Parse(t);
                        times.Add(time);
                    }
                    break;
                case "raceTypeRadioOptions":
                    raceTypeMode = val;
                    break;
                case "__RequestVerificationToken":
                    break;
                default: // Race Type Options
                    var valSplit = val.Split('_');
                    var selected = val.Split('_')[3];
                    switch (selected){
                        case "0":
                            modes.Add("adult");
                            break;
                        case "1":
                            modes.Add("children");
                            break;
                        case "2":
                            modes.Add("family");
                            break;
                    }
                    break;

            }
        }
        var _context = new AppointmentContext(connectionString); 
        _context.PostOrder(date, times, raceTypeMode, modes);  
        return NoContent();
    }
/////////////////////////////////////////
    [HttpGet("all")]
    public async Task<IEnumerable<AppointmentSlot>> GetAllAppointments([FromQuery] DateTime start, [FromQuery] DateTime end){
        var _context = new AppointmentContext(connectionString); 
        var occupied = await _context.GetSlots(start, end);
        var timeline = Timeline.GenerateSlots(start, end, "hours");
        for (int i = 0; i < timeline.Count; i++){
            for (int j = 0; j < occupied.Count; j++){
                
                if (occupied[j].SlotStart == occupied[i].SlotStart){
                    timeline[i] = occupied[j];
                    
                }
            } 
        }
        return timeline;
    }

    [HttpGet("free")]
    public async Task<IEnumerable<AppointmentSlot>> GetAppointments([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] string patient)
    {
        var _context = new AppointmentContext(connectionString); 
        var data = await _context.GetFreeSlots(start, end, patient);
        return data;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentSlot>> GetAppointmentSlot(int id){
        var _context = new AppointmentContext(connectionString); 
        return View(await _context.GetAppointmentSlot(id));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAppointmentSlot(int id, AppointmentSlotUpdate update){
        var _context = new AppointmentContext(connectionString); 
        _context.PutAppointmentSlot(id, update);
        return NoContent();
    }

    [HttpPost("create")]
    public async Task<ActionResult<AppointmentSlot>> PostAppointmentSlots(AppointmentSlotRange range)
    {
        var _context = new AppointmentContext(connectionString);
        _context.PostAppointmentSlots(range);
        return NoContent();
    }
    
}