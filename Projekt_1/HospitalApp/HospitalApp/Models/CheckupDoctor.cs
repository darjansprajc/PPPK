using System;
using System.Collections.Generic;

namespace HospitalApp.Models;

public partial class CheckupDoctor
{
    public int Id { get; set; }

    public int CheckupId { get; set; }

    public int DoctorId { get; set; }

    public virtual Checkup Checkup { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;
}
