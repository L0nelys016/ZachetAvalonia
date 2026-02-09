using System;
using System.Collections.Generic;

namespace Zachet.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Article { get; set; } = null!;

    public decimal Price { get; set; }

    public string ProductName { get; set; } = null!;

    public int TypeId { get; set; }

    public int MaterialId { get; set; }

    public DateOnly ProductionDate { get; set; }

    public virtual Material Material { get; set; } = null!;

    public virtual ProductType Type { get; set; } = null!;
}
