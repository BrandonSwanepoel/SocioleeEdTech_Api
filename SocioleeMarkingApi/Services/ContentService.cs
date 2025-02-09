using System.Linq;
using Azure.Storage.Blobs;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using RazorHtmlEmails.Common;
using SocioleeMarkingApi.Common;
using SocioleeMarkingApi.Models;
using SocioleeMarkingApi.Models.Database;

namespace SocioleeMarkingApi.Services
{
	public interface IContentService
	{
		Task<ContentForm> Create(ContentFromDto request);
		Task<List<AssetAspectRatio>> GetAspectRatio();
		Task<ContentForm> Update(ContentFromDto request);
		Task<List<Step>> GetStepTypes();
		Task<IEnumerable<Models.StudentDTO>> GetStudents(Guid userId);
		Task<List<DesignRequest>> GetClientDesignRequests();
		Task<ContentForm> GetContentForm(Guid contentFormId);
		Task<IEnumerable<UserActiveStep>> GetUserDesignStep(Guid userId);
		Task<UserActiveStep> GetRequestDesignStep(Guid requestId);
		Task<UserActiveStep?> GetFormDesignStep(Guid formId);
		Task<AssetAspectRatio> GetFormAspectRatio(Guid formId);
		Task<bool> UploadContent(UploadContentDesign uploadContentDesign);
		//Task<UserActiveStep> UpdateStep(UpdateFormStep updateFormStep);
		//Task<IEnumerable<Design>> DesignByFromId(Guid formId);
		Task<bool> AddContentNotes(DesignChangeRequestsDTO designChangeRequestsDTO);
		Task<bool> CompletedDesignRequest(Guid requestId);
		Task<bool> NoContentNotes(DesignChangeRequestsDTO designChangeRequestsDTO);
		Task<IEnumerable<DesignChange>> GetContentNote(Guid formId);
		Task<DesignReview> GetContentDesignReview(Guid messageId);
		Task<bool> ContentDownloaded(Guid formId);
		//Task<List<UserContent>> GetUserContent(Guid userId);
		Task<int> GetDesignIndex(Guid requestId);
		//Task<MessageAsset?> UploadMessageAsset(UploadMessageAsset uploadContentDesign);

		//Projects
		Task<bool> UpsertProject(UpsertStudentProjectDTO project, bool editProject);
		Task<bool> DeleteProject(Guid projectId);
		Task<List<StudentProjectDetailsDTO>> GetProjects(Guid? userId, bool studentProjects);
		Task<List<StudentProjectDetails>> GetProjectStudents(Guid projectId);
		Task<StudentProjectDTO> GetProjectDetails(Guid projectId);
		Task<bool> AddStudentToProject(StudentProjectAssignmentDTO studentProjectAssignmentDTO);
		Task<bool> RemoveStudentToProject(StudentProjectAssignmentDTO studentProjectAssignmentDTO);
		Task<List<ProjectRubricDTO>> GetRubricForProject(Guid projectId);
		Task<bool> SaveRubricForProject(List<ProjectRubricDTO> projectRubricDTO);
		Task<List<ProjectRubricOfStudentDTO>> GetRubricForProjectOfStudent(Guid projectId, Guid studentId);
		Task<bool> SaveRubricForProjectOfStudent(List<ProjectRubricOfStudentMarkedDTO> projectRubricOfStudentMarkedDTO);
		Task<List<StudentProjectDetailsDTO>> GetStudentProjects(Guid studentId);
		Task<IEnumerable<ProjectStudent>> GetProjectStudentsToAdd(StudentProjectDTO projectDetails);
	}

	public class ContentService : IContentService
	{
		private readonly BlobServiceClient _blobServiceClient;
		private readonly IUniqueIds _uniqueIds;
		private readonly IRegisterAccountService _registerAccountService;

		private readonly SocioleeDesignContext _db;
		public ContentService(SocioleeDesignContext dbContext, IUniqueIds uniqueIds, BlobServiceClient blobServiceClient, IRegisterAccountService registerAccountService)
		{
			_db = dbContext;
			_uniqueIds = uniqueIds;
			_blobServiceClient = blobServiceClient;
			_registerAccountService = registerAccountService;
		}

