using SocioleeMarkingApi.Models.Database;

namespace SocioleeMarkingApi.Models
{
	public class ContentFromDto
	{
		public Guid? Id { get; set; }
		public Guid UserId { get; set; }
		public string Title { get; set; } = string.Empty;
		public string ContentType { get; set; } = string.Empty;
		public bool IncludedLogo { get; set; }
		public bool IncludedBackground { get; set; }
		public string ContentHeadline { get; set; } = string.Empty;
		public string ContentBodyText { get; set; } = string.Empty;
		public string ContentDescription { get; set; } = string.Empty;
		public bool IncludedVoiceNote { get; set; }
		public List<string> PrimaryColors { get; set; } = new List<string>();
		public bool IncludedExamplePhotos { get; set; }
		public int ExamplePhotoCount { get; set; }
		public Guid AssetAspectRatioId { get; set; }
		public List<ContentFormExamplePhoto> ContentFormExamplePhotos { get; set; } = new List<ContentFormExamplePhoto>();
		public bool IncludeContactDetails { get; set; }
		public string Email { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public bool DesignRedeemed { get; set; }
	}

	public class ContentFormIn
	{
		public ContentFormIn(ContentForm contentForm, AssetAspectRatio assetAspectRatio)
		{
			Id = contentForm.Id;
			UserId = contentForm.UserId;
			ContentType = contentForm.ContentType;
			LogoId = contentForm.LogoId;
			BackgroundImageId = contentForm.BackgroundImageId;
			ContentHeadline = contentForm.ContentHeadline;
			ContentBodyText = contentForm.ContentBodyText;
			ContentDescription = contentForm.ContentDescription;
			VoiceNoteId = contentForm.VoiceNoteId;
			IncludeContactDetails = contentForm.IncludeContactDetails;
			Email = contentForm.Email;
			PhoneNumber = contentForm.PhoneNumber;
			Created = contentForm.Created;
			AssetAspectRatio = assetAspectRatio;
		}

		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public string? ContentType { get; set; }
		public Guid? LogoId { get; set; }
		public Guid? BackgroundImageId { get; set; }
		public string? ContentHeadline { get; set; }
		public string? ContentBodyText { get; set; }
		public string? ContentDescription { get; set; }
		public Guid? VoiceNoteId { get; set; }
		public bool IncludeContactDetails { get; set; }
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
		public DateTime Created { get; set; }
		public AssetAspectRatio AssetAspectRatio { get; set; } = null!;
	}

	public class ContentFormAssetIds
	{
		public Guid? LogoId { get; set; }
		public Guid? BackgroundImageId { get; set; }
		public Guid? VoiceNoteId { get; set; }
		public List<Guid> ExamplePhotoIds { get; set; } = new List<Guid>();
	}

	public class CreatedContentForm
	{
		public ContentForm ContentForm { get; set; } = null!;
		public ContentFormAssetIds ContentFormAssetIds { get; set; } = null!;
	}

	public class Step
	{
		public Step(int index, string name)
		{
			Index = index;
			Name = name;
		}
		public int Index { get; set; }
		public string Name { get; set; }
	}

	public class DesignRequest
	{
		public DesignRequest(ContentForm contentForm, User user, bool downloaded, StepType stepType)
		{
			FullName = user.FullName;
			ContentType = contentForm.ContentType;
			Title = contentForm.Title;
			FormId = contentForm.Id;
			Created = contentForm.Created;
			Downloaded = downloaded;
			Step = new Step(stepType.Index, stepType.Name);
		}

		public string FullName { get; set; } = string.Empty;
		public string? ContentType { get; set; } = string.Empty;
		public string? Title { get; set; } = string.Empty;
		public Guid FormId { get; set; }
		public DateTime Created { get; set; }
		public bool Downloaded { get; set; }
		public Step Step { get; set; }
	}

	public class UserActiveStep
	{
		public UserActiveStep(string requestTitle, Guid formId, int activeStepIndex, string stepName)
		{
			RequestTitle = requestTitle;
			FormId = formId;
			ActiveStepIndex = activeStepIndex;
			StepName = stepName;
		}

		public string RequestTitle { get; set; }
		public Guid FormId { get; set; }
		public int ActiveStepIndex { get; set; }
		public string StepName { get; set; }
	}

	public class UploadContentDesign
	{
		public string ContentPath { get; set; } = null!;
		public Guid FormId { get; set; }
		public int StepIndex { get; set; }
		public Guid AssetAspectRatioId { get; set; }
	}

	public class StudentDTO
	{
		public StudentDTO(User user, Student student, IEnumerable<Programme> Programmes)
		{
			Id = student.Id;
			Name = user.FullName;
			Year = student.Year;
			Programme = Programmes;
			Email = user.Email;
		}
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public int Year { get; set; }
		public IEnumerable<Programme> Programme { get; set; } = null!;
		public string Email { get; set; } = null!;
	}

	public class LecturerDTO
	{
		public LecturerDTO(User user, RoleType role, InstitutionLecturer lecturer, IEnumerable<Programme> Programmes)
		{
			Id = lecturer.Id;
			Name = user.FullName;
			Year = lecturer.Years;
			Programme = Programmes;
			Email = user.Email;
			Role = role;
		}
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public int? Year { get; set; }
		public IEnumerable<Programme> Programme { get; set; } = null!;
		public string Email { get; set; } = null!;
		public RoleType Role { get; set; } = null!;
	}

	public class UpsertStudentDTO
	{
		public Guid? Id { get; set; }
		public Guid InstitutionId { get; set; }
		public string Name { get; set; } = null!;
		public int Year { get; set; }
		public List<Guid> Programme { get; set; } = new List<Guid>();
		public string Email { get; set; } = null!;
	}

