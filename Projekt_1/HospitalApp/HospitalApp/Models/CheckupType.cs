using System;
using System.Collections.Generic;

namespace HospitalApp.Models;

public partial class CheckupType
{
    public int Id { get; set; }

    public string? Acronym { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Checkup> Checkups { get; set; } = new List<Checkup>();
}
