using System;
namespace SocioleeMarkingApi.Common
{
	public class CommonFunctions
	{
		public static string CreateAddress(string? streetAddress, string suburb, string city, string province, string country, string? zipCode)
		{
			var address = "";
			if (streetAddress != "")
			{
				address += streetAddress + ", ";
			}
			address += $"{suburb}, {city}, {province}, {country}";
			if (zipCode != "")
			{
				address += ", " + zipCode;
			}
			
			return address;
		}

		public static DateTime CurrentDateTime() {
			var datetime = DateTimeOffset.UtcNow;
			return datetime.LocalDateTime;
		}

		public static DateTime CurrentDateTimeAddDays(int days)
		{
			var datetime = DateTimeOffset.UtcNow.AddDays(days);
			return datetime.LocalDateTime;
		}

		public static DateTime CurrentDateTimeAddMonths(int months)
		{
			var datetime = DateTimeOffset.UtcNow.AddMonths(months);
			return datetime.LocalDateTime;
		}

		public static string GenerateRandomCode(int length)
		{
			Random _random = new();
			const string chars = "0123456789";
			return new string(Enumerable.Repeat(chars, length)
										.Select(s => s[_random.Next(s.Length)])
										.ToArray());
		}
	}
}

