using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class Message
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid RequestId { get; set; }

    public DateTime Created { get; set; }

    public string? TextMessage { get; set; }

    public Guid? AssetId { get; set; }

    public bool? Deleted { get; set; }

    public virtual MessageAsset? Asset { get; set; }

    public virtual ICollection<MessageAsset> MessageAssets { get; } = new List<MessageAsset>();

    public virtual ContentForm Request { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
