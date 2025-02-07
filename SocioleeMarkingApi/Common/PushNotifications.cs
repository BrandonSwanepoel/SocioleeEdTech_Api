using System;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace SocioleeMarkingApi.Common
{
	public static class PushNotifications
	{
		public async static Task SendPushNotification()
		{
			var message = new Message()
			{
				//Data = new Dictionary<string, string>()
				//{
				//	{ "myData", "1337" },
				//},
				//Token = "ehgi75cRTQ6YnCrWQdiDhU:APA91bH9p2lneh9Z58XOAdPbCmEMGtte3zVzFzM4q12lzlhwAY8QI7onMfLiU-74BVOt3EFl1vYulil_yhHzBeiwuxQjc8YKXhOUJWYbngS7pkvA9_vCd1SIM2Z1iNbVtaSeWwbqpwf-",
				Topic = "all",
				Notification = new Notification()
				{
					Title = "New Competition my bra!",
					Body = "Subscribe now to be part of this and other insane prizes.",
				}
			};

			// Send a message to the device corresponding to the provided
			// registration token.
			string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
			// Response is a message ID string.
			Console.WriteLine("Successfully sent message: " + response);
		}
	}
}

