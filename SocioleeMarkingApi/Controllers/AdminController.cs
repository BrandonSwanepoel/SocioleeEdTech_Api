using System;
using Microsoft.AspNetCore.Mvc;
using SocioleeMarkingApi.Models.Database;
using SocioleeMarkingApi.Models.DTOs;
using SocioleeMarkingApi.Services;

namespace SocioleeMarkingApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdminController : Controller
	{
		private readonly IAdminService _AdminService;
		//private readonly ILogger<UsersController> _logger;

		public AdminController(IAdminService adminService)
		{
			_AdminService = adminService;
		}

		[HttpGet("getAdminDetails")]
		[ProducesResponseType(typeof(InstitutionDTO), 200)]
		public async Task<IActionResult> GetAdminDetails(Guid institutionId)
		{
			var adminDetails = await _AdminService.GetAdminDetails(institutionId);
			return Ok(adminDetails);
		}

		[HttpGet("getLecturers")]
		[ProducesResponseType(typeof(Task<IEnumerable<LecturerDTO>>), 200)]
		public async Task<IActionResult> GetStudents([FromQuery] Guid institutionId)
		{
			var userDesignStep = await _AdminService.GetLecturers(institutionId);
			return Ok(userDesignStep);
		}

		[HttpPost("upsertInstitution")]
		[ProducesResponseType(typeof(bool), 200)]
		public async Task<IActionResult> UpsertInstitution([FromBody] InstitutionDTO institutionDto, [FromQuery] bool editInstitution = false)
		{
			var updated = await _AdminService.UpsertInstitution(institutionDto, editInstitution);
			return Ok(updated);
		}

		[HttpPost("upsertLecturer")]
		[ProducesResponseType(typeof(bool), 200)]
		public async Task<IActionResult> UpsertLecturer([FromBody] UpsertLecturerDTO lecturerDto, [FromQuery] bool editLecturer = false)
		{
			var updated = await _AdminService.UpsertLecturer(lecturerDto, editLecturer);
			return Ok(updated);
		}

		[HttpGet("programmes")]
		[ProducesResponseType(typeof(IEnumerable<Programme>), 200)]
		public async Task<IActionResult> GetProgrammes([FromQuery] Guid institutionId)
		{
			var programmes = await _AdminService.GetProgrammes(institutionId);
			return Ok(programmes);
		}

		[HttpDelete("deleteStudent")]
		[ProducesResponseType(typeof(bool), 200)]
		public async Task<IActionResult> DeleteStudent([FromQuery] Guid studentId)
		{
			var deleted = await _AdminService.DeleteLecturer(studentId);
			return Ok(deleted);
		}

		[HttpGet("getUserRoles")]
		[ProducesResponseType(typeof(IEnumerable<RoleType>), 200)]
		public async Task<IActionResult> GetUserRoles()
		{
			var roleTypes = await _AdminService.GetRoleType();
			return Ok(roleTypes);
		}

		//[HttpPut("updateUserDetails")]
		//[ProducesResponseType(typeof(Task<bool>), 200)]
		//public async Task<IActionResult> UpdateUserDetails(UserDto userDto)
		//{
		//	var updatedUser = await _AdminService.UpdateAdminDetails(userDto);
		//	return Ok(updatedUser);
		//}

		//[HttpPut("updateUserEmail")]
		//[ProducesResponseType(typeof(Task<bool>), 200)]
		//public async Task<IActionResult> UpdateUserEmail(UserDto userDto)
		//{
		//	var updatedUser = await _AdminService.UpdateAdminEmail(userDto);
		//	return Ok(updatedUser);
		//}
	}
}

