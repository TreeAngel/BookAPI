using BookAPI.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookAPI.Models
{
    public class WishlistDto
    {
        [Key]
        public int Id { get; set; }

        [Key]
        public int UserId { get; set; }

        [Key]
        public int BookId { get; set; }

        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public DateTime? DeletedAt { get; set; }

        [JsonIgnore]
        public virtual BookDto Book { get; set; } = null!;

        [JsonIgnore]
        public virtual UserDto User { get; set; } = null!;
    }

    public class GetWishlistDto
    {
        public DateTime CreatedAt { get; set; }

        public virtual BookDto Book { get; set; } = null!;
    }
}