		public async Task<ContentForm> Create(ContentFromDto request)
		{
			var contentFormId = await _uniqueIds.UniqueContentAssetId();

			var user = await _db.Users
						.Where(x => x.Id == request.UserId)
						.FirstOrDefaultAsync() ?? throw new Exception("User does not exist.");

			if(request.DesignRedeemed == true)
			{
				var coupon = await _db.Coupons.Where(x => x.UserId == request.UserId && x.Redeemed == false).FirstOrDefaultAsync() ?? throw new Exception("You don't have any coupons."); ;
				coupon.Redeemed = true;
			}

			var contentForm = new ContentForm
			{
				Id = contentFormId,
				UserId = request.UserId,
				ContentType = request.ContentType,
				Title = request.Title,
				ContentHeadline = request.ContentHeadline,
				ContentBodyText = request.ContentBodyText,
				ContentDescription = request.ContentDescription,
				IncludeContactDetails = request.IncludeContactDetails,
				AssetAspectRatioId = request.AssetAspectRatioId,
				Email = request.Email,
				PhoneNumber = request.PhoneNumber,
				Created = CommonFunctions.CurrentDateTime(),
			};

			if (request.IncludedLogo)
			{
				contentForm.LogoId = await _uniqueIds.UniqueContentAssetId();
			}

			if (request.IncludedBackground)
			{
				contentForm.BackgroundImageId = await _uniqueIds.UniqueContentAssetId();
			}

			if (request.IncludedVoiceNote)
			{
				contentForm.VoiceNoteId = await _uniqueIds.UniqueContentAssetId();
			}

			_db.ContentForms.Add(contentForm);

			if (request.IncludedExamplePhotos)
			{
				for (int i = 0; i < request.ExamplePhotoCount; i++)
				{
					var contentFormExamplePhoto = new ContentFormExamplePhoto
					{
						Id = await _uniqueIds.UniqueContentFormExamplePhotoId(),
						ContentFormId = contentFormId,
						ExamplePhotoId = await _uniqueIds.UniqueContentAssetId()
					};
					_db.ContentFormExamplePhotos.Add(contentFormExamplePhoto);
				}
			}

			foreach (var primaryColor in request.PrimaryColors)
			{
				var contentFormPrimaryColor = new ContentFormPrimaryColor
				{
					Id = await _uniqueIds.UniqueContentFormPrimaryColorId(),
					ContentFormId = contentFormId,
					PrimaryColor = primaryColor
				};

				_db.ContentFormPrimaryColors.Add(contentFormPrimaryColor);
			}


			await _db.SaveChangesAsync();

			await AddContentStep(contentFormId, 0);

			var notificationText = $"{user.FullName} has requested a new design. Please review the form and begin working on the design.";
			await SendEmailToDesigners(notificationText, contentFormId);

			return contentForm;
		}

		public async Task<List<AssetAspectRatio>> GetAspectRatio()
		{
			var aspectRatios = await _db.AssetAspectRatios.ToListAsync();
			return aspectRatios;
		}

		public async Task<List<Step>> GetStepTypes()
		{
			return await _db.StepTypes.OrderBy(x => x.Index).Select(x => new Step(x.Index, x.Name)).ToListAsync();
		}

		public async Task<IEnumerable<Models.StudentDTO>> GetStudents(Guid userId)
		{
			var students = await _db.Students
				.Include(u => u.StudentCourses)
						.ThenInclude(sc => sc.InstitutionCourses)
				.Include(x => x.User)
				.Select(s => new Models.StudentDTO(
					s.User,
					s,
					s.StudentCourses.Select(sc => new Course
					{
						Id = sc.InstitutionCourses.Id,
						Name = sc.InstitutionCourses.Course
					}).ToList()
				))
				.ToListAsync();

			return students;
		}

		public async Task<List<DesignRequest>> GetClientDesignRequests()
		{
			var designRequests = await _db.ContentSteps
				.Include(x => x.StepType)
				.Include(x => x.ContentForm)
					.ThenInclude(x => x.User)
				.OrderBy(x => x.ContentForm.Created)
				.Select(step => new DesignRequest(step.ContentForm, step.ContentForm.User, step.Downloaded, step.StepType))
				.ToListAsync();

			return designRequests;
		}

		public async Task<ContentForm> GetContentForm(Guid contentFormId)
		{
			var existingContentForm = await _db.ContentForms
				.Include(x => x.AssetAspectRatio)
				.Include(x => x.ContentFormPrimaryColors)
				.Include(x => x.ContentFormExamplePhotos)
				.Where(x => x.Id == contentFormId)
				.FirstOrDefaultAsync();

			return existingContentForm ?? new ContentForm();
		}

		public async Task<IEnumerable<UserActiveStep>> GetUserDesignStep(Guid userId)
		{
			var userActiveSteps = await _db.ContentSteps
				.Include(x => x.ContentForm)
				.Include(x => x.StepType)
				.OrderBy(x => x.ContentForm.Created)
				.Where(x => x.ContentForm.UserId == userId && x.Downloaded == false)
				.Select(userActiveStep => new UserActiveStep(userActiveStep.ContentForm.Title, userActiveStep.ContentFormId, userActiveStep.StepType.Index, userActiveStep.StepType.Name))
				.ToListAsync();

			return userActiveSteps;
		}

		public async Task<UserActiveStep> GetRequestDesignStep(Guid requestId)
		{
			var userActiveStep = await _db.ContentSteps
				.Include(x => x.ContentForm)
				.Include(x => x.StepType)
				.OrderBy(x => x.ContentForm.Created)
				.Where(x => x.ContentForm.Id == requestId)
				.Select(userActiveStep => new UserActiveStep(userActiveStep.ContentForm.Title, userActiveStep.ContentFormId, userActiveStep.StepType.Index, userActiveStep.StepType.Name))
				.FirstOrDefaultAsync() ?? throw new Exception("Can't find request.");

			return userActiveStep;
		}

		public async Task<UserActiveStep?> GetFormDesignStep(Guid formId)
		{
			var userActiveStep = await _db.ContentSteps
				.Include(x => x.ContentForm)
				.Include(x => x.StepType)
				.Where(x => x.ContentForm.Id ==	formId)
				.OrderByDescending(x => x.StepType.Index)
				.FirstOrDefaultAsync();

			if (userActiveStep != null)
			{
				return new UserActiveStep(userActiveStep.ContentForm.Title, userActiveStep.ContentFormId, userActiveStep.StepType.Index, userActiveStep.StepType.Name);
			}

			return null;
		}

		//public async Task<MessageAsset?> UploadMessageAsset(UploadMessageAsset messageAsset)
		//{
			//var existingMessage = await _db.Messages
			//	.Include(x => x.User)
			//	.Where(x => x.Id == messageAsset.MessageId)
			//	.FirstOrDefaultAsync();

