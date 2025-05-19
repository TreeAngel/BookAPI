using System;
using System.Collections.Generic;

namespace BookAPI.Entities;

public partial class Book
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Publisher { get; set; } = null!;

    public DateOnly PublishDate { get; set; }

    public string ImageCover { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();

    public virtual Wishlist? Wishlist { get; set; }
}
