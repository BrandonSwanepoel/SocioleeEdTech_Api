using Microsoft.EntityFrameworkCore;
using SocioleeMarkingApi.Common;
using SocioleeMarkingApi.Enums.EmailTracking;
using SocioleeMarkingApi.Models.Database;

namespace SocioleeMarkingApi.Services
{
	public interface IEmailTrackingService
	{
		Task<bool> Unsubscribe(Guid userId, EmailTrackingType emailTrackingType);
		Task<bool> Subscribe(Guid userId, EmailTrackingType emailTrackingType);
		Task<bool> AddToEmailTracking(Guid userId);
	}
	public class EmailTrackingService : IEmailTrackingService
	{
		private readonly SocioleeDesignContext _db;
		private readonly IUniqueIds _uniqueIds;

		public EmailTrackingService(SocioleeDesignContext dbContext, IUniqueIds uniqueIds)
		{
			_db = dbContext;
			_uniqueIds = uniqueIds;
		}

		public async Task<bool> Unsubscribe(Guid userId, EmailTrackingType emailTrackingType)
		{
			var userEmailSetting = await _db.EmailTrackings.FirstOrDefaultAsync(x => x.UserId == userId && x.Type == emailTrackingType.ToString());
			if (userEmailSetting != null)
			{
				userEmailSetting.Subscribed = false;
				await _db.SaveChangesAsync();
			}
			return true;
		}

		public async Task<bool> Subscribe(Guid userId, EmailTrackingType emailTrackingType)
		{
			var userEmailSetting = await _db.EmailTrackings.FirstOrDefaultAsync(x => x.UserId == userId && x.Type == emailTrackingType.ToString());
			if (userEmailSetting != null)
			{
				userEmailSetting.Subscribed = true;
				await _db.SaveChangesAsync();
			}
			
			return true;

		}

		public async Task<bool> AddToEmailTracking(Guid userId)
		{
			var emailTracking = new EmailTracking
			{
				Id = await _uniqueIds.UniqueEmailTrackingId(),
				UserId = userId,
				Subscribed = true,
				Type = EmailTrackingType.Marketing.ToString()
			};
			_db.EmailTrackings.Add(emailTracking);

			emailTracking = new EmailTracking
			{
				Id = await _uniqueIds.UniqueEmailTrackingId(),
				UserId = userId,
				Subscribed = true,
				Type = EmailTrackingType.Message.ToString()
			};
			_db.EmailTrackings.Add(emailTracking);

			await _db.SaveChangesAsync();
			return true;
		}
	}
}

