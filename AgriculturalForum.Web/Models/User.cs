using System;
using System.Collections.Generic;

namespace AgriculturalForum.Web.Models;

public partial class User
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string? ProfileImage { get; set; }

    public DateTime? MemberSince { get; set; }

    public string? Address { get; set; }

    public string? Province { get; set; }

    public DateTime? Birthday { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<PostReply> PostReplies { get; set; } = new List<PostReply>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual Province? ProvinceNavigation { get; set; }
}
