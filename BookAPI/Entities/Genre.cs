﻿using System;
using System.Collections.Generic;

namespace BookAPI.Entities;

public partial class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}
