using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class ContentStep
{
    public Guid Id { get; set; }

    public Guid ContentFormId { get; set; }

    public Guid StepTypeId { get; set; }

    public Guid DesignerId { get; set; }

    public bool Downloaded { get; set; }

    public bool? Completed { get; set; }

    public virtual ContentForm ContentForm { get; set; } = null!;

    public virtual StepType StepType { get; set; } = null!;
}
