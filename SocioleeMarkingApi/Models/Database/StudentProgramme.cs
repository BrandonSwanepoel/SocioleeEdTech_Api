using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class StudentProgramme
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public Guid InstitutionProgrammeId { get; set; }

    public virtual InstitutionProgramme InstitutionProgramme { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
