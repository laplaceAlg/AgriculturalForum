using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgriculturalForum.Web.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }
   
    public double? Price { get; set; }

    public bool IsSelling { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? UserId { get; set; }

    public int? CategoryProductId { get; set; }

    public virtual CategoryProduct? CategoryProduct { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual User? User { get; set; }
}
