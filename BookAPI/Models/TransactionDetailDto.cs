using BookAPI.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookAPI.Models
{
    public class TransactionDetailDto
    {
        [Key]
        public int Id { get; set; }

        [JsonIgnore]
        public int TransactionId { get; set; }

        [Key]
        public int BookId { get; set; }

        public int Qty { get; set; }

        public long TotalPrice { get; set; }
    }

    public class TDetailsDto
    {
        public int Id { get; set; }

        public int Qty { get; set; }

        public long TotalPrice { get; set; }

        public virtual BookDto Book { get; set; } = null!;
    }

    public class CreateTransactionDto
    {
        [Key, Required]
        public int BookId { get; set; }

        [Required]
        public int Qty { get; set; } = 1;
    }
}
