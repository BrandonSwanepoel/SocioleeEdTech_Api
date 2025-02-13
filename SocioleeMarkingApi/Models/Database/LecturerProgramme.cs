using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class LecturerProgramme
{
    public Guid Id { get; set; }

    public Guid LecturerId { get; set; }

    public Guid InstitutionProgrammeId { get; set; }

    public virtual InstitutionProgramme InstitutionProgramme { get; set; } = null!;

    public virtual InstitutionLecturer Lecturer { get; set; } = null!;
}
