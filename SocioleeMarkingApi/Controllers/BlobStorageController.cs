using Microsoft.AspNetCore.Mvc;
using SocioleeMarkingApi.Common;
using SocioleeMarkingApi.Models.Database;
using SocioleeMarkingApi.Services;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using SocioleeMarkingApi.Models;

namespace SocioleeMarkingApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BlobStorageController : Controller
	{

		private readonly IBlobStorageService _BlobStorageService;
		private readonly IContentService _ContentService;
		private readonly IUserService _userService;
		private readonly IUniqueIds _uniqueIds;
		private readonly IConfiguration _configuration;
		//private readonly ILogger<UsersController> _logger;

		public BlobStorageController(IBlobStorageService authService, IContentService contentService, IUserService userService, IUniqueIds uniqueIds, IConfiguration configuration)
		{
			_BlobStorageService = authService;
			_ContentService = contentService;
			_userService = userService;
			_uniqueIds = uniqueIds;
		}
		// GET: /<controller>/
		[HttpGet("GetBlobFile")]
		public async Task<IActionResult> GetBlobFile(string url)
		{
			var exists = await _BlobStorageService.GetBlobFile(url);
			return Ok(exists);
		}

		[HttpGet("generate-sas-token")]
		public async Task<IActionResult> GenerateSasToken(string url)
		{
			var sasUrl = await _BlobStorageService.GetBlobFile(url);
			return Ok(new { sasUrl });

		}

		[HttpGet("downloadBlob")]
		public async Task<IActionResult> DownloadBlob([FromQuery] string url)
		{
			var exists = await _BlobStorageService.DownloadBlob(url);
			return Ok(exists);
		}

		[HttpPost("UploadBlobFile/{container}")]
		public async Task<IActionResult> UploadBlobFile(string container)
		{
			//var result = await _BlobStorageService.UploadBlobFile(model.FilePath, model.FileName);
			//return Ok(result);

			IFormFile file = Request.Form.Files[0];
			if (file == null)
			{
				return BadRequest();
			}

			var result = await _BlobStorageService.UploadFileBlobAsync(
					container,
					file.OpenReadStream(),
					file.ContentType,
					file.Name);

			var toReturn = result.AbsoluteUri;

			return Ok(new { path = toReturn });
		}

		[HttpPost("UploadMessageAsset")]
		public async Task<IActionResult> UploadMessageAsset([FromQuery]Guid requestId, [FromQuery] Guid messageId, [FromQuery]string container, [FromQuery] bool reviewDesign)
		{
			//var result = await _BlobStorageService.UploadBlobFile(model.FilePath, model.FileName);
			//return Ok(result);

			IFormFile file = Request.Form.Files[0];

			if (file == null)
			{
				return BadRequest();
			}

			var index = await _ContentService.GetDesignIndex(requestId);
			var name = $"{requestId}_design_v{index}";

			var result = await _BlobStorageService.UploadFileBlobAsync(
					container,
					file.OpenReadStream(),
					file.ContentType,
					name);

			var uploadMessageAsset = new UploadMessageAsset {
				MessageId = messageId,
				ContentPath = name,
				ReviewDesign = reviewDesign
			};

			var messageAsset = await _ContentService.UploadMessageAsset(uploadMessageAsset);

			return Ok(messageAsset);
		}

		//[HttpPost("UploadSong/{container}")]
		//public async Task<IActionResult> UploadSong(string container)
		//{
		//	IFormFile file = Request.Form.Files[0];
		//	if (file == null)
		//	{
		//		return BadRequest();
		//	}

		//	var userId = _userService.GetUserId();
		//	if (userId == Guid.Empty)
		//		throw new HttpRequestException("Please sign in again.");

		//	//await _userService.RemoveSong(userId, "song");

		//	var assetId = await _uniqueIds.UniqueAssetId();

		//	var result = await _BlobStorageService.UploadFileBlobAsync(
		//			container,
		//			file.OpenReadStream(),
		//			file.ContentType,
		//			assetId.ToString());

		//	var toReturn = result.AbsoluteUri;

		//	await _userService.AddUserAsset(userId, assetId, "song");

		//	return Ok(new { path = toReturn });
		//}

		//[HttpPost("uploadUserAsset/{container}")]
		//public async Task<IActionResult> UploadProfilePicture([FromRoute] string container, [FromQuery] string type)
		//{
		//	IFormFile file = Request.Form.Files[0];
		//	if (file == null)
		//	{
		//		return BadRequest();
		//	}

		//	var userId = _userService.GetUserId();
		//	if (userId == Guid.Empty)
		//		throw new HttpRequestException("Please sign in again.");

		//	await _userService.RemoveUserAsset(userId, type);

		//	var assetId = await _uniqueIds.UniqueAssetId();

		//	var result = await _BlobStorageService.UploadFileBlobAsync(
		//			container,
		//			file.OpenReadStream(),
		//			file.ContentType,
		//			assetId.ToString());

		//	var toReturn = result.AbsoluteUri;

		//	await _userService.AddUserAsset(userId, assetId, type);

		//	return Ok(new { path = toReturn });
		//}

		[HttpDelete("DeleteBlob")]
		public IActionResult DeleteBlob(string path)
		{
			_BlobStorageService.DeleteBlob(path);
			return Ok();
		}

		//[HttpDelete("deleteReviewAsset")]
		//public async Task<IActionResult> DeleteReviewAssetAsync(Guid reviewAssetId, string path)
		//{
		//	await _BlobStorageService.DeleteBlob(path);
		//	var removed = await _ReviewService.DeleteReviewImage(reviewAssetId);
		//	return Ok(removed);
		//}

	}
}

