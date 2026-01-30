using System.ComponentModel.DataAnnotations;

namespace MyStreetlight2._0.DTOs.UserDtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
