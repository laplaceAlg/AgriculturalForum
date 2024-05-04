using System;
using System.Collections.Generic;

namespace AgriculturalForum.Web.Models;

public partial class Province
{
    public string ProvinceName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
