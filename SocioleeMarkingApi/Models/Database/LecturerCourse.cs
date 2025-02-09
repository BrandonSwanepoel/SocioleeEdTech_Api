using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class LecturerCourse
{
    public Guid Id { get; set; }

    public Guid LecturerId { get; set; }

    public Guid InstitutionCoursesId { get; set; }

    public virtual InstitutionCourse InstitutionCourses { get; set; } = null!;

    public virtual InstitutionLecturer Lecturer { get; set; } = null!;
}
