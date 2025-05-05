using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestMVC.Models;

public class TechnicalBreaks
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime DateStart { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime DateFinish { get; set; }

    [StringLength(50)]
    public string Desc { get; set; }

    [Required]
    public int BreakStatusId { get; set; }
    
    [ForeignKey("BreakStatusId")]
    public BreakStatuses BreakStatus { get; set; }
}

public class BreakStatuses
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; }
}

public class Settings
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan DayStart { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan DayFinish { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan RaceDuration { get; set; }
}