using System.Security.Claims;
using System.Security.Cryptography;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using RazorHtmlEmails.Common;
using SendGrid;
using SendGrid.Helpers.Mail;
using SocioleeMarkingApi.Common;
using SocioleeMarkingApi.Models.Database;
using SocioleeMarkingApi.Models.DTOs;
using SocioleeMarkingApi.Models.UserModels;

namespace SocioleeMarkingApi.Services
{
	public interface IUserService
	{
		Guid GetUserId();
		Task<bool> ContactUs(ContactUsDto contactUs);
		Task<bool> CheckIfEmailExists(string email);
		Task<bool> AddPaymentRequest(Guid userId, int amount);
		Task<UserDetails> GetUserDetails(Guid userId);
		Task<bool> UserHasCoupon(Guid userId);
		Task<int> UserDesignsLeft(Guid userId);
		Task<int> GetProjectCount(Guid institutionId);
		Task<int> GetStudentCount(Guid institutionId);
		Task<bool> UpdateUserDetails(UserDto user);
		Task<bool> UpdateUserEmail(UserDto user);
		Task<Guid?> GetAsset(string type, Guid? userId);
		Task<bool> AddUserAsset(Guid userId, Guid assetId, string type);
		Task RemoveUserAsset(Guid userId, string type);

		//Student
		Task<bool> UpsertStudent(UpsertStudentDTO studentDTO, bool editStudent);
		Task<IEnumerable<Programme>> GetProgrammes(Guid institutionId);
		Task<bool> DeleteStudent(Guid studentId);
		Task<StudentDTO> GetStudentDetails(Guid studentUserId, bool fromStudentUserId);
		Task<UserAnalytics> UserAnalytics(Guid studentId);
	}
    public class UserService : IUserService
    {
		private readonly BlobServiceClient _blobServiceClient;
		private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SocioleeDesignContext _db;
        private readonly IRegisterAccountService _registerAccountService;
		private readonly IConfiguration _configuration;
		private readonly IUniqueIds _uniqueIds;
		private readonly IAuthService _authService;
		public UserService(SocioleeDesignContext dbContext, IHttpContextAccessor httpContextAccessor, IRegisterAccountService registerAccountService, IConfiguration configuration, BlobServiceClient blobServiceClient, IUniqueIds uniqueIds, IAuthService authService)
        {
            _db = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _registerAccountService = registerAccountService;
			_configuration = configuration;
			_blobServiceClient = blobServiceClient;
			_uniqueIds = uniqueIds;
			_authService = authService;
		}

