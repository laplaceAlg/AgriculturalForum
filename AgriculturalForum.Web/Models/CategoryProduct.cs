using System;
using System.Collections.Generic;

namespace AgriculturalForum.Web.Models;

public partial class CategoryProduct
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? CreateDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
