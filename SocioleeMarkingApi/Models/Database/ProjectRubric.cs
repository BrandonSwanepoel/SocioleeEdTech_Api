using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class ProjectRubric
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public double Weight { get; set; }

    public int SortOrder { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual StudentProject Project { get; set; } = null!;

    public virtual ICollection<ProjectRubricsOfStudent> ProjectRubricsOfStudents { get; } = new List<ProjectRubricsOfStudent>();
}