		public async Task<bool> ContactUs(ContactUsDto contactUs)
		{
			var apiKey = _configuration.GetValue<string>("SendGrid_ApiKey");
			var client = new SendGridClient(apiKey);
			var from = new EmailAddress(_configuration.GetSection("Email:Username").Value, "Sociolee Design Team");
			var subject = $"{contactUs.MessageTypeHeading}\n" +
				$"Name:{contactUs.Name}\n" +
				$"Email:{contactUs.Email}";
			var to = new EmailAddress(_configuration.GetSection("Email:Username").Value, "Sociolee Design Team");
			var msg = MailHelper.CreateSingleEmail(from, to, subject, contactUs.Message, "");
			var response = await client.SendEmailAsync(msg);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> CheckIfEmailExists(string email)
		{
			return await _db.Users.AnyAsync(x => x.Email == email);
		}

		public async Task<bool> AddPaymentRequest(Guid userId, int amount)
		{
			var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new HttpRequestException("User does not exist");

			var paymentRequest = await _db.PaymentRequests.Where(x => x.UserId == userId && x.ActivatedSubscription == false).FirstOrDefaultAsync();

			if (paymentRequest == null)
			{
				string amountAsString;
				string reference;

				if (amount == 6000)
				{
					var uniqueCode = CommonFunctions.GenerateRandomCode(6);
					reference = "Bas_" + uniqueCode;
					amountAsString = "R6,000";
				}
				else if (amount == 66000)
				{
					var uniqueCode = CommonFunctions.GenerateRandomCode(6);
					reference = "Bas_" + uniqueCode;
					amountAsString = "R66,000";
				}
				else if (amount == 11000)
				{
					var uniqueCode = CommonFunctions.GenerateRandomCode(6);
					reference = "Pro_" + uniqueCode;
					amountAsString = "R11,000";
				}
				else if (amount == 121000)
				{
					var uniqueCode = CommonFunctions.GenerateRandomCode(6);
					reference = "Pro_" + uniqueCode;
					amountAsString = "R121,000";
				}
				else
				{
					throw new Exception("Error Sending Payment Details.");
				}

				var newPaymentRequest = new PaymentRequest
				{
					Id = await _uniqueIds.UniquePaymentRequestUserId(),
					UserId = user.Id,
					PaymentReference = reference,
					ActivatedSubscription = false,
					Created = CommonFunctions.CurrentDateTime(),
					Amount = amount,
				};

				_db.PaymentRequests.Add(newPaymentRequest);
				await _db.SaveChangesAsync();

				await _registerAccountService.AccountPaymentDetails(user.FullName, user.Email, amountAsString, reference);
				return true;
			}
			return false;
		}

		//public async Task<bool> AlreadyAddedToWaitList(WaitListDTO waitListDto)
		//{
		//	var added = await _db.WaitLists.AnyAsync(x => x.Email == waitListDto.Email);
		//	return added;
		//}

		public Guid GetUserId()
		{
            var userId = Guid.Empty;
            if(_httpContextAccessor.HttpContext != null)
			{
                var result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (result == null)
                    return Guid.Empty;

                userId = new Guid(result);
			}
			return userId;
		}

		public async Task<UserDetails> GetUserDetails(Guid userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new HttpRequestException("User does not exist");

			return new UserDetails(user.Id, user.FullName, user.Email, user.RoleId, null, user.IsAdmin);
        }

		public async Task<bool> UserHasCoupon(Guid userId)
		{
			var hasCoupon = await _db.Coupons
							.AnyAsync(x => x.UserId == userId && x.Redeemed == false);

			return hasCoupon;
		}

		public async Task<int> UserDesignsLeft(Guid userId)
		{
			int count = 0;
			var paymentDetails = await _db.PaymentDetails.Include(x => x.PaymentPackage).OrderByDescending(p => p.ExpiryDate).FirstOrDefaultAsync(x => x.UserId == userId);
			if (paymentDetails != null && paymentDetails.ExpiryDate > CommonFunctions.CurrentDateTime())
			{

				var contentFormCount = await _db.ContentForms
										.CountAsync(form =>
										form.UserId == userId &&
										form.Created >= paymentDetails.StartDate &&
										form.Created <= paymentDetails.ExpiryDate);

				count = paymentDetails.PaymentPackage.DesignAmount - contentFormCount;
			}

			var coupons = await _db.Coupons.CountAsync(x => x.UserId == userId && x.Redeemed == false);

			return count + coupons;
		}

		public async Task<int> GetProjectCount(Guid institutionId)
		{
			//return await _db.Students
			//						.CountAsync(form => form.UserId == userId);
			return await _db.StudentProjects
								.CountAsync(x => x.InstitutionId == institutionId);
		}

		public async Task<int> GetStudentCount(Guid institutionId)
		{
			//return await _db.Students
			//						.CountAsync(form => form.UserId == userId);
			return await _db.Students
								.CountAsync(x => x.InstitutionId == institutionId);
		}

		public async Task<bool> UpdateUserDetails(UserDto user)
		{
			var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == user.Id) ?? throw new HttpRequestException("User does not exist");
			existingUser.FullName = user.FullName;
			await _db.SaveChangesAsync();
			return true;
		}

