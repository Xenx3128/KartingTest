using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestMVC.Models;
using TestMVC.Data;
using TestMVC.Utility;


namespace TestMVC.Controllers;

[Route("api/appointments")]
[ApiController]
public class AppointmentController : Controller
{
    private string connectionString = ConnectionString.CName;

    [HttpGet]
    public async Task<IEnumerable<AppointmentSlot>> GetAppointments([FromQuery] DateTime start, [FromQuery] DateTime end){
        var _context = new AppointmentSlotLayer(connectionString); 
        var data = await _context.GetSlots(start, end);
        return data;
    }

    [HttpGet("free")]
    public async Task<IEnumerable<AppointmentSlot>> GetAppointments([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] string patient)
    {
        var _context = new AppointmentSlotLayer(connectionString); 
        var data = await _context.GetFreeSlots(start, end, patient);
        return data;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentSlot>> GetAppointmentSlot(int id){
        var _context = new AppointmentSlotLayer(connectionString); 
        return View(await _context.GetAppointmentSlot(id));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAppointmentSlot(int id, AppointmentSlotUpdate update){
        var _context = new AppointmentSlotLayer(connectionString); 
        _context.PutAppointmentSlot(id, update);
        return NoContent();
    }
    
}