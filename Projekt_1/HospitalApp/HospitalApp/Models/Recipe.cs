using System;
using System.Collections.Generic;

namespace HospitalApp.Models;

public partial class Recipe
{
    public int Id { get; set; }

    public string Medicine { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateOnly StartOfMedication { get; set; }

    public DateOnly EndOfMedication { get; set; }

    public int DoctorId { get; set; }

    public int PatientId { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
