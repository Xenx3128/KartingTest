using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace TestMVC.Models
{

    public class ApplicationUser : IdentityUser<int> // Inherit from IdentityUser
    {
        // Keep all CartUser properties
        [StringLength(255)]
        public string? FullName { get; set; }  // Replaces UserName
        
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        
        // Identity brings these automatically:
        // - Email
        // - PasswordHash
        // - UserName 
        // - etc.

        [StringLength(255)]
        public string? FromWhereFoundOut { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        public bool AcceptTerms { get; set; }
        public bool ReceivePromotions { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public virtual ICollection<IdentityUserRole<int>>? UserRoles { get; set; }

        // Navigation property for orders
        public virtual ICollection<Order> Orders { get; set; }
    }

    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }

        public virtual ICollection<IdentityUserRole<int>> UserRoles { get; set; }
    }
}