using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class InstitutionLecturer
{
    public Guid Id { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid UserId { get; set; }

    public int? Years { get; set; }

    public virtual Institution Institution { get; set; } = null!;

    public virtual ICollection<LecturerProgramme> LecturerProgrammes { get; } = new List<LecturerProgramme>();

    public virtual User User { get; set; } = null!;
}
