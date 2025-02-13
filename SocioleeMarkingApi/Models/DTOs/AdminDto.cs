using System;
using EllipticCurve;
using SocioleeMarkingApi.Models.Database;

namespace SocioleeMarkingApi.Models.DTOs
{

	public class InstitutionDTO
	{
		public Guid? Id { get; set; }

		public string Name { get; set; } = null!;

		public string? Address { get; set; }

		public string? Email { get; set; }

		public string? PhoneNumber { get; set; }

		public List<string> Programmes { get; set; } = new List<string>();
	}

	public class UpsertLecturerDTO
	{
		public Guid? Id { get; set; }
		public Guid InstitutionId { get; set; }
		public string Name { get; set; } = null!;
		public int? Years { get; set; }
		public List<Guid>? Programmes { get; set; } = new List<Guid>();
		public string Email { get; set; } = null!;
		public RoleType Role { get; set; } = null!;
	}
}