			//if (existingMessage != null)
			//{
			//	var contentAssetId = await _uniqueIds.UniqueContentAssetId();
			//	var newMessageAsset = new MessageAsset
			//	{
			//		Id = contentAssetId,
			//		MessageId = messageAsset.MessageId,
			//		ContentPath = messageAsset.ContentPath,
			//		Type = "image",
			//		Downloaded = false,
			//		ReviewDesign = messageAsset.ReviewDesign
			//	};

			//	if (messageAsset.ReviewDesign)
			//	{
			//		//So that there is always only one design to review.
			//		var reviewDesigns = await _db.Messages
			//						.Include(x => x.Asset)
			//						.Where(x =>
			//									x.Asset != null &&
			//									x.Asset.ReviewDesign == true &&
			//									x.RequestId == existingMessage.RequestId
			//						)
			//						.Select(x => x.Asset)
			//						.ToListAsync();

			//		if (reviewDesigns != null)
			//		{
			//			foreach (var reviewDesign in reviewDesigns)
			//			{
			//				if (reviewDesign?.Reviewed != true)
			//				{
			//					reviewDesign.Reviewed = true;
			//				}

			//			}
			//		}
			//	}

			//	await _db.MessageAssets.AddAsync(newMessageAsset);

			//	existingMessage.AssetId = newMessageAsset.Id;
			//	await _db.SaveChangesAsync();

			//	if (messageAsset.ReviewDesign)
			//	{
			//		await UpdateDesignStep(existingMessage.RequestId, 2);

			//		var stepText = UpdatedStepText(1);
			//		if (stepText != null)
			//		{
			//			var assetUrl = "https://SocioleeMarking.com/content-list/content/" + existingMessage.RequestId;

			//			await _registerAccountService.StepUpdated(existingMessage.User.FullName, existingMessage.User.Email, stepText, assetUrl);
			//		}
			//	}

			//	return newMessageAsset;

			//}
			//return null;
		//}

		public async Task<bool> UploadContent(UploadContentDesign uploadContentDesign)
		{
			//var existingContentForm = await _db.ContentForms
			//	.Include(x => x.User)
			//	.Where(x => x.Id == uploadContentDesign.FormId)
			//	.FirstOrDefaultAsync();

			//if (existingContentForm != null)
			//{
			//	var contentAssetId = await _uniqueIds.UniqueContentAssetId();
			//	var contentAsset = new ContentAsset
			//	{
			//		Id = contentAssetId,
			//		UserId = existingContentForm.UserId,
			//		ContentFormId = uploadContentDesign.FormId,
			//		ContentPath = uploadContentDesign.ContentPath,
			//		Created = CommonFunctions.CurrentDateTime(),
			//		AssetAspectRatioId = uploadContentDesign.AssetAspectRatioId,
			//		Type = "image",
			//	};

			//	await _db.ContentAssets.AddAsync(contentAsset);
			//	await _db.SaveChangesAsync();

			//	await AddContentStep(uploadContentDesign.FormId, uploadContentDesign.StepIndex);

			//	var stepText = UpdatedStepText(uploadContentDesign.StepIndex);
			//	if (stepText != null)
			//	{
			//		var assetUrl = "https://SocioleeMarking.com/content-list/content/" + uploadContentDesign.FormId;

			//		await _registerAccountService.StepUpdated(existingContentForm.User.FullName, existingContentForm.User.Email, stepText, assetUrl);
			//	}
			//	return true;
			//}
			return false;
		}

		private static string? UpdatedStepText(int stepIndex)
		{
			if(stepIndex == 1)
			{
				return "Your design is ready for review. Click the button to view it on our website, where you can request any changes or confirm that no changes are needed. If you choose not to make any changes, your design will be available for download so you can start using it on your social media.";
			}else if (stepIndex == 3)
			{
				return "Your final design is ready and available for download, so you can start using it on your social media.";
			}
			return null;
		}

		//public async Task<IEnumerable<Design>> DesignByFromId(Guid formId)
		//{
		//	var message = await _db.Messages
		//					.Include(x => x.MessageAssets)
		//						.ThenInclude(x => x.ContentNotes)
		//					.Include(x => x.Request)
		//						.ThenInclude(x => x.AssetAspectRatio)
		//					.Where(x => x.RequestId == formId)
		//					.OrderBy(x => x.Created)
		//					.ToListAsync();

		//	return message.Select(x => new Design(x.MessageAssets, x.Request.AssetAspectRatio.AspectRatio));
		//}

		public async Task<AssetAspectRatio> GetFormAspectRatio(Guid formId)
		{
			var assetAspectRatio = await _db.ContentForms
							.Include(x => x.AssetAspectRatio)
							.Where(x => x.Id == formId)
							.Select(x => x.AssetAspectRatio)
							.FirstOrDefaultAsync() ?? throw new Exception("Aspect ratio does not exist");

			return assetAspectRatio;
		}

