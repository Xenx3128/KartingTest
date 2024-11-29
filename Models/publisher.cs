using System.ComponentModel.DataAnnotations;
//using Microsoft.EntityFrameworkCore;

namespace TestMVC.Models;

public class Publisher{
    [Key]
    public string? PubName { get; set;}
    public string? PubKind { get; set;}
}