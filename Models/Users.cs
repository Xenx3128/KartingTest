using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestMVC.Models;

/*public class CartUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string UserName { get; set; }

    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    [Required]
    [StringLength(20)]
    public string PhoneNum { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; }

    [Required]
    [StringLength(255)]
    public string Pwd { get; set; }

    [StringLength(50)]
    public string UserRole { get; set; }

    [StringLength(255)]
    public string FromWhereFoundOut { get; set; }

    [StringLength(50)]
    public string Status { get; set; }

    [StringLength(500)]
    public string Note { get; set; }
}*/

public class UserStatus
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; }
}