		public async Task<bool> AddContentNotes(DesignChangeRequestsDTO designChangeRequestsDTO)
		{
			//if (designChangeRequestsDTO.HasChanges)
			//{
			//	var existingMessage = await _db.Messages
			//		.Include(x => x.Asset)
			//		.Include(x => x.Request)
			//			.ThenInclude(x => x.User)
			//		.Where(x => x.AssetId == designChangeRequestsDTO.MessageAssetId)
			//		.FirstOrDefaultAsync();

			//	if (existingMessage?.Asset == null) {
			//		throw new Exception("Error: Saving change requests, try again.");
			//	}

			//	foreach (var change in designChangeRequestsDTO.DesignChanges)
			//	{
			//		var contentNoteId = await _uniqueIds.UniqueDesignChangeRequestId();
			//		var designChangeRequest = new DesignChangeRequest
			//		{
			//			Id = contentNoteId,
			//			UserId = existingMessage.Request.UserId,
			//			ContentFormId = designChangeRequestsDTO.RequestId,
			//			ContentAssetId = designChangeRequestsDTO.MessageAssetId,
			//			X = change.X,
			//			Y = change.Y,
			//			Number = change.Number,
			//			Text = change.Text,
			//			Created = CommonFunctions.CurrentDateTime(),
			//		};
			//		await _db.DesignChangeRequests.AddAsync(designChangeRequest);
			//		await _db.SaveChangesAsync();
			//	}

			//	existingMessage.Asset.Reviewed = true;
			//	await _db.SaveChangesAsync();

			//	var notificationText = $"{existingMessage.Request.User.FullName} has reviewed your design. Please make the requested updates.";

			//	await SendEmailToDesigners(notificationText, existingMessage.RequestId);
			//}

			////await AddContentStep(designChangeRequestsDTO.RequestId, designChangeRequestsDTO.StepIndex);
			//await UpdateDesignStep(designChangeRequestsDTO.RequestId, 1);


			return true;
		}

		private async Task SendEmailToDesigners(string text, Guid designId)
		{
			var designers = await _db.Users
						.Where(x => x.RoleId == new Guid("b06ef99b-d72c-424a-9276-897da6be3c73"))
						.ToListAsync();

			foreach (var designer in designers)
			{
				await _registerAccountService.DesignerNotification(designer.FullName, designer.Email, text, designId);
			}
		}

		public async Task<bool> CompletedDesignRequest(Guid requestId)
		{
			var contentStep = await _db.ContentSteps.Where(x => x.ContentFormId == requestId).FirstOrDefaultAsync();

			if(contentStep != null)
			{
				var completedStep = await _db.StepTypes.Where(x => x.Index == 4).FirstOrDefaultAsync();
				if(completedStep != null)
				{
					contentStep.StepTypeId = completedStep.Id;
				}
				contentStep.Completed = true;
				await _db.SaveChangesAsync();
			}
			return true;
		}

		public async Task<bool> NoContentNotes(DesignChangeRequestsDTO contentNotesDTO)
		{
			//var contentStep = await _db.ContentSteps.Where(x => x.ContentFormId == contentNotesDTO.RequestId).FirstOrDefaultAsync();

			//if (contentStep != null)
			//{
			//	var existingMessage = await _db.Messages
			//		.Include(x => x.Asset)
			//		.Include(x => x.Request)
			//			.ThenInclude(x => x.User)
			//		.Where(x => x.AssetId == contentNotesDTO.MessageAssetId)
			//		.FirstOrDefaultAsync();

			//	if (existingMessage?.Asset == null)
			//	{
			//		throw new Exception("Error: Completing request, try again.");
			//	}

			//	existingMessage.Asset.Reviewed = true;

			//	var completedStep = await _db.StepTypes.Where(x => x.Index == 4).FirstOrDefaultAsync();
			//	if (completedStep != null)
			//	{
			//		contentStep.StepTypeId = completedStep.Id;
			//	}
			//	contentStep.Completed = true;
			//	await _db.SaveChangesAsync();
			//}
			return true;
		}

		public async Task<IEnumerable<DesignChange>> GetContentNote(Guid formId)
		{
			var designChanges = await _db.DesignChangeRequests
						.Where(x => x.ContentFormId == formId)
						.Select(x => new DesignChange(x.X, x.Y, x.Number, x.Text))
						.ToListAsync();

			return designChanges;
		}

		public async Task<bool> ContentDownloaded(Guid formId)
		{
			var contentStep = await _db.ContentSteps
						.Where(x => x.ContentFormId == formId)
						.FirstOrDefaultAsync() ?? throw new Exception("Contentstep does not exist.Contact admin.");

			if (contentStep.Downloaded != true)
			{
				contentStep.Downloaded = true;
				await _db.SaveChangesAsync();
			}

			return true;
		}

		//public async Task<List<UserContent>> GetUserContent(Guid userId)
		//{
		//	//var userContents = await _db.ContentSteps
		//	//					.Include(x => x.StepType)
		//	//					.Include(x => x.ContentForm)
		//	//						.ThenInclude(x => x.Messages)
		//	//							.ThenInclude(x => x.Asset)
		//	//					.Where(x => x.ContentForm.UserId == userId && x.StepType.Index == 4)
		//	//					.Select(x => new
		//	//					{
		//	//						LatestMessageAsset = x.ContentForm.Messages
		//	//							.Where(x => x.AssetId != null)
		//	//							.OrderByDescending(ca => ca.Created)
		//	//							.FirstOrDefault()
		//	//					})
		//	//					.Select(x => new UserContent(x.LatestMessageAsset))
		//	//					.ToListAsync();

		//	//return userContents;

		//}

		public async Task<DesignReview> GetContentDesignReview(Guid messageId)
		{
			var messageAsset = await _db.MessageAssets
								.Include(x => x.DesignChangeRequests)
								.Where(x => x.MessageId == messageId && x.ReviewDesign == true)
								.FirstOrDefaultAsync() ?? throw new Exception("Error: retrieving Design");

			return new DesignReview
			{
				MessageAssetId = messageAsset.Id,
				MessageAssetPath = messageAsset.ContentPath,
				DesignChanges = messageAsset.DesignChangeRequests.Select(x => new DesignChange(x.X, x.Y, x.Number, x.Text)).OrderBy(x => x.Number),
			};
		}

