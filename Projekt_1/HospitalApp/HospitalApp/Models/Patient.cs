using System;
using System.Collections.Generic;

namespace HospitalApp.Models;

public partial class Patient
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Oib { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public char Gender { get; set; }

    public virtual ICollection<Checkup> Checkups { get; set; } = new List<Checkup>();

    public virtual ICollection<MedicalDocument> MedicalDocuments { get; set; } = new List<MedicalDocument>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