		public async Task<bool> UpdateUserEmail(UserDto user)
		{
			var verificationToken = CommonFunctions.GenerateRandomCode(6);
			var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == user.Id) ?? throw new HttpRequestException("User does not exist");
			existingUser.Email = user.Email;
            existingUser.VerificationToken = verificationToken;
            existingUser.VerifiedAt = null;
			await _db.SaveChangesAsync();

            await _registerAccountService.ChangeEmail(existingUser.FullName, existingUser.Email, verificationToken);
			return true;
		}

		public async Task<Guid?> GetAsset(string type, Guid? userId)
		{
			var assetId = await _db.ContentAssets.Where(x => x.UserId == userId && x.Type == type).Select(x => x.Id).FirstOrDefaultAsync();
			return assetId;
		}

		public async Task<bool> AddUserAsset(Guid userId, Guid assetId, string type)
		{
			var asset = new ContentAsset
			{
				Id = assetId,
				UserId = userId,
				Type = type
			};

			_db.ContentAssets.Add(asset);
			await _db.SaveChangesAsync();
			return true;
		}

		public async Task RemoveUserAsset(Guid userId, string type)
		{
			var oldAssets = await _db.ContentAssets.Where(x => x.UserId == userId && x.Type == type).ToListAsync();
			_db.ContentAssets.RemoveRange(oldAssets);

			foreach (var oldAsset in oldAssets)
			{
				var path = @$"https://socioleeblobstorage.blob.core.windows.net/userprofilepicture/{oldAsset.Id}";
				var imageDetails = new Uri(path).Segments;
				var client = _blobServiceClient.GetBlobContainerClient(imageDetails[1]);

				var blobClient = client.GetBlobClient(imageDetails[2]);
				await blobClient.DeleteIfExistsAsync();
			}

			await _db.SaveChangesAsync();
		}

		public async Task<StudentDTO> GetStudentDetails(Guid studentUserId, bool fromStudentUserId)
		{
			IQueryable<Student> studentQueryable;

			if (fromStudentUserId)
			{
				studentQueryable = _db.Users
					.Where(x => x.Id == studentUserId)
					.SelectMany(x => x.Students) // Flatten the Students collection
					.AsQueryable();
			}
			else
			{
				studentQueryable = _db.Students
					.Where(x => x.Id == studentUserId)
					.AsQueryable();
			}

			var students = await studentQueryable
				.Include(u => u.StudentProgrammes)
						.ThenInclude(sc => sc.InstitutionProgramme)
				.Include(x => x.User)
				.Select(s => new Models.StudentDTO(
					s.User,
					s,
					s.StudentProgrammes.Select(sc => new Programme
					{
						Id = sc.InstitutionProgramme.Id,
						Name = sc.InstitutionProgramme.Programme
					}).ToList()
				))
				.FirstOrDefaultAsync();
			if(students != null)
			{
				return students;
			}
			else
			{
				throw new Exception("Can't find student name");
			}
		}

		public async Task<IEnumerable<Programme>> GetProgrammes(Guid institutionId)
		{
			var Programmes = await _db.InstitutionProgrammes.Where(x => x.InstitutionId == institutionId)
				.Select(x => new Programme{ Id = x.Id, Name = x.Programme})
				.ToListAsync();
			return Programmes;
		}

