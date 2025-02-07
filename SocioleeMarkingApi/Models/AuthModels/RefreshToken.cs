using System;
using SocioleeMarkingApi.Common;

namespace SocioleeMarkingApi.Models.AuthModels
{
	public class RefreshToken
	{
		public string Token { get; set; } = string.Empty;
		public DateTime Created { get; set; } = CommonFunctions.CurrentDateTime();
		public DateTime Expires { get; set; }
	}
}

