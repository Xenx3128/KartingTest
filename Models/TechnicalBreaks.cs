using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace TestMVC.Models;

public class TechnicalBreaks
{
    [Key]
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime FinishDate { get; set; }
}