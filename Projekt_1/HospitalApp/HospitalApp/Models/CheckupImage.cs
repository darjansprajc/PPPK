using System;
using System.Collections.Generic;

namespace HospitalApp.Models;

public partial class CheckupImage
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string ImgPath { get; set; } = null!;

    public int CheckupId { get; set; }

    public virtual Checkup Checkup { get; set; } = null!;
}
