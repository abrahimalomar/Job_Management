using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobManagements.Models;

public partial class Job
{
    public int Id { get; set; }

    public string? JobTitle { get; set; }

    public string? JobDescription { get; set; }

    public int? Salary { get; set; }

    public DateTime? Date { get; set; }

    public string? Location { get; set; }

    public string? UserId { get; set; }

    public int? CategoryId { get; set; }
    public string? ImageSrc { get; set; }
    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    public virtual Category? Category { get; set; }

    public virtual AspNetUser? User { get; set; }

    public virtual ICollection<UserJob> UserJobs { get; } = new List<UserJob>();
}
