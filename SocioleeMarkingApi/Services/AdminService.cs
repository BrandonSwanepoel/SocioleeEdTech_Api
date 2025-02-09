using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorHtmlEmails.Common;
using SocioleeMarkingApi.Authorization;
using SocioleeMarkingApi.Common;
using SocioleeMarkingApi.Models.Database;
using SocioleeMarkingApi.Models.DTOs;

namespace SocioleeMarkingApi.Services
{
	public interface IAdminService
	{
		//Lecturer
		Task<bool> UpsertLecturer(UpsertLecturerDTO studentDTO, bool editStudent);
		Task<IEnumerable<Course>> GetCourses(Guid institutionId);
		Task<bool> DeleteLecturer(Guid lecturerId);
		Task<InstitutionDTO> GetAdminDetails(Guid institutionId);
		Task<bool> UpsertInstitution(InstitutionDTO institutionDTO, bool editInstitution);
		Task<IEnumerable<LecturerDTO>> GetLecturers(Guid userId);
		Task<IEnumerable<RoleType>> GetRoleType();
	}
	public class AdminService : PageModel, IAdminService
	{
		private readonly SocioleeDesignContext _db;
		private readonly IConfiguration _configuration;
		private readonly IJwtUtils _jwtUtils;
		private readonly IRegisterAccountService _registerAccountService;
		IHttpContextAccessor _httpContextAccessor;
		private readonly IUniqueIds _uniqueIds;
		private readonly IAuthService _authService;

		public AdminService(SocioleeDesignContext dbContext, IConfiguration configuration, IJwtUtils jwtUtils, IRegisterAccountService registerAccountService, IHttpContextAccessor httpContextAccessor, IUniqueIds uniqueIds, IAuthService authService)
		{
			_db = dbContext;
			_configuration = configuration;
			_jwtUtils = jwtUtils;
			_registerAccountService = registerAccountService;
			_httpContextAccessor = httpContextAccessor;
			_uniqueIds = uniqueIds;
			_authService = authService;
		}

		public async Task<InstitutionDTO> GetAdminDetails(Guid institutionId)
		{
			var institution = await _db.Institutions
				.Include(x => x.InstitutionCourses)
				.Select(x => new InstitutionDTO
				{
					Id = x.Id,
					Name = x.Name,
					Address = x.Address,
					PhoneNumber = x.PhoneNumber,
					Courses = x.InstitutionCourses.Select(c => c.Course).ToList()
				})
				.FirstOrDefaultAsync(x => x.Id == institutionId);

			if (institution != null)
			{
				return institution;
			}
			else
			{
				throw new Exception("Can't find student name");
			}
		}

		public async Task<bool> UpsertInstitution(InstitutionDTO institutionDTO, bool editInstitution)
		{
			if (institutionDTO.Id != null && editInstitution == true)
			{
				var existingInstitution = await _db.Institutions.FirstOrDefaultAsync(x => x.Id == institutionDTO.Id)
					?? throw new Exception("Could not locate student.Please try again");
				existingInstitution.Name = institutionDTO.Name;
				existingInstitution.Address = institutionDTO.Address;
				existingInstitution.Email = institutionDTO.Email;
				existingInstitution.PhoneNumber = institutionDTO.PhoneNumber;

				// Get a list of existing course names for the given institution
				var existingCourses = await _db.InstitutionCourses
					.Where(x => x.InstitutionId == institutionDTO.Id)
					.Select(x => x.Course) // Select only the course names
					.ToListAsync();

				// Filter only the courses that are NOT already in the database
				var newCourses = institutionDTO.Courses
					.Where(course => !existingCourses.Contains(course)) // Check if course is not in existing list
					.ToList();

				// Add only the new courses
				foreach (var course in newCourses)
				{
					var institutionCourse = new InstitutionCourse
					{
						Id = Guid.NewGuid(),
						InstitutionId = (Guid)institutionDTO.Id,
						Course = course,
					};
					_db.InstitutionCourses.Add(institutionCourse);
				}
			}
			else
			{
				var existingInstitution = await _db.Institutions.FirstOrDefaultAsync(x => x.Id == institutionDTO.Id);

				if (existingInstitution != null && editInstitution == false)
				{
					throw new Exception("This institution exists with the same Name");
				}

				var institution = new Institution
				{
					Id = Guid.NewGuid(),
					Name = institutionDTO.Name,
					Address = institutionDTO.Address,
					Email = institutionDTO.Email,
					PhoneNumber = institutionDTO.PhoneNumber
				};

				await _db.Institutions.AddAsync(institution);

				foreach (var course in institutionDTO.Courses)
				{
					var institutionCourse = new InstitutionCourse
					{
						Id = Guid.NewGuid(),
						InstitutionId = (Guid)institutionDTO.Id,
						Course = course,
					};
					_db.InstitutionCourses.Add(institutionCourse);
				}
			}

			await _db.SaveChangesAsync();

			return true;
		}

