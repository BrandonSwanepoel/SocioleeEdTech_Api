using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class StepType
{
    public Guid Id { get; set; }

    public int Index { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ContentStep> ContentSteps { get; } = new List<ContentStep>();
}
