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
        public string PhoneNum { get; set; }

        [StringLength(255)]
        [Display(Name = "How did you hear about us?")]
        public string FromWhereFoundOut { get; set; }

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
        public string PhoneNum { get; set; }

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
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        [StringLength(100, ErrorMessage = "Пароль должен содержать от {2} до {1} символов", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }

    public class RaceHistoryViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public string Category { get; set; }
        public int Position { get; set; }
    }

    
    public class CreateUserViewModel
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

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "Пароль должен содержать от {2} до {1} символов", MinimumLength = 6)]
        public string Password { get; set; }

        public string FromWhereFoundOut { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Status { get; set; }

        public string Note { get; set; }

        [Required(ErrorMessage = "Необходимо принять правила техники безопасности")]
        [Display(Name = "Техника безопасности")]
        public bool AcceptSafetyRules { get; set; }

        [Display(Name = "Получать промо-предложения")]
        public bool ReceivePromotions { get; set; }
    }

    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ФИО обязательно")]
        [StringLength(255, ErrorMessage = "ФИО не может превышать 255 символов")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Электронная почта обязательна")]
        [EmailAddress(ErrorMessage = "Неверный формат электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Номер телефона обязателен")]
        [RegularExpression(@"^(\+7|7|8)[\s\-]?\(?[489][0-9]{2}\)?[\s\-]?[0-9]{3}[\s\-]?[0-9]{2}[\s\-]?[0-9]{2}$", 
            ErrorMessage = "Неверный формат номера телефона")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Дата рождения обязательна")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [StringLength(255, ErrorMessage = "Текст не может превышать 255 символов")]
        public string FromWhereFoundOut { get; set; }

        [StringLength(50, ErrorMessage = "Статус не может превышать 50 символов")]
        public string Status { get; set; }

        [StringLength(500, ErrorMessage = "Примечание не может превышать 500 символов")]
        public string Note { get; set; }

        [Required(ErrorMessage = "Необходимо подтвердить технику безопасности")]
        public bool AcceptTerms { get; set; }

        public bool ReceivePromotions { get; set; }

        public string Role { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