		public async Task<IEnumerable<Course>> GetCourses(Guid institutionId)
		{
			var courses = await _db.InstitutionCourses.Where(x => x.InstitutionId == institutionId)
				.Select(x => new Course { Id = x.Id, Name = x.Course })
				.ToListAsync();
			return courses;
		}

		public async Task<bool> UpsertLecturer(UpsertLecturerDTO lecturerDTO, bool editLecturer)
		{
			if (lecturerDTO.Id != null && editLecturer == true)
			{
				var existingLecturer = await _db.InstitutionLecturers.Include(x => x.User).ThenInclude(x => x.Role).FirstOrDefaultAsync(x => x.Id == lecturerDTO.Id)
					?? throw new Exception("Could not locate student.Please try again");
				existingLecturer.User.FullName = lecturerDTO.Name;
				existingLecturer.Years = lecturerDTO.Years;
				existingLecturer.User.Email = lecturerDTO.Email;
				existingLecturer.User.Role.Id = lecturerDTO.Role.Id;

				var existingCourses = await _db.LecturerCourses.Where(x => x.LecturerId == lecturerDTO.Id).ToListAsync();

				_db.LecturerCourses.RemoveRange(existingCourses);
				await _db.SaveChangesAsync();
				foreach (var course in lecturerDTO.Courses)
				{
					var studentCourse = new LecturerCourse
					{
						Id = Guid.NewGuid(),
						LecturerId = (Guid)lecturerDTO.Id,
						InstitutionCoursesId = course,
					};
					_db.LecturerCourses.Add(studentCourse);
				}
			}
			else
			{
				var existingStudent = await _db.Users.FirstOrDefaultAsync(x => x.Email == lecturerDTO.Email);

				var institution = await _db.Institutions.FirstOrDefaultAsync(x => x.Id == lecturerDTO.InstitutionId) ?? throw new Exception("Your institution can not be found.");

				var userId = await _uniqueIds.UniqueUserId();
				if (existingStudent == null && editLecturer == false)
				{
					var user = new UserSignUpRequest(
						userId,
						institution.Name,
						lecturerDTO.Name,
						lecturerDTO.Email,
						lecturerDTO.Role.Id,
						$"Pass_{CommonFunctions.GenerateRandomCode(6)}");

					await _authService.SignUp(user);
				}
				else if (existingStudent != null && editLecturer == false)
				{
					userId = existingStudent.Id;
				}

				var lecturer = new InstitutionLecturer
				{
					Id = Guid.NewGuid(),
					UserId = userId,
					InstitutionId = lecturerDTO.InstitutionId,
					Years = lecturerDTO.Years,
				};

				await _db.InstitutionLecturers.AddAsync(lecturer);

				var newUser = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId) ?? throw new Exception("Error creating Lecturer"); ;

				foreach (var course in lecturerDTO.Courses)
				{
					var lecturerCourse = new LecturerCourse
					{
						Id = Guid.NewGuid(),
						LecturerId = lecturer.Id,
						InstitutionCoursesId = course,
					};
					_db.LecturerCourses.Add(lecturerCourse);
				}
			}

			await _db.SaveChangesAsync();

			return true;
		}

		public async Task<bool> DeleteLecturer(Guid lecturerId)
		{
			var existingStudent = await _db.Students.FirstOrDefaultAsync(x => x.Id == lecturerId) ?? throw new Exception("Could not locate student.Please try again");

			_db.Students.Remove(existingStudent);
			await _db.SaveChangesAsync();

			return true;
		}

		public async Task<IEnumerable<LecturerDTO>> GetLecturers(Guid institutionId)
		{
			var lecturers = await _db.InstitutionLecturers
				.Include(u => u.LecturerCourses)
						.ThenInclude(sc => sc.InstitutionCourses)
				.Include(x => x.User)
					.ThenInclude(x => x.Role)
				.Select(s => new LecturerDTO(
					s.User,
					s.User.Role,
					s,
					s.LecturerCourses.Select(sc => new Course
					{
						Id = sc.InstitutionCourses.Id,
						Name = sc.InstitutionCourses.Course
					}).ToList()
				))
				.ToListAsync();

			return lecturers;
		}

		public async Task<IEnumerable<RoleType>> GetRoleType()
		{
			return await _db.RoleTypes.ToListAsync();
		}
	}
}

