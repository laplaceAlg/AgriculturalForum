using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class CategoryPost
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreateDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
