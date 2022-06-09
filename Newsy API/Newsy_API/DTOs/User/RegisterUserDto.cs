using Newsy_API.AuthenticationModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Newsy_API.DTOs.User
{
    public class RegisterUserDto
    {
        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; } = null!;


        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; } = null!;


        [Required(AllowEmptyStrings = false)]
        public string EMail { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$",
           ErrorMessage = "Password must be at least 6 characters and containt a least upper case letter, one lowercase letter, one number and one special character")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        [DefaultValue("reader")]
        public string RoleKey { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Enum.TryParse<ApplicationUserRoles>(RoleKey, true, out _))
            {
                yield return new ValidationResult("Role key is not valid ", new List<string> { "RoleKey" });
            }
        }
    }
}
