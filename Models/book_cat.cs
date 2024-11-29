using System.ComponentModel.DataAnnotations;
//using Microsoft.EntityFrameworkCore;

namespace TestMVC.Models;

public class BookCat{
    [Key]
    public string? Isbn { get; set;}
    [Key]
    public string? CatName { get; set;}

}