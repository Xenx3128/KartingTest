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

public class Cart
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [StringLength(50)]
    public string? Name { get; set; }

    [StringLength(500)]
    public string? Desc { get; set; }

}

public class UserRace
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }

    [Required]
    public int RaceId { get; set; }

    [ForeignKey("RaceId")]
    public Races Race { get; set; }


    [Required]
    public int? CartId { get; set; }

    [ForeignKey("CartId")]
    public Cart? Cart { get; set; }


    public int? Position { get; set; }


}

public class CircleResults
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserRaceId { get; set; }

    [ForeignKey("UserRaceId")]
    public UserRace UserRace { get; set; }

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