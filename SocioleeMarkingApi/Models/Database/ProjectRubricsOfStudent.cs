using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class ProjectRubricsOfStudent
{
    public Guid Id { get; set; }

    public Guid RubricId { get; set; }

    public Guid StudentId { get; set; }

    public string? Comment { get; set; }

    public double? Mark { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ProjectRubric Rubric { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
