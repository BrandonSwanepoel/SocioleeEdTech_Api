using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class ContentAsset
{
    public Guid Id { get; set; }

    public Guid ContentFormId { get; set; }

    public string Type { get; set; } = null!;

    public string ContentPath { get; set; } = null!;

    public DateTime Created { get; set; }

    public Guid? NoteId { get; set; }

    public bool Downloaded { get; set; }

    public Guid? UserId { get; set; }

    public Guid AssetAspectRatioId { get; set; }

    public virtual AssetAspectRatio AssetAspectRatio { get; set; } = null!;

    public virtual ContentForm ContentForm { get; set; } = null!;

    public virtual DesignChangeRequest? Note { get; set; }

    public virtual User? User { get; set; }
}
