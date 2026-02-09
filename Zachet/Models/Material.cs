using System;
using System.Collections.Generic;

namespace Zachet.Models;

public partial class Material
{
    public int MaterialId { get; set; }

    public string MaterialName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
