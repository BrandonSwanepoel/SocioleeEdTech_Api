using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using PayFast.AspNet;
using RazorHtmlEmails.Common;
using RazorHtmlEmails.RazorClassLib.Services;
using SocioleeMarkingApi.Authorization;
using SocioleeMarkingApi.Common;
using SocioleeMarkingApi.Services;
using SocioleeMarkingApi.Models.Database;

namespace SocioleeMarkingApi.Helper
{
	public static class StartupHelpers
    {

        public static void AddCorsHelper(this IServiceCollection services)
        {
            services.AddCors(p => p.AddPolicy(name: "SocioleeMarkingOrigin", services =>
            {
                services.WithOrigins("capacitor://localhost", "http://localhost", "https://sociolee-design.web.app", "http://localhost:8100", "http://127.0.0.1:4040", "http://192.168.1.103:8100", "http://localhost:4200", "http://localhost:8100", "http://localhost:8101", "https://sandbox.payfast.co.za", "http://127.0.0.1:8080", "https://SocioleeMarking-client.azurewebsites.net", "http://localhost:47694", "http://www.payfast.co.za", "http://w1w.payfast.co.za",
					"http://w2w.payfast.co.za", "https://SocioleeMarkingclient--sociolee-design.us-central1.hosted.app",

					"https://SocioleeMarking.com", "http://localhost:47694/chatHub").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
			}));
		}

        public static void AddAuthenticationHelper(this IServiceCollection services, string? token)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(token)),
                    ValidateIssuer = false,
                    ValidateAudience = false

                };
            });
        }

        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<SocioleeDesignContext>(options =>
            {
                var connectionstring = configuration.GetConnectionString("SQLAZURECONNSTR_DefaultConnection");
				options.UseSqlServer(connectionstring);
            });

            services.AddScoped<SocioleeDesignContext, SocioleeDesignContext>(provider => new SocioleeDesignContext(provider.GetService<DbContextOptions<SocioleeDesignContext>>()));

			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IJwtUtils, JwtUtils>();
			services.AddScoped<IRegisterAccountService, RegisterAccountService>();
			services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddScoped<IBlobStorageService, BlobStorageService>();
			services.AddScoped<IPaymentService, PaymentService>();
			services.AddScoped<IContentService, ContentService>();
			services.AddScoped<IUniqueIds, UniqueIds>();
			services.AddScoped<IEmailTrackingService, EmailTrackingService>();
			services.AddScoped<IMessagingService, MessagingService>();
			services.AddScoped<PayFastNotifyModelBinder>();
			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddRazorPages();

		}
	}
}

