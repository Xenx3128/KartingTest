using System.ComponentModel.DataAnnotations;
//using Microsoft.EntityFrameworkCore;

namespace TestMVC.Models;

public class Category{
    [Key]
    public string? CatName { get; set;}
    public string? ParentCat { get; set;}
}