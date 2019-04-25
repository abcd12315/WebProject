using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Email
{
	public class EmailSender:IEmailSender
	{
	
			public Task SendEmailAsync(string email, string subject, string message)
			{
				var emailSender = new Xxy.EmailSend.EmailSender("398212699@qq.com", "wtxsoistrdnfbjjd");
				var emailReceiver = new Xxy.EmailSend.EmailReceiver(email);
				var emailToSend = new Xxy.EmailSend.Email(subject, message);
				emailSender.SendEmail(emailReceiver, emailToSend);

				return Task.CompletedTask;
			}
		
	}
}
