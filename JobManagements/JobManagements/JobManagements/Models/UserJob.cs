using System;
using System.Collections.Generic;

namespace JobManagements.Models;

public partial class UserJob
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string? Message { get; set; }

    public string? City { get; set; }

    public DateTime? Date { get; set; }

    public string? UserId { get; set; }

    public int? JobId { get; set; }

    public virtual Job? Job { get; set; }

    public virtual AspNetUser? User { get; set; }
}


