using BookAPI.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookAPI.Models
{
    public class UserDto
    {
        [Key]
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        [Key]
        public string Username { get; set; } = null!;

        [MinLength(4), JsonIgnore]
        public string Password { get; set; } = null!;

        [AllowedValues("admin", "user"), JsonIgnore]
        public string Role { get; set; } = null!;

        public string ImageProfile { get; set; } = null!;
    }

    public class LoginRequestDto
    {
        [Required, Key]
        public string Username { get; set; } = null!;

        [Required, MinLength(4)]
        public string Password { get; set; } = null!;
    }

    public class RegisterRequestDto
    {
        [Required]
        public string FullName { get; set; } = null!;

        [Required, Key]
        public string Username { get; set; } = null!;

        [Required, MinLength(4)]
        public string Password { get; set; } = null!;
    }

    public class UpdateProfileDto
    {
        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string Username { get; set; } = null!;
    }

    public class UploadImageProfile
    {
        public IFormFile? Image;
    }
}
