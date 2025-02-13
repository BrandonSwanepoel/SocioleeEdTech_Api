using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class InstitutionProgramme
{
    public Guid Id { get; set; }

    public Guid InstitutionId { get; set; }

    public string Programme { get; set; } = null!;

    public virtual Institution Institution { get; set; } = null!;

    public virtual ICollection<LecturerProgramme> LecturerProgrammes { get; } = new List<LecturerProgramme>();

    public virtual ICollection<StudentProgramme> StudentProgrammes { get; } = new List<StudentProgramme>();

    public virtual ICollection<StudentProject> StudentProjects { get; } = new List<StudentProject>();
}
