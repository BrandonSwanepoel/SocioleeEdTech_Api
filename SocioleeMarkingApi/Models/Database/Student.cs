using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class Student
{
    public Guid Id { get; set; }

    public int Year { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid UserId { get; set; }

    public virtual Institution Institution { get; set; } = null!;

    public virtual ICollection<ProjectRubricStudentMark> ProjectRubricStudentMarks { get; } = new List<ProjectRubricStudentMark>();

    public virtual ICollection<ProjectRubricsOfStudent> ProjectRubricsOfStudents { get; } = new List<ProjectRubricsOfStudent>();

    public virtual ICollection<StudentProgramme> StudentProgrammes { get; } = new List<StudentProgramme>();

    public virtual ICollection<StudentProjectAssignment> StudentProjectAssignments { get; } = new List<StudentProjectAssignment>();

    public virtual User User { get; set; } = null!;
}