		public async Task<ContentForm> Update(ContentFromDto request)
		{
			var existingContentForm = await _db.ContentForms
				.Include(x => x.ContentFormPrimaryColors)
				.Include(x => x.ContentFormExamplePhotos)
				.Where(x => x.Id == request.Id)
				.FirstOrDefaultAsync();


			if (existingContentForm == null)
			{
				return new ContentForm();
			}

			existingContentForm.ContentType = request.ContentType;
			existingContentForm.ContentHeadline = request.ContentHeadline;
			existingContentForm.ContentBodyText = request.ContentBodyText;
			existingContentForm.ContentDescription = request.ContentDescription;
			existingContentForm.IncludeContactDetails = request.IncludeContactDetails;
			existingContentForm.Email = request.Email;
			existingContentForm.PhoneNumber = request.PhoneNumber;
			existingContentForm.Created = CommonFunctions.CurrentDateTime();

			if (request.IncludedLogo && existingContentForm.LogoId == null)
			{
				existingContentForm.LogoId = await _uniqueIds.UniqueContentAssetId();
			}
			else if(request.IncludedLogo == false && existingContentForm.LogoId != null)
			{
				existingContentForm.LogoId = null;
				//Delete logo from blobstorage
			}

			if (request.IncludedVoiceNote && existingContentForm.VoiceNoteId == null)
			{
				existingContentForm.VoiceNoteId = await _uniqueIds.UniqueContentAssetId();
			}
			else if (request.IncludedVoiceNote == false && existingContentForm.VoiceNoteId != null)
			{
				existingContentForm.VoiceNoteId = null;
				//Delete voicenote from blobstorage
			}

			await UpdatePrimaryColors(request, existingContentForm);
			await UpdateExamplePhotos(request, existingContentForm);

			await _db.SaveChangesAsync();

			return existingContentForm;
		}

		public async Task<int> GetDesignIndex(Guid requestId)
		{
			var index = await _db.Messages.CountAsync(x => x.RequestId == requestId && x.AssetId != null);
			return index;
		}

		private async Task UpdatePrimaryColors(ContentFromDto contentFromDto, ContentForm existingContentFrom)
		{
			_db.ContentFormPrimaryColors.RemoveRange(existingContentFrom.ContentFormPrimaryColors);
			foreach (var primaryColor in contentFromDto.PrimaryColors)
			{
				var contentFormPrimaryColor = new ContentFormPrimaryColor
				{
					Id = await _uniqueIds.UniqueContentFormPrimaryColorId(),
					ContentFormId = existingContentFrom.Id,
					PrimaryColor = primaryColor
				};
				_db.ContentFormPrimaryColors.Add(contentFormPrimaryColor);
			}
		}

		private async Task UpdateExamplePhotos(ContentFromDto contentFromDto, ContentForm existingContentFrom)
		{
			// Update the primary colors
			foreach (var contentFormExamplePhoto in contentFromDto.ContentFormExamplePhotos)
			{
				// Find the existing primary color to update
				var existingContentFormExamplePhoto = existingContentFrom.ContentFormExamplePhotos
					.FirstOrDefault(c => c.Id == contentFormExamplePhoto.Id);

				// If the primary color exists, update it
				if (existingContentFormExamplePhoto == null && existingContentFrom.ContentFormExamplePhotos.Count() < 3)
				{
					// If the primary color does not exist, add a new one
					_db.ContentFormExamplePhotos.Add(new ContentFormExamplePhoto
					{
						Id = await _uniqueIds.UniqueContentFormExamplePhotoId(),
						ContentFormId = existingContentFrom.Id,
						ExamplePhotoId = await _uniqueIds.UniqueContentAssetId()
					});
				}
			}
		}

		private async Task UpdateDesignStep(Guid requestId, int stepIndex)
		{
			var newStepTypeId = await _db.StepTypes
				.Where(x => x.Index == stepIndex)
				.Select(x => x.Id)
				.FirstOrDefaultAsync();

			var contentStep = await _db.ContentSteps
							.Where(x => x.ContentFormId == requestId)
							.FirstOrDefaultAsync() ?? throw new Exception("Error: Content Step does exists."); ;

			contentStep.StepTypeId = newStepTypeId;
			await _db.SaveChangesAsync();
		}

		private async Task AddContentStep(Guid contentFormId, int stepIndex)
		{

			if (stepIndex == 0)
			{
				var stepTypeId = await _db.StepTypes
				.Where(x => x.Index == 1)
				.Select(x => x.Id)
				.FirstOrDefaultAsync();
				var contentStepId = await _uniqueIds.UniqueContentStepId();
				var newContentStep = new ContentStep
				{
					Id = contentStepId,
					ContentFormId = contentFormId,
					DesignerId = Guid.Empty,
					StepTypeId = stepTypeId,
				};
				await _db.ContentSteps.AddAsync(newContentStep);
				await _db.SaveChangesAsync();
				return;
			}
			
			var newStepTypeId = await _db.StepTypes
				.Where(x => x.Index == stepIndex + 1)
				.Select(x => x.Id)
				.FirstOrDefaultAsync();

			var contentStep = await _db.ContentSteps
							.Where(x => x.ContentFormId == contentFormId)
							.FirstOrDefaultAsync() ?? throw new Exception("Error: Content Step does exists."); ;

			contentStep.StepTypeId = newStepTypeId;
			await _db.SaveChangesAsync();
		}

