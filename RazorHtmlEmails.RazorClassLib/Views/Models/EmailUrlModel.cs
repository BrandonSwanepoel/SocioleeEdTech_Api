namespace RazorHtmlEmails.RazorClassLib.Views.Models;

public record UrlViewModel(string url, string username);
public record StudentSignUpModel(string url, string username,string email, string password, string institionName);
public record StepUpdatedModel(string Username, string EmailText, string AssetUrl);
public record AccountPaymentModel(string Username, string Amount, string Reference);
public record StepUpdatedModelDesigner(string Username, string EmailText,string DesignUrl);
public record UnlistedBusinessReviewCreateProfileViewModel(string url,string businessName, string reviewMessage, string username);
public record SubscriptionTypeModel(string subscriptionType);
public record NewBusinessReviewViewModel(string unsubscribeUrl, string url, string subscriptionType);
public record ReplyOnReviewViewModel(string unsubscribeUrl, string businessName, string url, string subscriptionType);
public record ReportReviewViewModel(Guid id, Guid reviewId, Guid userId, string reason);
public record UnlistedBusinessReviewViewModel(Guid businessId, Guid reviewId, Guid reviewerUserId);
public record SubscriptionExpiryReminderViewModel(string expiryDate);