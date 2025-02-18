using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class ProjectFile
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public Guid StudentId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string Type { get; set; } = null!;

    public virtual StudentProject Project { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
