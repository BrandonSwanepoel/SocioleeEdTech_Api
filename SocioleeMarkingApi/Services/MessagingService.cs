using Microsoft.EntityFrameworkCore;
using SocioleeMarkingApi.Common;
using SocioleeMarkingApi.Models.Database;

namespace SocioleeMarkingApi.Services
{
	public interface IMessagingService
	{
		Task<IEnumerable<MessageOut>> GetMessages(Guid requestId);
		Task<MessageOut> SendMessage(MessageIn message);
		Task<bool> DeleteMessage(Guid messageId);
	}
	public class MessagingService : IMessagingService
	{
		private readonly SocioleeDesignContext _db;
		private readonly IUniqueIds _uniqueIds;

		public MessagingService(SocioleeDesignContext dbContext, IUniqueIds uniqueIds)
		{
			_db = dbContext;
			_uniqueIds = uniqueIds;
		}


		public async Task<IEnumerable<MessageOut>> GetMessages(Guid requestId)
		{
			var messages = await _db.Messages
				.Include(x => x.MessageAssets)
				.Where(x => x.RequestId == requestId)
				.OrderBy(x => x.Created)
				.Select(x => new MessageOut(x))
				.ToListAsync();
			return messages;
		}

		public async Task<MessageOut> SendMessage(MessageIn message)
		{
			var newMessage = new Message
			{
				Id = await _uniqueIds.UniqueMessageId(),
				UserId = message.UserId,
				RequestId = message.RequestId,
				TextMessage = message.TextMessage,
				Created = CommonFunctions.CurrentDateTime(),
			};

			_db.Messages.Add(newMessage);
			await _db.SaveChangesAsync();

			return new MessageOut(newMessage);
		}

		public async Task<bool> DeleteMessage(Guid messageId)
		{
			var message = await _db.Messages
						.Include(x => x.MessageAssets)
						.Where(x => x.Id == messageId).FirstOrDefaultAsync() ?? throw new Exception("Error deleting message.");

			message.Deleted = true;
			message.TextMessage = "Message is deleted";
			_db.MessageAssets.RemoveRange(message.MessageAssets);
			await _db.SaveChangesAsync();
			return true;
		}
	}
}