		public async Task<bool> UpsertProject(UpsertStudentProjectDTO project, bool editProject)
		{
			if (project.Id != null && editProject == true)
			{
				var existingProject = await _db.StudentProjects.FirstOrDefaultAsync(x => x.Id == project.Id)
					?? throw new Exception("Could not locate project.Please try again");
				existingProject.Name = project.Name;
				existingProject.InstitutionCourseId = project.Course;
				existingProject.InstitutionId = project.InstitutionId;
				existingProject.Year = project.Year;
				existingProject.YearWeight = project.YearWeight;
				existingProject.StartDateTime = project.StartDateTime;
				existingProject.EndDateTime = project.EndDateTime;
			}
			else
			{

				var existingProject = await _db.StudentProjects
								.Where(x => x.Year == project.Year &&
								x.InstitutionCourseId == project.Course &&
								x.Name == project.Name)
								.FirstOrDefaultAsync();

				if (existingProject != null && editProject == false)
				{
					throw new Exception("This project exists with the same Name, Course and year.");
				}

				var studentProject = new StudentProject
				{
					Id = Guid.NewGuid(),
					Name = project.Name,
					InstitutionCourseId = project.Course,
					InstitutionId = project.InstitutionId,
					Year = project.Year,
					YearWeight = project.YearWeight,
					StartDateTime = project.StartDateTime,
					EndDateTime = project.EndDateTime,
					CreatedAt = CommonFunctions.CurrentDateTime(),
					CreatedBy = project.CreatedBy
				};
				await _db.StudentProjects.AddAsync(studentProject);
			}

			await _db.SaveChangesAsync();

			return true;

		}

		public async Task<bool> DeleteProject(Guid projectId)
		{
			var existingProject = await _db.StudentProjects.FirstOrDefaultAsync(x => x.Id == projectId) ?? throw new Exception("Could not locate project.Please try again");

			_db.StudentProjects.Remove(existingProject);
			await _db.SaveChangesAsync();

			return true;
		}

		public async Task<List<StudentProjectDetails>> GetProjectStudents(Guid projectId)
		{
			var students = await _db.StudentProjects
				.Include(x => x.StudentProjectAssignments)
				.ThenInclude(x => x.AssignedToNavigation.User)
				.Include(x => x.ProjectRubricStudentMarks)
				.Where(x => x.Id == projectId)
				.SelectMany(x => x.StudentProjectAssignments.Select(a => new StudentProjectDetails
				{
					Id = a.AssignedToNavigation.Id,
					Name = a.AssignedToNavigation.User.FullName,
					Email = a.AssignedToNavigation.User.Email,
					Mark = x.ProjectRubricStudentMarks
							.Where(m => m.StudentId == a.AssignedTo) // Filter marks for the specific student
							.Select(m => m.Mark) // Select the mark value
							.FirstOrDefault() // Adjust the property for marks
				}))
				.ToListAsync();
			return students;
		}

		public async Task<List<StudentProjectDetailsDTO>> GetProjects(Guid? userId, bool studentProjects)
		{
			var projects = _db.StudentProjects.Include(x => x.InstitutionCourse).Select(x => new StudentProjectDetailsDTO
			{
				Id = x.Id,
				Name = x.Name,
				Course = new Course { Id = x.InstitutionCourse.Id, Name = x.InstitutionCourse.Course },
				CreatedBy = x.CreatedBy,
				Year = x.Year,
				YearWeight = x.YearWeight,
				Mark = null,
				StartDateTime = x.StartDateTime,
				EndDateTime = x.EndDateTime
			}).AsQueryable();

			if (userId != null)
			{
				projects = projects.Where(sp => sp.CreatedBy == userId);
			}

			return await projects.ToListAsync();
		}

		public async Task<List<StudentProjectDetailsDTO>> GetStudentProjects(Guid studentId)
		{
			var projects = _db.StudentProjects
				.Include(x => x.InstitutionCourse)
				.Include(sp => sp.StudentProjectAssignments)
				.Include(m => m.ProjectRubricStudentMarks)
				.Where(sp => sp.StudentProjectAssignments.Any(a => a.AssignedTo == studentId))
				.Select(x => new StudentProjectDetailsDTO
				{
					Id = x.Id,
					Name = x.Name,
					Course = new Course { Id = x.InstitutionCourse.Id, Name = x.InstitutionCourse.Course },
					CreatedBy = x.CreatedBy,
					Year = x.Year,
					YearWeight = x.YearWeight,
					StartDateTime = x.StartDateTime,
					EndDateTime = x.EndDateTime,
					Mark = x.ProjectRubricStudentMarks
						.Where(j => j.ProjectId == x.Id && j.StudentId == studentId)
						.Select(j => j.Mark)
						.FirstOrDefault()
				});

			return await projects.ToListAsync();
		}

		public async Task<StudentProjectDTO> GetProjectDetails(Guid projectId)
		{
			var project = await _db.StudentProjects.Include(x => x.InstitutionCourse)
				.Select(x => new StudentProjectDTO
				{
					Id = x.Id,
					Name = x.Name,
					Year = x.Year,
					YearWeight = x.YearWeight,
					CreatedBy = x.CreatedBy,
					StartDateTime = x.StartDateTime,
					EndDateTime = x.EndDateTime,
					Course = new Course { Id = x.InstitutionCourse.Id, Name = x.InstitutionCourse.Course}

				})
				.FirstOrDefaultAsync(x => x.Id == projectId) ?? throw new Exception("Can't find project Details");

			return project;
		}

