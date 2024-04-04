using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class PostReply
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public int? UserId { get; set; }

    public int? PostId { get; set; }

    public virtual Post? Post { get; set; }

    public virtual User? User { get; set; }
}