	public class UserAnalytics
	{
		public double GradeAve { get; set; }
		public List<UserProgrammeAnalytics> UserProgrammeAnalytics { get; set; } = new List<UserProgrammeAnalytics>();
	}

	public class UserProgrammeAnalytics
	{
		public string ProgrammeName { get; set; } = null!;
		public double AverageGrade { get; set; }
	}

	public class Programme
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
	}

	public class UpdateFormStep
	{
		public int StepIndex { get; set; }
		public Guid FormId { get; set; }
	}

	public class DesignChangeRequestsDTO
	{
		public Guid RequestId { get; set; }
		public Guid MessageAssetId { get; set; }
		public bool HasChanges { get; set; }
		public List<DesignChange> DesignChanges { get; set; } = new List<DesignChange>();
	}

	public class DesignChange
	{
		public DesignChange(double x, double y, int number, string text)
		{
			X = x;
			Y = y;
			Number = number;
			Text = text;
		}
		public double X { get; set; }
		public double Y { get; set; }
		public int Number { get; set; }
		public string Text { get; set; } = null!;
	}

	public class ContentNotes
	{
		public string Note { get; set; } = null!;
	}

	public class DesignReview
	{
		public Guid MessageAssetId { get; set; }
		public string MessageAssetPath { get; set; } = null!;
		public IEnumerable<DesignChange>? DesignChanges { get; set; } = null;
	}


	public class StudentProjectDTO
	{
		public Guid? Id { get; set; }
		public string Name { get; set; } = null!;
		public int Year { get; set; }
		public Programme Programme { get; set; }
		public string Subject { get; set; } = null!;
		public double YearWeight { get; set; }
		public DateTime? StartDateTime { get; set; }
		public DateTime? EndDateTime { get; set; }
	}

	public class UpsertStudentProjectDTO
	{
		public Guid? Id { get; set; }
		public Guid InstitutionId { get; set; }
		public string Name { get; set; } = null!;
		public int Year { get; set; }
		public Guid Programme { get; set; }
		public string Subject { get; set; } = null!;
		public double YearWeight { get; set; }
		public DateTime? StartDateTime { get; set; }
		public DateTime? EndDateTime { get; set; }
		public Guid CreatedBy { get; set; }
	}

	public class ProjectStudent
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public int Year { get; set; }
		//public string Programme { get; set; } = null!;
		public bool HasProject { get; set; }
	}

	public class UpsertProjectDTO
	{
		public Guid? Id { get; set; }
		public Guid InstitutionProgrammeId { get; set; }
		public string Name { get; set; } = null!;
		public int Year { get; set; }
		public string Email { get; set; } = null!;
	}

	public class StudentProjectDetails
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public string Email { get; set; } = null!;
		public double? Mark { get; set; }
	}

	public class StudentProjectDetailsDTO
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public Programme Programme { get; set; } = null!;
		public int Year { get; set; }
		public double YearWeight { get; set; }
		public double? Mark { get; set; }
		public DateTime? StartDateTime { get; set; }
		public DateTime? EndDateTime { get; set; }
		public Guid CreatedBy { get; set; }
	}

	public class ProjectRubricDTO
	{
		public Guid ProjectId { get; set; }
		public string Name { get; set; } = null!;
		public string Description { get; set; } = null!;
		public double Weight { get; set; }
		public int SortOrder { get; set; }
	}

	public class ProjectRubricOfStudentDTO
	{
		public Guid ProjectId { get; set; }
		public Guid StudentId { get; set; }
		public Guid RubricId { get; set; }
		public string Name { get; set; } = null!;
		public string Description { get; set; } = null!;
		public double Weight { get; set; }
		public int SortOrder { get; set; }
		public double? Mark { get; set; }
		public string? Comment { get; set; } = null!;
	}

	public class ProjectRubricOfStudentMarkedDTO
	{
		public Guid ProjectId { get; set; }
		public Guid StudentId { get; set; }
		public Guid RubricId { get; set; }
		public double Mark { get; set; }
		public string? Comment { get; set; } = null!;
	}

	public class StudentProjectAssignmentDTO
	{
		public Guid StudentProjectId { get; set; } // Reference to the student project

		public Guid AssignedBy { get; set; }       // User who assigned the project

		public Guid AssignedTo { get; set; }
	}
		//public class ContentAssetAspectRatio
		//{

		//	public ContentAssetAspectRatio(AssetAspectRatio assetAspectRatio)
		//	{
		//		AspectRatioId = assetAspectRatio.Id;
		//		AspectRatio = assetAspectRatio.AspectRatio;
		//		Type
		//	}
		//	public Guid AspectRatioId { get; set; }
		//	public string AspectRatio { get; set; }
		//	public string Type { get; set; } = null!;
		//}

		public class Design
	{
		public Design(MessageAsset messageAsset, string aspectRatio)
		{
			ContentPath = messageAsset.ContentPath;
			AspectRatio = aspectRatio;
			Changes = messageAsset.DesignChangeRequests.Select(x => new DesignChange(x.X, x.Y, x.Number, x.Text));
		}

		public string ContentPath { get; set; }
		public IEnumerable<DesignChange>? Changes { get; set; }
		public string AspectRatio { get; set; }
	}

	public class UserContent
	{
		public UserContent(Message? message)
		{
			//if (message != null && message.AssetId != null && message.Asset != null)
			//{
			//	Id = (Guid)message.AssetId;
			//	UserId = message.UserId;
			//	ContentPath = message.Asset.ContentPath;
			//	Created = message.Created;
			//}
		}

		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public string ContentPath { get; set; }
		public DateTime Created { get; set; }
	}
}
