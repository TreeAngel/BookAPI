using System;
using System.Collections.Generic;

namespace BookAPI.Entities;

public partial class TransactionDetail
{
    public int Id { get; set; }

    public int TransactionId { get; set; }

    public int BookId { get; set; }

    public int Qty { get; set; }

    public long TotalPrice { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Transaction Transaction { get; set; } = null!;
}
