using BookAPI.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookAPI.Models
{
    public class TransactionDto
    {
        [Key]
        public int Id { get; set; }

        [Key]
        public string Code { get; set; } = null!;

        [Key]
        public int UserId { get; set; }

        public long Subtotal { get; set; }

        public DateOnly TransactionDate { get; set; }

        [JsonIgnore]
        public virtual ICollection<TransactionDetailDto> TransactionDetails { get; set; } = [];

        [JsonIgnore]
        public virtual UserDto User { get; set; } = null!;
    }
}
