using System;
using System.Collections.Generic;

namespace AgriculturalForum.Web.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Photo { get; set; }

    public bool IsWorking { get; set; }

    public string? RoleNames { get; set; }
}
