using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TestMVC.Models;

public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime OrderDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [Required]
    public int OrderStatusId { get; set; }

    [ForeignKey("OrderStatusId")]
    public OrderStatus OrderStatus { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }

    public List<Races> Races { get; set; } = new List<Races>();
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
