using System;
using System.Collections.Generic;

namespace BookAPI.Entities;

public partial class Transaction
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public int UserId { get; set; }

    public long Subtotal { get; set; }

    public DateOnly TransactionDate { get; set; }

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();

    public virtual User User { get; set; } = null!;
}
