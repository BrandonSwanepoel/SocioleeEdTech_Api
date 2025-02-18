using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class StudentProject
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int Year { get; set; }

    public double YearWeight { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? StartDateTime { get; set; }

    public DateTime? EndDateTime { get; set; }

    public Guid InstitutionProgrammeId { get; set; }

    public Guid InstitutionId { get; set; }

    public string Subject { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Institution Institution { get; set; } = null!;

    public virtual InstitutionProgramme InstitutionProgramme { get; set; } = null!;

    public virtual ICollection<ProjectFile> ProjectFiles { get; } = new List<ProjectFile>();

    public virtual ICollection<ProjectRubricStudentMark> ProjectRubricStudentMarks { get; } = new List<ProjectRubricStudentMark>();

    public virtual ICollection<ProjectRubric> ProjectRubrics { get; } = new List<ProjectRubric>();

    public virtual ICollection<StudentProjectAssignment> StudentProjectAssignments { get; } = new List<StudentProjectAssignment>();
}
