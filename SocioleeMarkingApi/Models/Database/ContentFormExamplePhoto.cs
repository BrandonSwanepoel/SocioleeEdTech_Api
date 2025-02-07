using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class ContentFormExamplePhoto
{
    public Guid ContentFormId { get; set; }

    public Guid ExamplePhotoId { get; set; }

    public Guid Id { get; set; }

    public virtual ContentForm ContentForm { get; set; } = null!;
}
