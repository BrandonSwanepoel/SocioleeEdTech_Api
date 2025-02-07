//using Microsoft.AspNetCore.Mvc;
//using SocioleeMarkingApi.Models.Database;
//using SocioleeMarkingApi.Services;

//namespace SocioleeMarkingApi.Controllers
//{
//	[Route("api/[controller]")]
//	[ApiController]
//	public class CommentController : Controller
//	{

//		private readonly ICommentService _CommentService;
//		private readonly IUserService _userService;
//		//private readonly ILogger<UsersController> _logger;

//		public CommentController(ICommentService commentService,IUserService userService)
//		{
//			_CommentService = commentService;
//			_userService = userService;
//		}

//		[HttpPost("create")]
//		[ProducesResponseType(typeof(Task<UserComment>), 200)]
//		public async Task<IActionResult> Create([FromBody] ContentCommentDto request)
//		{
//			var userComment = await _CommentService.Create(request);
//			return Ok(userComment);
//		}

//		[HttpDelete("{commentId}")]
//		[ProducesResponseType(typeof(Task<bool>), 200)]
//		public async Task<IActionResult> DeletePost([FromRoute] Guid commentId)
//		{
//			var deleted = await _CommentService.Delete(commentId);
//			return Ok(deleted);
//		}


//		[HttpGet("{postId}/count")]
//		[ProducesResponseType(typeof(Task<int>), 200)]
//		public async Task<IActionResult> GetCommentCount([FromRoute] Guid postId)
//		{
//			var commentCount = await _CommentService.GetCommentCount(postId);
//			return Ok(commentCount);
//		}

//		[HttpGet("{contentId}")]
//		[ProducesResponseType(typeof(Task<IEnumerable<UserComment>>), 200)]
//		public async Task<IActionResult> GetComments([FromRoute] Guid contentId)
//		{
//			var userId = _userService.GetUserId();
//			if (userId == Guid.Empty)
//				throw new HttpRequestException("Please sign in again.");
//			var contetComments = await _CommentService.GetComments(userId, contentId);
//			return Ok(contetComments);
//		}

//		[HttpPost("like/{commentId}")]
//		[ProducesResponseType(typeof(Task<bool>), 200)]
//		public async Task<IActionResult> LikeComment([FromRoute] Guid commentId)
//		{
//			//var sent = await _ReviewService.SendBusinessEmail();
//			//return Ok(sent);
//			var userId = _userService.GetUserId();
//			if (userId == Guid.Empty)
//				throw new HttpRequestException("Please sign in again.");
//			var commentLiked = await _CommentService.LikeComment(commentId, userId);
//			return Ok(commentLiked);
//		}

//		[HttpDelete("unlike/{commentId}")]
//		[ProducesResponseType(typeof(Task<bool>), 200)]
//		public async Task<IActionResult> UnlikeComment([FromRoute] Guid commentId)
//		{
//			var userId = _userService.GetUserId();
//			if (userId == Guid.Empty)
//				throw new HttpRequestException("Please sign in again.");
//			var commentLiked = await _CommentService.UnlikeComment(commentId, userId);
//			return Ok(commentLiked);
//		}

//		[HttpGet("likes/{commentId}")]
//		[ProducesResponseType(typeof(Task<List<LikedComment>>), 200)]
//		public async Task<IActionResult> GetCommentLikes([FromRoute] Guid commentId)
//		{
//			var commentLikes = await _CommentService.GetCommentLikes(commentId);
//			return Ok(commentLikes);
//		}
//	}
//}

