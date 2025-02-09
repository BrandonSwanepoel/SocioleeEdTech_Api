using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class InstitutionCourse
{
    public Guid Id { get; set; }

    public Guid InstitutionId { get; set; }

    public string Course { get; set; } = null!;

    public virtual Institution Institution { get; set; } = null!;

    public virtual ICollection<LecturerCourse> LecturerCourses { get; } = new List<LecturerCourse>();

    public virtual ICollection<StudentCourse> StudentCourses { get; } = new List<StudentCourse>();

    public virtual ICollection<StudentProject> StudentProjects { get; } = new List<StudentProject>();
}
