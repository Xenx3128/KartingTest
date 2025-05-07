using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace TestMVC.Models
{

    public class ApplicationUser : IdentityUser<int>
    {
        [StringLength(255)]
        public string? FullName { get; set; } 
        
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        

        [StringLength(255)]
        public string? FromWhereFoundOut { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        public bool AcceptTerms { get; set; }
        public bool ReceivePromotions { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public virtual ICollection<IdentityUserRole<int>>? UserRoles { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }

    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }

        public virtual ICollection<IdentityUserRole<int>> UserRoles { get; set; }
    }
}