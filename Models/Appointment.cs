using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace TestMVC.Models;

public class Races{
    [Key]
    public int Id { get; set; }
    [ForeignKey("Orders")]
    public int OrderId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime FinishDate { get; set; }
    public string? Status { get; set; }
}
public class AppointmentSlot
{
    [Required]
    public int? Id { get; set; }

    [Required]
    public DateOnly SlotDate { get; set; }
    public TimeOnly SlotStart { get; set; }
    public int? OrderId { set; get; }

    public string? Status { get; set; } = "free";
    public string? RaceType { get; set; }

}

public class FormData
{
    public string Date { get; set; }
    public string Time { get; set; }
    
    public Dictionary<string, object> Fields { get; set; } = new Dictionary<string, object>();
}

public class AppointmentSlotUpdate
{
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
    public int? PatientId { get; set; }
    public string? Status { get; set; }
}

public class AppointmentSlotRange
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int? PatientId { get; set; }
    public string Scale { get; set; }
}