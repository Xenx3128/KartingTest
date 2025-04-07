using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace TestMVC.Models;

public class Order{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [ForeignKey("CartUser")]
    public int UserId { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime OrderDate { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime RaceDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; }
}

public class OrderStatus
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; }
}