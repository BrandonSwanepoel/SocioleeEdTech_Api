using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class Institution
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<InstitutionCourse> InstitutionCourses { get; } = new List<InstitutionCourse>();

    public virtual ICollection<InstitutionLecturer> InstitutionLecturers { get; } = new List<InstitutionLecturer>();

    public virtual ICollection<Student> Students { get; } = new List<Student>();
}
