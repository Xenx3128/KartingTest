using System.ComponentModel.DataAnnotations;
//using Microsoft.EntityFrameworkCore;

namespace TestMVC.Models;

public class Borrowing{
    [Key]
    public int ReaderNum { get; set;}
    [Key]
    public string? Isbn { get; set;}
    public int CopyNumber { get; set;}
    public DateTime ReturnDate { get; set;}

}