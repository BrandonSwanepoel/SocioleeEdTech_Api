using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class AssetAspectRatio
{
    public Guid Id { get; set; }

    public int Index { get; set; }

    public string AspectRatio { get; set; } = null!;

    public string Usage { get; set; } = null!;

    public virtual ICollection<ContentAsset> ContentAssets { get; } = new List<ContentAsset>();

    public virtual ICollection<ContentForm> ContentForms { get; } = new List<ContentForm>();
}
