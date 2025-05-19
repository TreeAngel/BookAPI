using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookAPI.Models
{
    public class GenreDto
    {
        [Key, JsonIgnore]
        public int Id { get; set; }

        [Key]
        public string Name { get; set; } = null!;
    }
}
