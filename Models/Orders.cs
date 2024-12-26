using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace TestMVC.Models;

public class Orders
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("CartUser")]
    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }
    public decimal Price { get; set; }
    public string? Status { get; set; }
}