		public async Task<bool> UpsertStudent(UpsertStudentDTO studentDTO, bool editStudent)
		{
			if (studentDTO.Id != null && editStudent == true)
			{
				var existingStudent = await _db.Students.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == studentDTO.Id)
					?? throw new Exception("Could not locate student.Please try again");
				existingStudent.User.FullName = studentDTO.Name;
				existingStudent.Year = studentDTO.Year;
				existingStudent.User.Email = studentDTO.Email;

				var existingProgrammes = await _db.StudentProgrammes.Where(x => x.StudentId == studentDTO.Id).ToListAsync();

				_db.StudentProgrammes.RemoveRange(existingProgrammes);
				await _db.SaveChangesAsync() ;
				foreach (var Programme in studentDTO.Programme)
				{
					var studentProgramme = new StudentProgramme
					{
						Id = Guid.NewGuid(),
						StudentId = (Guid)studentDTO.Id,
						InstitutionProgrammeId = Programme,
					};
					_db.StudentProgrammes.Add(studentProgramme);
				}
			}
			else
			{
				var existingStudent = await _db.Users.FirstOrDefaultAsync(x => x.Email == studentDTO.Email);

				var institution = await _db.Institutions.FirstOrDefaultAsync(x => x.Id == studentDTO.InstitutionId);

				if (institution == null)
				{
					throw new Exception("Your institution can not be found.");
				}


				var userId = await _uniqueIds.UniqueUserId();
				if (existingStudent == null && editStudent == false)
				{
					var user = new UserSignUpRequest(
						userId,
						institution.Name,
						studentDTO.Name,
						studentDTO.Email,
						Guid.Parse("101f13ab-f7eb-4a13-8fab-8d728db09329"),
						$"Pass_{CommonFunctions.GenerateRandomCode(6)}");

					await _authService.SignUp(user);
				}
				else if (existingStudent != null && editStudent == false)
				{
					userId = existingStudent.Id;
				}
				var studentExists = await _db.Students.AnyAsync(x => x.UserId == userId);
				if(studentExists == true)
				{
					throw new Exception("Student already exists.");
				}

				var student = new Student
				{
					Id = Guid.NewGuid(),
					UserId = userId,
					InstitutionId = studentDTO.InstitutionId,
					Year = studentDTO.Year,
				};

				await _db.Students.AddAsync(student);

				foreach (var Programme in studentDTO.Programme)
				{
					var studentProgramme = new StudentProgramme
					{
						Id = Guid.NewGuid(),
						StudentId = student.Id,
						InstitutionProgrammeId = Programme,
					};
					_db.StudentProgrammes.Add(studentProgramme);
				}
			}
			
			await _db.SaveChangesAsync();

			return true;
		}

		public async Task<bool> DeleteStudent(Guid studentId)
		{
			var existingStudent = await _db.Students.FirstOrDefaultAsync(x => x.Id == studentId) ?? throw new Exception("Could not locate student.Please try again");

			_db.Students.Remove(existingStudent);
			await _db.SaveChangesAsync();

			return true;
		}

		public async Task<UserAnalytics> UserAnalytics(Guid studentId)
		{
			var studentProgrammes = await _db.StudentProgrammes
			.Include(x => x.InstitutionProgramme)
			.Include(x => x.Student)
				.ThenInclude(x => x.ProjectRubricStudentMarks)
				.ThenInclude(x => x.Project)
			.Where(x => x.StudentId == studentId)
			.ToListAsync();

			var analytics = new UserAnalytics();
			double? totalGradeSum = 0;
			int totalProjects = 0;

			foreach (var item in studentProgrammes)
			{
				var programmeMarks = item.Student.ProjectRubricStudentMarks
					.Where(mark => mark.Project.InstitutionProgrammeId == item.InstitutionProgramme.Id)
					.Select(mark => mark.Mark)
					.ToList();

				if (programmeMarks.Any())
				{
					var ProgrammeAnalytics = new UserProgrammeAnalytics
					{
						ProgrammeName = item.InstitutionProgramme.Programme,
						AverageGrade = programmeMarks.Average()
					};

					analytics.UserProgrammeAnalytics.Add(ProgrammeAnalytics);

					totalGradeSum += programmeMarks.Sum();
					totalProjects += programmeMarks.Count;
				}
				else
				{
					var ProgrammeAnalytics = new UserProgrammeAnalytics
					{
						ProgrammeName = item.InstitutionProgramme.Programme,
						AverageGrade = 0
					};

					analytics.UserProgrammeAnalytics.Add(ProgrammeAnalytics);
				}
			}

			// Compute overall average
			analytics.GradeAve = totalProjects > 0 ? (double)(totalGradeSum / totalProjects) : 0;

			return analytics;
		}
	}
}

