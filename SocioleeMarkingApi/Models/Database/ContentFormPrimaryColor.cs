using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class ContentFormPrimaryColor
{
    public Guid ContentFormId { get; set; }

    public string PrimaryColor { get; set; } = null!;

    public Guid Id { get; set; }

    public virtual ContentForm ContentForm { get; set; } = null!;
}
