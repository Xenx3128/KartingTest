using System.ComponentModel.DataAnnotations;

namespace TestMVC.Models
{
    public class RegisterViewModel
    {
        // Identity Core Properties
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        // Extended Properties from CartUser
        [Required]
        [StringLength(255)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        [Display(Name = "How did you hear about us?")]
        public string? FromWhereFoundOut { get; set; }

        public bool AcceptTerms { get; set; }
        public bool ReceivePromotions { get; set; }
        
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class ProfileViewModel
    {
        public string Id { get; set; }

        [StringLength(255)]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [StringLength(500)]
        public string Note { get; set; }
    }

    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [EmailAddress(ErrorMessage = "Некорректный email адрес")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Phone(ErrorMessage = "Некорректный номер телефона")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string? OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        [StringLength(100, ErrorMessage = "Пароль должен содержать от {2} до {1} символов", MinimumLength = 6)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string? ConfirmPassword { get; set; }
    }

    public class RaceHistoryViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public string Category { get; set; }
        public string CartName { get; set; }
        public int? Position { get; set; }
    }

    
public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(255, ErrorMessage = "Имя не может превышать 255 символов")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Электронная почта обязательна")]
        [EmailAddress(ErrorMessage = "Неверный формат электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Номер телефона обязателен")]
        [Phone(ErrorMessage = "Неверный формат номера телефона")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Дата рождения обязательна")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль должен содержать от 8 до 100 символов")]
        public string Password { get; set; }

        [StringLength(255, ErrorMessage = "Текст не может превышать 255 символов")]
        public string? FromWhereFoundOut { get; set; }

        [Required(ErrorMessage = "Роль обязательна")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Статус обязателен")]
        public string Status { get; set; } // "Active" or "Banned"

        [StringLength(500, ErrorMessage = "Примечание не может превышать 500 символов")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "Необходимо принять правила безопасности")]
        public bool AcceptSafetyRules { get; set; }

        public bool ReceivePromotions { get; set; }
    }

    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(255, ErrorMessage = "Имя не может превышать 255 символов")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Электронная почта обязательна")]
        [EmailAddress(ErrorMessage = "Неверный формат электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Номер телефона обязателен")]
        [Phone(ErrorMessage = "Неверный формат номера телефона")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Дата рождения обязательна")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль должен содержать от 8 до 100 символов")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string? ConfirmPassword { get; set; }

        [StringLength(255, ErrorMessage = "Текст не может превышать 255 символов")]
        public string? FromWhereFoundOut { get; set; }

        [Required(ErrorMessage = "Роль обязательна")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Статус обязателен")]
        public string Status { get; set; } // "Active" or "Banned"

        [StringLength(500, ErrorMessage = "Примечание не может превышать 500 символов")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "Необходимо принять правила безопасности")]
        public bool AcceptTerms { get; set; }

        public bool ReceivePromotions { get; set; }
    }

    // Model for user dropdown
    public class UserDropdownModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
    }
    
    public class OrderingModel
    {
        [Required(ErrorMessage = "Дата обязательна")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Выберите хотя бы одно время")]
        public List<TimeOnly> Times { get; set; }

        [Required(ErrorMessage = "Выберите тип заезда")]
        public bool IsUniform { get; set; }

        [Required(ErrorMessage = "Выберите категорию заезда")]
        public List<int> RaceCategoryIds { get; set; }

        [Required(ErrorMessage = "Необходимо принять технику безопасности")]
        public bool TermsAccepted { get; set; }
    }

}
