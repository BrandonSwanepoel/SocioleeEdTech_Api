using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class ContentForm
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? ContentType { get; set; }

    public Guid? LogoId { get; set; }

    public string? ContentBodyText { get; set; }

    public string? ContentDescription { get; set; }

    public Guid? VoiceNoteId { get; set; }

    public bool IncludeContactDetails { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime Created { get; set; }

    public Guid AssetAspectRatioId { get; set; }

    public Guid? BackgroundImageId { get; set; }

    public string Title { get; set; } = null!;

    public string? ContentHeadline { get; set; }

    public virtual AssetAspectRatio AssetAspectRatio { get; set; } = null!;

    public virtual ICollection<ContentAsset> ContentAssets { get; } = new List<ContentAsset>();

    public virtual ICollection<ContentFormExamplePhoto> ContentFormExamplePhotos { get; } = new List<ContentFormExamplePhoto>();

    public virtual ICollection<ContentFormPrimaryColor> ContentFormPrimaryColors { get; } = new List<ContentFormPrimaryColor>();

    public virtual ICollection<ContentStep> ContentSteps { get; } = new List<ContentStep>();

    public virtual ICollection<DesignChangeRequest> DesignChangeRequests { get; } = new List<DesignChangeRequest>();

    public virtual ICollection<Message> Messages { get; } = new List<Message>();

    public virtual User User { get; set; } = null!;
}
