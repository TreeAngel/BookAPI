using BookAPI.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookAPI.Models
{
    public class BookGenreDto
    {
        [Key]
        public int BookId { get; set; }

        [Key]
        public int GenreId { get; set; }

        [JsonIgnore]
        public virtual Book Book { get; set; } = null!;

        [JsonIgnore]
        public virtual Genre Genre { get; set; } = null!;
    }
}
