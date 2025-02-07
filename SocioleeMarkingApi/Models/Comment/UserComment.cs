
//namespace SocioleeMarkingApi.Models.Database;

//public class UserComment
//{
//    public Guid Id { get; set; }

//    public Guid? CommentReplyId { get; set; }

//    public Guid ContentId { get; set; }

//    public DateTime Created { get; set; }

//    public string Text { get; set; } = null!;

//    public Guid UserId { get; set; }

//	public string FullName { get; set; } = null!;

//	public int Likes { get; set; }

//	public Guid? ProfilePictureId { get; set; }


//	public UserComment(Comment comment, string fullName, int likes, IEnumerable<Asset> asset)
//    {
//        Id = comment.Id;
//        CommentReplyId = comment.CommentReplyId;
//        ContentId = comment.ContentId;
//        Created = comment.Created;
//        Text = comment.Text;
//        UserId = comment.UserId;
//		FullName = fullName;
//		Likes = likes;
//		ProfilePictureId = asset.FirstOrDefault(x => x.Type == "profilePicture")?.Id;
//	}

//}
