using Microsoft.AspNetCore.Mvc;
using SocioleeMarkingApi.Models.Database;
using SocioleeMarkingApi.Services;

namespace SocioleeMarkingApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ContentController : Controller
	{

		private readonly IContentService _ContentService;
		private readonly IUserService _userService;

		//private readonly ILogger<UsersController> _logger;

		public ContentController(IContentService contentService, IUserService userService)
		{
			_ContentService = contentService;
			_userService = userService;
	}

		[HttpPost("createContentForm")]
		[ProducesResponseType(typeof(Task<ContentForm>), 200)]
		public async Task<IActionResult> Create([FromBody] ContentFromDto request)
		{
			var contentFormAssetIds = await _ContentService.Create(request);

			//if(request.ContentType == "Compition")
			//{
			//	await PushNotifications.SendPushNotification();
			//}
			return Ok(contentFormAssetIds);
		}

		[HttpGet("assetAspectRatios")]
		[ProducesResponseType(typeof(Task<AssetAspectRatio[]>), 200)]
		public async Task<IActionResult> GetAspectRatios()
		{
			var contentFormAssetIds = await _ContentService.GetAspectRatio();
			return Ok(contentFormAssetIds);
		}

		[HttpGet("form/{formId}")]
		[ProducesResponseType(typeof(Task<ContentForm>), 200)]
		public async Task<IActionResult> GetContentForm([FromRoute] Guid formId)
		{
			var contentForm = await _ContentService.GetContentForm(formId);
			return Ok(contentForm);
		}

		[HttpGet("stepTypes")]
		[ProducesResponseType(typeof(Task<List<Step>>), 200)]
		public async Task<IActionResult> GetStepTypes()
		{
			var steps = await _ContentService.GetStepTypes();
			return Ok(steps);
		}

		[HttpGet("designIndex")]
		[ProducesResponseType(typeof(Task<int>), 200)]
		public async Task<IActionResult> GetDesignIndex([FromQuery] Guid requestId)
		{
			var index = await _ContentService.GetDesignIndex(requestId);
			return Ok(index);
		}

		[HttpGet("clientDesignRequests")]
		[ProducesResponseType(typeof(Task<List<DesignRequest>>), 200)]
		public async Task<IActionResult> GetClientDesignRequests()
		{
			var clientDesignRequests = await _ContentService.GetClientDesignRequests();
			return Ok(clientDesignRequests);
		}

		[HttpGet("userDesignStep")]
		[ProducesResponseType(typeof(Task<IEnumerable<UserActiveStep>>), 200)]
		public async Task<IActionResult> GetUserDesignStep([FromQuery] Guid userId)
		{
			var userDesignStep = await _ContentService.GetUserDesignStep(userId);
			return Ok(userDesignStep);
		}

		[HttpGet("students")]
		[ProducesResponseType(typeof(Task<IEnumerable<Models.StudentDTO>>), 200)]
		public async Task<IActionResult> GetStudents([FromQuery] Guid userId, [FromQuery] int limit)
		{
			var userDesignStep = await _ContentService.GetStudents(userId);
			return Ok(userDesignStep);
		}

		[HttpGet("requestDesignStep")]
		[ProducesResponseType(typeof(Task<UserActiveStep>), 200)]
		public async Task<IActionResult> GetRequestDesignStep([FromQuery] Guid requestId)
		{
			var userDesignStep = await _ContentService.GetRequestDesignStep(requestId);
			return Ok(userDesignStep);
		}

		[HttpGet("formAspectRatio")]
		[ProducesResponseType(typeof(Task<IEnumerable<AssetAspectRatio>>), 200)]
		public async Task<IActionResult> GetFormAspectRatio([FromQuery] Guid formId)
		{
			var assetAspectRatio = await _ContentService.GetFormAspectRatio(formId);
			return Ok(assetAspectRatio);
		}

		[HttpGet("formDesignStep")]
		[ProducesResponseType(typeof(Task<UserActiveStep?>), 200)]
		public async Task<IActionResult> GetformDesignStep([FromQuery] Guid formId)
		{
			var userDesignStep = await _ContentService.GetFormDesignStep(formId);
			return Ok(userDesignStep);
		}

		[HttpPost("uploadContent")]
		[ProducesResponseType(typeof(Task<bool>), 200)]
		public async Task<IActionResult> UploadContent([FromBody] UploadContentDesign uploadContentDesign)
		{
			var uploaded = await _ContentService.UploadContent(uploadContentDesign);

			return Ok(uploaded);
		}

		[HttpPost("contentDownloaded")]
		[ProducesResponseType(typeof(Task<bool>), 200)]
		public async Task<IActionResult> ContentDownloaded([FromQuery] Guid contentFormId)
		{
			var downloaded = await _ContentService.ContentDownloaded(contentFormId);
			return Ok(downloaded);
		}

		[HttpGet("userContent")]
		[ProducesResponseType(typeof(Task<List<UserContent>>), 200)]
		public async Task<IActionResult> GetUserContent()
		{
			var userId = _userService.GetUserId();
			if (userId == Guid.Empty)
				throw new HttpRequestException("Token invalid please sign in again.");
			var contents = await _ContentService.GetUserContent(userId);
			return Ok(contents);
		}

		//[HttpGet("designByFromId")]
		//[ProducesResponseType(typeof(Task<IEnumerable<Design>>), 200)]
		//public async Task<IActionResult> DesignByFromId([FromQuery] Guid formId)
		//{
		//	var designs = await _ContentService.DesignByFromId(formId);
		//	return Ok(designs);
		//}

		[HttpPost("addContentNotes")]
		[ProducesResponseType(typeof(Task<bool>), 200)]
		public async Task<IActionResult> AddContentNotes([FromBody] DesignChangeRequestsDTO updateFormStep)
		{
			var addNote = await _ContentService.AddContentNotes(updateFormStep);
			return Ok(addNote);
		}

		[HttpPost("completedDesignRequest")]
		[ProducesResponseType(typeof(Task<bool>), 200)]
		public async Task<IActionResult> CompletedDesignRequest([FromQuery] Guid requestId)
		{
			var completedDesign = await _ContentService.CompletedDesignRequest(requestId);
			return Ok(completedDesign);
		}
		

		[HttpPost("noContentNotes")]
		[ProducesResponseType(typeof(Task<bool>), 200)]
		public async Task<IActionResult> NoContentNotes([FromBody] DesignChangeRequestsDTO updateFormStep)
		{
			var addNote = await _ContentService.NoContentNotes(updateFormStep);
			return Ok(addNote);
		}

		[HttpGet("contentNote")]
		[ProducesResponseType(typeof(Task<IEnumerable<ContentNotes>>), 200)]
		public async Task<IActionResult> GetContentNote([FromQuery] Guid formId)
		{
			var note = await _ContentService.GetContentNote(formId);
			return Ok(note);
		}

		[HttpGet("ContentDesignReview")]
		[ProducesResponseType(typeof(Task<DesignReview>), 200)]
		public async Task<IActionResult> GetContentDesignReview([FromQuery] Guid messageId)
		{
			var note = await _ContentService.GetContentDesignReview(messageId);
			return Ok(note);
		}

		[HttpPost("upsertProject")]
		[ProducesResponseType(typeof(Task<bool>), 200)]
		public async Task<IActionResult> UpsertProject([FromBody] UpsertStudentProjectDTO project, [FromQuery] bool editProject = false)
		{
			var added = await _ContentService.UpsertProject(project, editProject);
			return Ok(added);
		}

		[HttpDelete("deleteProject")]
		[ProducesResponseType(typeof(bool), 200)]
		public async Task<IActionResult> DeleteProject([FromQuery] Guid projectId)
		{
			var deleted = await _ContentService.DeleteProject(projectId);
			return Ok(deleted);
		}

		[HttpGet("getProjects")]
		[ProducesResponseType(typeof(Task<List<StudentProjectDetailsDTO>>), 200)]
		public async Task<IActionResult> GetProjects([FromQuery] Guid? userId, [FromQuery] bool studentProjects)
		{
			var projects = await _ContentService.GetProjects(userId, studentProjects);
			return Ok(projects);
		}

		[HttpGet("getProjectStudents")]
		[ProducesResponseType(typeof(Task<IEnumerable<StudentProjectDetails>>), 200)]
		public async Task<IActionResult> GetProjectStudents([FromQuery] Guid projectId)
		{
			var students = await _ContentService.GetProjectStudents(projectId);
			return Ok(students);
		}

		[HttpGet("getStudentProjects")]
		[ProducesResponseType(typeof(Task<List<StudentProjectDetailsDTO>>), 200)]
		public async Task<IActionResult> GetStudentProjects([FromQuery] Guid studentId)
		{
			var projects = await _ContentService.GetStudentProjects(studentId);
			return Ok(projects);
		}

		[HttpGet("getProjectDetails")]
		[ProducesResponseType(typeof(Task<StudentProject>), 200)]
		public async Task<IActionResult> GetProjecDetails([FromQuery] Guid projectId)
		{
			var projects = await _ContentService.GetProjectDetails(projectId);
			return Ok(projects);
		}

		[HttpPost("getProjectStudentsToAdd")]
		[ProducesResponseType(typeof(Task<List<ProjectStudent>>), 200)]
		public async Task<IActionResult> GetProjectStudentsToAdd([FromBody] StudentProjectDTO projectDetails)
		{
			var students = await _ContentService.GetProjectStudentsToAdd(projectDetails);
			return Ok(students);
		}

		[HttpPost("addProjectStudent")]
		[ProducesResponseType(typeof(Task<bool>), 200)]
		public async Task<IActionResult> AddStudentToProject([FromBody] StudentProjectAssignmentDTO studentProjectAssignmentDTO)
		{
			var added = await _ContentService.AddStudentToProject(studentProjectAssignmentDTO);
			return Ok(added);
		}

		[HttpPost("removeProjectStudent")]
		[ProducesResponseType(typeof(Task<bool>), 200)]
		public async Task<IActionResult> RemoveProjectStudent([FromBody] StudentProjectAssignmentDTO studentProjectAssignmentDTO)
		{
			var added = await _ContentService.RemoveStudentToProject(studentProjectAssignmentDTO);
			return Ok(added);
		}

		[HttpGet("getRubricForProject")]
		[ProducesResponseType(typeof(Task<List<ProjectRubric>>), 200)]
		public async Task<IActionResult> GetRubricForProject([FromQuery] Guid projectId)
		{
			var projectRubricDetails = await _ContentService.GetRubricForProject(projectId);
			return Ok(projectRubricDetails);
		}

		[HttpGet("getRubricForProjectOfStudent")]
		[ProducesResponseType(typeof(Task<List<ProjectRubricOfStudentDTO>>), 200)]
		public async Task<IActionResult> GetRubricForProjectOfStudent([FromQuery] Guid projectId, [FromQuery] Guid studentId)
		{
			var projectRubricDetails = await _ContentService.GetRubricForProjectOfStudent(projectId, studentId);
			return Ok(projectRubricDetails);
		}

		[HttpPost("saveRubricForProjectOfStudent")]
		[ProducesResponseType(typeof(Task<bool>), 200)]
		public async Task<IActionResult> SaveRubricForProjectOfStudent([FromBody] List<ProjectRubricOfStudentMarkedDTO> projectRubricOfStudentMarkedDTO)
		{
			var saved = await _ContentService.SaveRubricForProjectOfStudent(projectRubricOfStudentMarkedDTO);
			return Ok(saved);
		}


		[HttpPost("saveRubricForProject")]
		[ProducesResponseType(typeof(Task<bool>), 200)]
		public async Task<IActionResult> SaveRubricForProject([FromBody] List<ProjectRubricDTO> projectRubricDTO)
		{
			var saved = await _ContentService.SaveRubricForProject(projectRubricDTO);
			return Ok(saved);
		}
	}
}

