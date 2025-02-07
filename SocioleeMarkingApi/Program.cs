global using SocioleeMarkingApi.Models;
using System.Text.Json.Serialization;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SocioleeMarkingApi.Common;
using SocioleeMarkingApi.Helper;
using SocioleeMarkingApi.Middleware;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.Configuration.AddJsonFile("appsettings.json", false, true);
builder.Configuration.AddJsonFile($"appsettings.{environmentName}.json", true, true);
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oath2", new OpenApiSecurityScheme
    {
        Description="Standard Authorization header using the Bearer scheme(\"bearer{token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetConnectionString("BlobStorageConnection")));


    builder.Services.AddAuthenticationHelper(builder.Configuration.GetValue<string>("Token"));


builder.Services.AddCorsHelper();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddControllers().AddJsonOptions(options =>
   options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
	options.SuppressModelStateInvalidFilter = true;
});


builder.Services.Configure<FormOptions>(x =>
{
	x.ValueLengthLimit = int.MaxValue;
	x.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
	x.MultipartHeadersLengthLimit = int.MaxValue;
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
	options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("SocioleeMarkingOrigin");

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("chatHub").RequireAuthorization();
app.Run();

