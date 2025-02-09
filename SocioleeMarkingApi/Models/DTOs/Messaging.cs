using SocioleeMarkingApi.Models.Database;

namespace SocioleeMarkingApi.Models
{
	public class MessageIn
	{
		public Guid UserId { get; set; }
		public Guid RequestId { get; set; }
		public string TextMessage { get; set; } = string.Empty;
	}

	public class MessageOut
	{
		public MessageOut(Message message)
		{
			Id = message.Id;
			Created = message.Created;
			UserId = message.UserId;
			TextMessage = message.TextMessage;
			//Asset = message.Asset;
			Deleted = message.Deleted ?? false;
			//ReviewDesign = message.Asset?.ReviewDesign ?? false;
			//Reviewed = message.Asset?.Reviewed ?? false;
		}

		public Guid Id { get; set; }
		public DateTime Created { get; set; }
		public Guid UserId { get; set; }
		public string? TextMessage { get; set; } = string.Empty;
		public MessageAsset? Asset { get; set; }
		public bool Deleted { get; set; }
		public bool ReviewDesign { get; set; }
		public bool Reviewed { get; set; }
	}

	public class UploadMessageAsset
	{
		public Guid MessageId { get; set; }
		public string ContentPath { get; set; } = null!;
		public bool ReviewDesign { get; set; }
	}
}
