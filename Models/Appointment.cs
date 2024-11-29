using System.ComponentModel.DataAnnotations;
//using Microsoft.EntityFrameworkCore;

namespace TestMVC.Models;

public class AppointmentSlot
{
    public int? Id { get; set; }
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }

    public string? PatientId { set; get; }

    public string? Status { get; set; } = "free";

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
    public string Scale { get; set; }
}