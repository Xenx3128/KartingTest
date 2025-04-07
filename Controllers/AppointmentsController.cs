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


    [HttpGet("day")]
    public async Task<IEnumerable<string>> GetDailyAppointments([FromQuery] DateTime querydate){
        var _context = new AppointmentContext(connectionString); 

        var data1 = await _context.GetPlannedRaces(querydate);
        var data2 = await _context.GetTechBreaks(querydate);
        var data = data1.Concat(data2);
        var data3 = data.Select(a => a.ToString("HH:mm")).ToList();
        return data3;
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
}