using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestMVC.Models;

public class Races
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime FinishDate { get; set; }

    [Required]
    [StringLength(50)]
    public string Category { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; }
}

public class AppointmentSlot
{
    [Required]
    public int? Id { get; set; }

    [Required]
    public DateOnly SlotDate { get; set; }
    public TimeOnly SlotStart { get; set; }
    public int? OrderId { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; } = "free";
}

public class RaceCart
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int Position { get; set; }
}

public class UserCart
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }  // FK only

    [Required]
    public int RaceCartId { get; set; }  // FK only
}

public class CircleResults
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int RaceCartId { get; set; }  // FK only

    [Required]
    public int CircleNum { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan CircleTime { get; set; }
}

public class RaceStatus
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; }
}

public class RaceCategory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Category { get; set; }
}