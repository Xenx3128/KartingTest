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

    [ForeignKey("OrderId")]
    public Order Order { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime FinishDate { get; set; }

    [Required]
    public int RaceCategoryId { get; set; }

    [ForeignKey("RaceCategoryId")]
    public RaceCategory RaceCategory { get; set; }

    [Required]
    public int RaceStatusId { get; set; }
    
    [ForeignKey("RaceStatusId")]
    public RaceStatus RaceStatus { get; set; }
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
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }

    [Required]
    public int RaceCartId { get; set; }

    [ForeignKey("RaceCartId")]
    public RaceCart RaceCart { get; set; }
}

public class CircleResults
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int RaceCartId { get; set; }

    [ForeignKey("RaceCartId")]
    public RaceCart RaceCart { get; set; }

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