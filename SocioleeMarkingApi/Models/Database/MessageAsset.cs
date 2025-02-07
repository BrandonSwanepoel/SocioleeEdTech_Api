using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class MessageAsset
{
    public Guid Id { get; set; }

    public Guid MessageId { get; set; }

    public string Type { get; set; } = null!;

    public string ContentPath { get; set; } = null!;

    public Guid? NoteId { get; set; }

    public bool Downloaded { get; set; }

    public bool? ReviewDesign { get; set; }

    public bool? Reviewed { get; set; }

    public virtual ICollection<DesignChangeRequest> DesignChangeRequests { get; } = new List<DesignChangeRequest>();

    public virtual Message Message { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; } = new List<Message>();

    public virtual DesignChangeRequest? Note { get; set; }
}
