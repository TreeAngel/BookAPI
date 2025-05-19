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

        [JsonIgnore]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public DateTime? DeletedAt { get; set; }

        [JsonIgnore]
        public virtual ICollection<TransactionDto> Transactions { get; set; } = [];

        [JsonIgnore]
        public virtual WishlistDto? Wishlist { get; set; }
    }

    public class LoginRequest
    {
        [Required, Key]
        public string Username { get; set; } = null!;

        [Required, MinLength(4)]
        public string Password { get; set; } = null!;
    }

    public class RegisterRequest
    {
        [Required]
        public string FullName { get; set; } = null!;

        [Required, Key]
        public string Username { get; set; } = null!;

        [Required, MinLength(4)]
        public string Password { get; set; } = null!;
    }
}
