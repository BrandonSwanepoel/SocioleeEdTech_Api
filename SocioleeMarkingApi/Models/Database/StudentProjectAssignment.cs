using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class StudentProjectAssignment
{
    public Guid Id { get; set; }

    public Guid StudentProjectId { get; set; }

    public Guid AssignedBy { get; set; }

    public Guid AssignedTo { get; set; }

    public DateTime? AssignedAt { get; set; }

    public virtual User AssignedByNavigation { get; set; } = null!;

    public virtual Student AssignedToNavigation { get; set; } = null!;

    public virtual StudentProject StudentProject { get; set; } = null!;
}