		public async Task<IEnumerable<ProjectStudent>> GetProjectStudentsToAdd(StudentProjectDTO projectDetails)
		{
			var students = _db.Students
						.Include(x => x.User)
						.Include(s => s.StudentProjectAssignments)
						.Include(x => x.StudentCourses)
						.Where(x => x.StudentCourses.Any(x => x.InstitutionCoursesId == projectDetails.Course.Id) && x.Year == projectDetails.Year)
						.Select(s => new ProjectStudent
						{
							Id = s.Id,
							Name = s.User.FullName,
							Year = s.Year,
							HasProject = s.StudentProjectAssignments.Any(a => a.StudentProjectId == projectDetails.Id)
						})
						.AsQueryable();

			return await students.ToListAsync();
		}

		public async Task<bool> AddStudentToProject(StudentProjectAssignmentDTO studentProjectAssignmentDTO)
		{
			var userHasBeenAdded = await _db.StudentProjectAssignments.FirstOrDefaultAsync(x => x.AssignedTo == studentProjectAssignmentDTO.AssignedTo && x.StudentProjectId == studentProjectAssignmentDTO.StudentProjectId);

			if (userHasBeenAdded == null)
			{
				var studentProjectAssignment = new StudentProjectAssignment
				{
					Id = Guid.NewGuid(),
					StudentProjectId = studentProjectAssignmentDTO.StudentProjectId,
					AssignedBy = studentProjectAssignmentDTO.AssignedBy,
					AssignedTo = studentProjectAssignmentDTO.AssignedTo,
					AssignedAt = CommonFunctions.CurrentDateTime(),
				};

				await _db.StudentProjectAssignments.AddAsync(studentProjectAssignment);
				await _db.SaveChangesAsync();
			}

			return true;
		}

		public async Task<bool> RemoveStudentToProject(StudentProjectAssignmentDTO studentProjectAssignmentDTO)
		{
			var userHasBeenAdded = await _db.StudentProjectAssignments.FirstOrDefaultAsync(x => x.AssignedTo == studentProjectAssignmentDTO.AssignedTo && x.StudentProjectId == studentProjectAssignmentDTO.StudentProjectId);

			if (userHasBeenAdded != null)
			{
				_db.StudentProjectAssignments.Remove(userHasBeenAdded);
				await _db.SaveChangesAsync();
			}

			return true;
		}

		public async Task<List<ProjectRubricDTO>> GetRubricForProject(Guid projectId)
		{
			var projectRubricDetails = await _db.ProjectRubrics.Where(x => x.ProjectId == projectId).OrderBy(x => x.SortOrder)
				.Select(x => new
			ProjectRubricDTO {
				ProjectId = x.ProjectId,
				Name = x.Name,
				Description = x.Description,
				Weight = x.Weight,
				
			}).ToListAsync();

			return projectRubricDetails;
		}

		public async Task<List<ProjectRubricOfStudentDTO>> GetRubricForProjectOfStudent(Guid projectId, Guid studentId)
		{
			var projectRubricDetails = await _db.ProjectRubrics
				.Include(x => x.ProjectRubricsOfStudents) // Include related students
				.Where(x => x.ProjectId == projectId) // Filter by project
				.OrderBy(x => x.SortOrder) // Order by sort order
				.Select(x => new ProjectRubricOfStudentDTO
				{
					RubricId = x.Id,
					ProjectId = x.ProjectId,
					Name = x.Name,
					Description = x.Description,
					Weight = x.Weight,
					Mark = x.ProjectRubricsOfStudents
						.Where(j => j.RubricId == x.Id && j.StudentId == studentId)
						.Select(j => j.Mark)
						.FirstOrDefault(), // Get the mark for the specific student
					Comment = x.ProjectRubricsOfStudents
						.Where(j => j.RubricId == x.Id && j.StudentId == studentId)
						.Select(j => j.Comment)
						.FirstOrDefault(), // Get the comment for the specific student
				})
				.ToListAsync();

			return projectRubricDetails;
		}

		public async Task<bool> SaveRubricForProjectOfStudent(List<ProjectRubricOfStudentMarkedDTO> projectRubricOfStudentMarkedDTO)
		{
			// Extract relevant RubricIds and StudentIds for filtering
			var rubricIds = projectRubricOfStudentMarkedDTO.Select(dto => dto.RubricId).ToList();
			var studentId = projectRubricOfStudentMarkedDTO.First().StudentId;
			var projectId = projectRubricOfStudentMarkedDTO.First().ProjectId;

			// Fetch all relevant ProjectRubricsOfStudents
			var projectRubricDetails = await _db.ProjectRubrics
				.Include(x => x.ProjectRubricsOfStudents)
				.Where(x => rubricIds.Contains(x.Id))
				.ToListAsync();

			if (projectRubricDetails.Any())
			{
				double projectMark = 0.0;
				// Update the existing records
				foreach (var detail in projectRubricDetails)
				{
					var matchingDto = projectRubricOfStudentMarkedDTO
						.FirstOrDefault(dto => dto.RubricId == detail.Id);

					if(matchingDto == null)
					{
						continue;
					}

					var rubric = detail.ProjectRubricsOfStudents.FirstOrDefault(x => x.StudentId == matchingDto.StudentId);

					if (rubric != null)
					{
						rubric.Mark = matchingDto.Mark;
						rubric.Comment = matchingDto.Comment;
						rubric.UpdatedAt = CommonFunctions.CurrentDateTime();
					}
					else
					{
						var projectRubricsOfStudents = new ProjectRubricsOfStudent
						{
							Id = Guid.NewGuid(),
							RubricId = detail.Id,
							StudentId = matchingDto.StudentId,
							Mark = matchingDto.Mark,
							Comment = matchingDto.Comment,
							UpdatedAt = CommonFunctions.CurrentDateTime(),
						};

						_db.ProjectRubricsOfStudents.Add(projectRubricsOfStudents);
					}

					double weightInPercentage = detail.Weight / 100.0;
					projectMark += weightInPercentage * matchingDto.Mark;
				}

				var projectRubricStudentMarks = _db.ProjectRubricStudentMarks.FirstOrDefault(x => x.ProjectId == projectId);
				if (projectRubricStudentMarks != null)
				{
					projectRubricStudentMarks.Mark = projectMark;
				}
				else
				{
					var newProjectRubricStudentMarks = new ProjectRubricStudentMark
					{
						Id = Guid.NewGuid(),
						ProjectId = projectId,
						StudentId = studentId,
						Mark = projectMark,
						UpdatedAt = CommonFunctions.CurrentDateTime()
					};

					_db.ProjectRubricStudentMarks.Add(newProjectRubricStudentMarks);
				}
			}

			
			// Save changes to the database
			await _db.SaveChangesAsync();

			return true;
		}

