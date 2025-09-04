using System;
using System.Collections.Generic;

namespace HospitalApp.Models;

public partial class Doctor
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual ICollection<CheckupDoctor> CheckupDoctors { get; set; } = new List<CheckupDoctor>();

    public virtual ICollection<MedicalDocument> MedicalDocuments { get; set; } = new List<MedicalDocument>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
