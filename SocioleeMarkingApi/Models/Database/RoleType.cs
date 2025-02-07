using System;
using System.Collections.Generic;

namespace SocioleeMarkingApi.Models.Database;

public partial class RoleType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; } = new List<User>();
}
