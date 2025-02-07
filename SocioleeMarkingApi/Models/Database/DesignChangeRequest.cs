using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class DesignChangeRequest
{
    public Guid Id { get; set; }

    public Guid ContentFormId { get; set; }

    public DateTime Created { get; set; }

    public Guid UserId { get; set; }

    public Guid ContentAssetId { get; set; }

    public int Number { get; set; }

    public string Text { get; set; } = null!;

    public double X { get; set; }

    public double Y { get; set; }

    public virtual MessageAsset ContentAsset { get; set; } = null!;

    public virtual ICollection<ContentAsset> ContentAssets { get; } = new List<ContentAsset>();

    public virtual ContentForm ContentForm { get; set; } = null!;

    public virtual ICollection<MessageAsset> MessageAssets { get; } = new List<MessageAsset>();

    public virtual User User { get; set; } = null!;
}
