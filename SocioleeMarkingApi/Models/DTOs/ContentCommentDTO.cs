namespace SocioleeMarkingApi.Models
{
	public class ContentCommentDto
	{
		public Guid ContentId { get; set; }
		public Guid? CommentReplyId { get; set; }
		public Guid UserId { get; set; }
		public DateTime Created { get; set; }
		public string Text { get; set; } = string.Empty;
	}
}