using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TO_DO.DTOs.Auth;

public class RegisterRequest
{
    //[EmailAddress]
    //[Required]
    public string Email { get; set; } = string.Empty;
    //[MinLength(6)]
    //[Required]
    public string Password { get; set; } = string.Empty;

    //[UserName]
    //public string UserName { get; set; } = string.Empty;
}


//public class UserNameAttribute : ValidationAttribute
//{
//    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
//    {
//        if (value is not string userName)
//        {
//            return new ValidationResult("UserName is not a string");
//        }
//        if (userName.Length < 2)
//        {
//            return new ValidationResult("UserName is short");
//        }
//        if (!new Regex("$\\w{2, }^").IsMatch(userName))
//        {
//            return new ValidationResult("UserName can contains only letters");
//        }
//        return ValidationResult.Success;
//    }
//}