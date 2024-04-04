using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Post
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? Image { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? UserId { get; set; }

    public int? CategoryPostId { get; set; }

    public virtual CategoryPost? CategoryPost { get; set; }

    public virtual ICollection<PostReply> PostReplies { get; set; } = new List<PostReply>();

    public virtual User? User { get; set; }
}
