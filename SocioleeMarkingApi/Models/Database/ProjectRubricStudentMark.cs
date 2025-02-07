using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class ProjectRubricStudentMark
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public Guid StudentId { get; set; }

    public double Mark { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual StudentProject Project { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
