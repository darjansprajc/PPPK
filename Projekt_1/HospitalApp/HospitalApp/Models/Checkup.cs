using System;
using System.Collections.Generic;

namespace HospitalApp.Models;

public partial class Checkup
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Time { get; set; }

    public int TypeId { get; set; }

    public int PatientId { get; set; }

    public virtual ICollection<CheckupDoctor> CheckupDoctors { get; set; } = new List<CheckupDoctor>();

    public virtual ICollection<CheckupImage> CheckupImages { get; set; } = new List<CheckupImage>();

    public virtual Patient Patient { get; set; } = null!;

    public virtual CheckupType Type { get; set; } = null!;
}