		public async Task<bool> SaveRubricForProject(List<ProjectRubricDTO> projectRubricDTO)
		{
			var projectRubrics = await _db.ProjectRubrics
								.Where(x => projectRubricDTO.Select(r => r.ProjectId).Contains(x.ProjectId))
								.ToListAsync();
			if (projectRubrics.Any())
			{

				_db.ProjectRubrics.RemoveRange(projectRubrics);
				await _db.SaveChangesAsync();
			}

			var projectRubricDetails = new List<ProjectRubric>();

			projectRubricDTO.ForEach(x =>
			{
				var projectRubric = new ProjectRubric
				{
					Id = Guid.NewGuid(),
					Name = x.Name,
					Description = x.Description,
					Weight = x.Weight,
					ProjectId = x.ProjectId,
					SortOrder = x.SortOrder,
					UpdatedAt = CommonFunctions.CurrentDateTime(),
				};

				projectRubricDetails.Add(projectRubric);
			});

			_db.ProjectRubrics.AddRange(projectRubricDetails);
			await _db.SaveChangesAsync();

			return true;
		}
	}
}

		//public async Task<bool> Delete(Guid contentId)
		//{
		//	var content = await _db.Contents.Where(x => x.Id == contentId).FirstOrDefaultAsync();
		//	if (content != null)
		//	{
		//		var likes = await _db.LikedContents.Where(x => x.ContentId == content.Id).ToListAsync();
		//		_db.LikedContents.RemoveRange(likes);

		//		var comments = await _db.Comments.Where(x => x.ContentId == contentId).ToListAsync();

		//		var commentIds = comments.Select(x => x.Id);
		//		var commentLikes = await _db.LikedComments.Where(x => commentIds.Contains(x.CommentId)).ToListAsync();
		//		_db.LikedComments.RemoveRange(commentLikes);
		//		_db.Comments.RemoveRange(comments);

		//		var path = @$"https://socioleeblobstorage.blob.core.windows.net/postasset/{content.Id}";
		//		var imageDetails = new Uri(path).Segments;
		//		var client = _blobServiceClient.GetBlobContainerClient(imageDetails[1]);

		//		var blobClient = client.GetBlobClient(imageDetails[2]);
		//		await blobClient.DeleteIfExistsAsync();
		//		_db.Contents.Remove(content);
		//		await _db.SaveChangesAsync();
		//		return true;
		//	}
		//	return false;
		//}

	//	public async Task<int> GetContentCount(string contentType)
	//	{
	//		var count = await _db.Contents
	//			.Where(x => x.ContentType == contentType).Select(x => x.Id).CountAsync();
	//		return count;
	//	}

	//	public async Task<IEnumerable<Content>> GetContents(string contentType, int skip, int limit){
	//		var contents = await _db.Contents
	//			.Include(x => x.LikedContents)
	//			.Where(x => x.ContentType == contentType)
	//			.OrderByDescending(x => x.Created)
	//			.Skip(skip)
	//			.Take(limit)
	//			.ToListAsync();

	//		return contents;
	//	}

	//	public async Task<bool> LikeContent(Guid contentId, Guid userId)
	//	{
	//		var hasLikedReview = await _db.LikedContents.
	//									FirstOrDefaultAsync(x => x.ContentId == contentId && x.UserId == userId);
	//		if (hasLikedReview != null)
	//			throw new HttpRequestException("Already liked post.");

	//		var likedContent = new LikedContent
	//		{
	//			Id = await _uniqueIds.UniqueLikedContentId(),
	//			ContentId = contentId,
	//			UserId = userId
	//		};

	//		_db.LikedContents.Add(likedContent);
	//		await _db.SaveChangesAsync();
	//		return true;
	//	}

	//	public async Task<bool> UnlikeContent(Guid contentId, Guid userId)
	//	{
	//		var hasLikedContent = await _db.LikedContents.FirstOrDefaultAsync(x => x.ContentId == contentId && x.UserId == userId) ?? throw new HttpRequestException("Error");

	//		_db.LikedContents.Remove(hasLikedContent);
	//		await _db.SaveChangesAsync();
	//		return true;
	//	}

	//	public async Task<List<LikedContent>> GetContentLikes(Guid contentId)
	//	{
	//		return await _db.LikedContents.Where(x => x.ContentId== contentId).ToListAsync();
	//	}
	//}
//}