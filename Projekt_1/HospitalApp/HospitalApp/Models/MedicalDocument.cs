using System;
using System.Collections.Generic;

namespace HospitalApp.Models;

public partial class MedicalDocument
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Sickness { get; set; } = null!;

    public string Diagnosis { get; set; } = null!;

    public DateOnly StartOfSickness { get; set; }

    public DateOnly? EndOfSickness { get; set; }

    public int DoctorId { get; set; }

    public int PatientId { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
