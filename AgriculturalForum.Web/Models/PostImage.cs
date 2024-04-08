using System;
using System.Collections.Generic;

namespace AgriculturalForum.Web.Models;

public partial class PostImage
{
    public int Id { get; set; }

    public string Image { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public bool IsHidden { get; set; }

    public int PostId { get; set; }

    public virtual Post Post { get; set; } = null!;
}
