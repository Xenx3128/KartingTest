using System.ComponentModel.DataAnnotations;
//using Microsoft.EntityFrameworkCore;

namespace TestMVC.Models;

public class CartUser
{
    [Key]
    public int Id { get; set; }
    public string? UserName { get; set; }
    public DateTime BirthDate { get; set; }
    public string? PhoneNum { get; set; }
    public string? Email { get; set; }
    public string? Pwd { get; set; }
    public string? UserRole { get; set; }
    public string? FromWhereFoundOut { get; set; }
    public string? Status { get; set; }

    public string? Note { get; set; }
}