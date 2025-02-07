using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class StudentCourse
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public Guid InstitutionCoursesId { get; set; }

    public virtual InstitutionCourse InstitutionCourses { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
