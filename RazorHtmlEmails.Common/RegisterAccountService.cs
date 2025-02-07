using System.Security.Cryptography.Xml;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using RazorHtmlEmails.RazorClassLib.Services;
using RazorHtmlEmails.RazorClassLib.Views.Models;

namespace RazorHtmlEmails.Common;

public interface IRegisterAccountService
{
	Task Register(string fullName, string email, string baseUrl);
	Task StudentRegister(string fullName, string email, string baseUrl, string institionName, string password);
	Task ChangeEmail(string fullName, string email, string baseUrl);
	Task StepUpdated(string fullName, string toAddresses, string stepUpdatedText, string imagePath);
	Task DesignerNotification(string fullName, string toAddresses, string notificationText, Guid designId);
	Task SubscriptionExpiryEmail(Guid userId, string fullName, string toAddresses, string expiryDate);
	string AccountConfirmed();
	Task ForgotPassword(string fullName, string toAddresses, string passwordResetToken);
	Task AccountPaymentDetails(string fullName, string toAddresses, string amount, string reference);
	string ResetPassword();
	string CreateBusinessAccount();
	string UnsubscribedPage();
}

public class RegisterAccountService : IRegisterAccountService
{
	private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
	private readonly IConfiguration _configuration;

	public RegisterAccountService(IRazorViewToStringRenderer razorViewToStringRenderer, IConfiguration configuration)
	{
		_razorViewToStringRenderer = razorViewToStringRenderer;
		_configuration = configuration;
	}

	public async Task SubscriptionExpiryEmail(Guid userId, string fullName, string toAddresses, string expiryDate)
	{
		var subscriptionExpiryReminderViewModel = new SubscriptionExpiryReminderViewModel(expiryDate);

		string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/Templates/SubscriptionExpiryEmail.cshtml", subscriptionExpiryReminderViewModel);

		await SendEmail(fullName, toAddresses, _configuration.GetSection("Email:Username").Value, "Subscription expiry reminder.", body);
	}

	public async Task Register(string fullName, string toAddresses, string verificationToken)
	{
		var urlViewModel = new UrlViewModel(verificationToken, fullName);

		string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/Templates/ConfirmAccountEmail.cshtml", urlViewModel);
		
		await SendEmail(fullName, toAddresses, _configuration.GetSection("Email:Username").Value, "Confirm your email address", body);
	}

	public async Task StudentRegister(string fullName, string toAddresses, string verificationToken,string institionName, string password)
	{
		var urlViewModel = new StudentSignUpModel(verificationToken, fullName, toAddresses, password, institionName);

		string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/Templates/StudentAutoSignUp.cshtml", urlViewModel);

		await SendEmail(fullName, toAddresses, _configuration.GetSection("Email:Username").Value, "Confirm your email address", body);
	}

	public async Task ChangeEmail(string fullName, string toAddresses, string verificationToken)
	{
		var urlViewModel = new UrlViewModel(verificationToken, fullName);

		string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/Templates/ConfirmChangeAccountEmail.cshtml", urlViewModel);

		await SendEmail(fullName, toAddresses, _configuration.GetSection("Email:Username").Value, "Confirm your email address", body);
	}

	public async Task StepUpdated(string fullName, string toAddresses, string stepUpdatedText,string imagePath)
	{
		var urlViewModel = new StepUpdatedModel(fullName,stepUpdatedText, imagePath);

		string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/Templates/DesignStepUpdated.cshtml", urlViewModel);

		await SendEmail(fullName, toAddresses, _configuration.GetSection("Email:Username").Value, "Design updated", body);
	}

	public async Task AccountPaymentDetails(string fullName, string toAddresses, string amount, string reference)
	{
		var urlViewModel = new AccountPaymentModel(fullName,amount, reference);

		string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/Templates/AccountPaymentDetails.cshtml", urlViewModel);

		await SendEmail(fullName, toAddresses, _configuration.GetSection("Email:Username").Value, "Payment Instructions", body);
	}

	public async Task DesignerNotification(string fullName, string toAddresses, string notificationText, Guid designId)
	{
		var designLink = $"https://SocioleeMarking.com//content-list/content/{designId}";
		var urlViewModel = new StepUpdatedModelDesigner(fullName, notificationText, designLink);

		string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/Templates/DesignStepUpdated_designer.cshtml", urlViewModel);

		await SendEmail(fullName, toAddresses, _configuration.GetSection("Email:Username").Value, "Design Alert", body);
	}

	public string CreateBusinessAccount()
	{
		return "/Views/Pages/CreateBusinessAccount.cshtml";
	}

	public string AccountConfirmed()
	{
		return "/Views/Pages/ConfirmedAccount.cshtml";
	}

	public string ResetPassword()
	{
		return "/Views/Pages/ConfirmedAccount.cshtml";
	}

	public string UnsubscribedPage()
	{
		return "/Views/Pages/Unsubscribed.cshtml";
	}

	public async Task ForgotPassword(string fullName, string toAddresses, string passwordResetToken)
	{
		var urlViewModel = new UrlViewModel(passwordResetToken, fullName);

		string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/Templates/ForgotPasswordEmail.cshtml", urlViewModel);

		await SendEmail(fullName, toAddresses, _configuration.GetSection("Email:Username").Value, "Forgot password.", body);
	}

	public async Task SendEmail(string fullname, string toAddresses, string fromAddress, string subjectInput, string body, string? plainText = null)
	{
		using var message = new MimeMessage();

		message.From.Add(new MailboxAddress(
			"Sociolee Design",
			fromAddress
		));

		message.To.Add(new MailboxAddress(
			 fullname,
			toAddresses
		));
		message.Subject = subjectInput;

		var bodyBuilder = new BodyBuilder
		{
			HtmlBody = body
		};
		
		message.Body = bodyBuilder.ToMessageBody();
		using var client = new MailKit.Net.Smtp.SmtpClient();
		// SecureSocketOptions.StartTls force a secure connection over TLS
		await client.ConnectAsync("smtp.sendgrid.net", 587, SecureSocketOptions.StartTls);
		await client.AuthenticateAsync(
			userName: "apikey", // the userName is the exact string "apikey" and not the API key itself.
			password: _configuration.GetValue<string>("SendGrid_ApiKey") // password is the API key
		);

		await client.SendAsync(message);
		await client.DisconnectAsync(true);

		//var apiKey = _configuration.GetValue<string>("SendGrid_ApiKey");
		//var client = new SendGridClient(apiKey);
		//var from = new EmailAddress(fromAddress, "SocioleeMarking");
		//var subject = subjectInput;
		//var to = new EmailAddress(toAddresses, fullname);
		//var htmlContent = body;
		//var msg = MailHelper.CreateSingleEmail(from, to, subject, plainText, htmlContent);
		//var response = await client.SendEmailAsync(msg);
	}

	
}
