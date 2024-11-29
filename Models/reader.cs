using System.ComponentModel.DataAnnotations;
//using Microsoft.EntityFrameworkCore;

namespace TestMVC.Models;

public class Reader{
    [Key]
    public int Id { get; set;}
    public string? LastName { get; set;}
    public string? FirstName { get; set;}
    public string? Address { get; set;}
    public DateTime BirthDate { get; set;}
}