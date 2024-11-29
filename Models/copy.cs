using System.ComponentModel.DataAnnotations;
//using Microsoft.EntityFrameworkCore;

namespace TestMVC.Models;

public class BookCopy{
    [Key]
    public string? Isbn { get; set;}
    [Key]
    public int CopyNumber { get; set;}
    public string? Shelf { get; set;}
    public string? Position { get; set;}

}