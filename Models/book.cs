using System.ComponentModel.DataAnnotations;
//using Microsoft.EntityFrameworkCore;

namespace TestMVC.Models;

public class Book{
    [Key]
    public string? Isbn { get; set;}
    public string? Title { get; set;}
    public string? Author { get; set;}
    public int PagesNum { get; set;}
    public int PubYear { get; set;}
    
    public string? PubName { get; set;}
}