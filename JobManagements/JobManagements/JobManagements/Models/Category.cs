using System;
using System.Collections.Generic;

namespace JobManagements.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<Job> Jobs { get; } = new List<Job>();
}
