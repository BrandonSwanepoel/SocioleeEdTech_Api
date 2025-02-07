using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Internal;
using Microsoft.EntityFrameworkCore;
using SocioleeMarkingApi.Common;
using SocioleeMarkingApi.Models.BlobModel;
using SocioleeMarkingApi.Models.Database;

namespace SocioleeMarkingApi.Services
{
	public interface IBlobStorageService
	{
		Task<string> GetBlobFile(string url);
		//Task<string> UploadBlobFile(string filePath, string filename);
		Task DeleteBlob(string path);
		Task<Uri> UploadFileBlobAsync(string blobContainerName, Stream content, string contentType, string fileName);
		Task<bool> DownloadBlob(string url);
		Task<int> GetDesignIndex(Guid requestId);

	}
	public class BlobStorageService : IBlobStorageService
	{
		private readonly BlobServiceClient _blobServiceClient;
		private BlobContainerClient _client;
		private readonly IUniqueIds _uniqueIds;
		public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPEG", ".PNG", ".MP4", ".MOV" };

		private readonly SocioleeDesignContext _db;
		public BlobStorageService(SocioleeDesignContext dbContext, IUniqueIds uniqueIds, BlobServiceClient blobServiceClient)
		{
			_db = dbContext;
			_uniqueIds = uniqueIds;
			_blobServiceClient = blobServiceClient;
			_client = _blobServiceClient.GetBlobContainerClient("designcontent");
		}

		public async Task DeleteBlob(string path)
		{
			var imageDetails = new Uri(path).Segments;
			var client = _blobServiceClient.GetBlobContainerClient(imageDetails[1]);

			var blobClient = client.GetBlobClient(imageDetails[2]);
			await blobClient.DeleteIfExistsAsync();
		}

		public async Task<int> GetDesignIndex(Guid requestId)
		{
			var index = await _db.Messages.CountAsync(x => x.RequestId == requestId && x.AssetId != null);
			return index;
		}

		public async Task<string> GetBlobFile(string url)
		{
			return null;
			//var imageDetails = new Uri(url).Segments;
			//var client = _blobServiceClient.GetBlobContainerClient(imageDetails[1]);

			//var blobClient = client.GetBlobClient(imageDetails[2]);

			//if (!blobClient.Exists())
			//{
			//	return null;
			//}

			//var sasBuilder = new BlobSasBuilder
			//{
			//	BlobContainerName = containerName,
			//	BlobName = blobName,
			//	Resource = "b",
			//	ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
			//};

			//sasBuilder.SetPermissions(BlobSasPermissions.Read);

			//var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(
			//	_configuration["BlobAccountName"],
			//	_configuration["BlobAccountKey"]
			//)).ToString();

			//var sasUrl = $"{blobClient.Uri}?{sasToken}";

			//return sasUrl;
			//var fileName= new Uri(url).Segments.LastOrDefault();

			//var blobClient = _client.GetBlobClient(fileName);



			//if (await blobClient.ExistsAsync())
			//{



			//var localFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Sociolee Design Downloads");

			//using (FileStream downloadFileStream = File.OpenWrite(localFilePath))
			//{

			//	string downloadFolderPath = GetDownloadsPath();

			//	// Ensure the Downloads folder exists
			//	if (!Directory.Exists(downloadFolderPath))
			//	{
			//		Directory.CreateDirectory(downloadFolderPath);
			//	}
			//	var blobClient = client.GetBlobClient(imageDetails[2]);

			//	// Set the download file path
			//	string downloadFilePath = Path.Combine(downloadFolderPath, imageDetails[2] + ".png");
			//	BlobDownloadInfo downloadInfo = await blobClient.DownloadToAsync(downloadFilePath);
			//	//downloadInfo.ReadTimeout = TimeSpan.FromMinutes(5); // Adjust timeout as needed

			//	//// Handle potential exceptions
			//	//while (!downloadInfo.IsComplete)
			//	//{
			//	//	await downloadInfo.WaitForCompletionAsync();
			//	//}
			//}

			//BlobDownloadResult content = await blobClient.DownloadContentAsync();
			//var downloadedData = content.Content.ToStream();

			//if (ImageExtensions.Contains(Path.GetExtension(fileName.ToUpperInvariant())))
			//{
			//	var extention = Path.GetExtension(fileName);
			//	return new BlobObject { Content = downloadedData, ContentType = "image/" + extention.Remove(0, 1) };
			//}
			//else
			//{
			//	return new BlobObject { Content = downloadedData, ContentType = content.Details.ContentType };
			//}

			//var client = _blobServiceClient.GetBlobContainerClient(imageDetails[1]);

			//var blobClient = client.GetBlobClient(imageDetails[2]);
			//if (await blobClient.ExistsAsync())
			//	return true;
			//else
			//	return false;

		}

		public async Task<bool> DownloadBlob(string url)
		{

			var fileName = new Uri(url).Segments.LastOrDefault();

			var blobClient = _client.GetBlobClient(fileName);

			if (await blobClient.ExistsAsync())
			{

				BlobDownloadResult content = await blobClient.DownloadContentAsync();
				// Download the blob to a local file
				string downloadFolderPath = GetDownloadsPath();

				// Ensure the Downloads folder exists
				if (!Directory.Exists(downloadFolderPath))
				{
					Directory.CreateDirectory(downloadFolderPath);
				}

				//string[] contentTypeParts = content.Details.ContentType.Split('/');

				// Get the file extension based on content type (improve mapping as needed)
				//string fileExtension = GetFileExtensionFromContentType(contentTypeParts[1]);

				// Set the download file path
				string downloadFilePath = Path.Combine(downloadFolderPath, fileName + ".png");
				throw new Exception(downloadFilePath);
				await blobClient.DownloadToAsync(downloadFilePath);
				return true;
			}
			return false;
		}

		private static string GetFileExtensionFromContentType(string contentType)
		{
			// Create a mapping between content types and file extensions
			var contentTypeToExtensionMap = new Dictionary<string, string>
	{
		{ "image/jpeg", ".jpg" },
		{ "image/png", ".png" },
        // Add more mappings as needed
    };

			if (contentTypeToExtensionMap.TryGetValue(contentType, out string extension))
			{
				return extension;
			}

			return string.Empty; // Or handle unknown content types
		}

		private static string GetDownloadsPath()
		{

			//string path = string.Empty;

			//if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			//{
			//	path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Sociolee Design Downloads");
			//}
			//else if (Environment.OSVersion.Platform == PlatformID.Unix)
			//{
				var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
				//path = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Downloads");
			//}
			//else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
			//{
			//	path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Sociolee Design Downloads");
			//}

			return path;
		}
		//public async Task<string> UploadBlobFile(string filePath, string filename)
		//{
		//	var imageDetails = new Uri(path).Segments;
		//	var client = _blobServiceClient.GetBlobContainerClient(imageDetails[1]);
		//	var blobClient = client.GetBlobClient(filename);
		//	var status = await blobClient.UploadAsync(filePath);
		//	return blobClient.Uri.AbsoluteUri;
		//}

		public async Task<Uri> UploadFileBlobAsync(string blobContainerName, Stream content, string contentType, string fileName)
		{
			var containerClient = GetContainerClient(blobContainerName);
			var blobClient = containerClient.GetBlobClient(fileName);
			await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });
			return blobClient.Uri;
		}

		private BlobContainerClient GetContainerClient(string blobContainerName)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
			containerClient.CreateIfNotExists(PublicAccessType.Blob);
			return containerClient;
		}
	}
}

