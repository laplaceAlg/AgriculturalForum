using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class ProductImage
{
    public int Id { get; set; }

    public string Image { get; set; } = null!;

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}
