using System;
using System.Collections.Generic;

namespace PTongHop.Models​;

public partial class Cart
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Image { get; set; }

    public int Quantity { get; set; }

    public double Price { get; set; }

    public double Total { get; set; }
}
