using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using WebProject.Data;
using WebProject.Models;

namespace WebProject.Controllers
{
	public class HomeController : Controller
	{

		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IEmailSender _emailSender;
		public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
							IEmailSender emailSender
			
			)
		{
			_context = context;
			_userManager = userManager;
			_emailSender = emailSender;
		}
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public IActionResult Contact()
		{


			return View();
		}
		[HttpPost]
		public IActionResult Contact(ContactModel model)
		{
			string titleWithName = $"You have received a Meesage From{model.Name}";
			string contentWithEmail = $"this is Message from ${model.Email}:and its title is {model.Title},here is the content of message:\n{model.Content}";
			_emailSender.SendEmailAsync("398212699@qq.com", titleWithName, contentWithEmail);


			return View();
		}
		public IActionResult Login()
		{
			return View();
		}
		public IActionResult Profile()
		{
			return View();
		}


		[HttpPost]
		public async Task<string> CreateLogTest(CreateLogModel model)
		{
			if (!ModelState.IsValid)
			{
				return "failed";
			}
			if (model.Title == "")
			{
				return "please enter a title!";
			}

			var user = await _userManager.GetUserAsync(User);
			if (user==null || user.Id != model.UserId)
			{
				return "failed,Invalid attempt,you are not a Valid User";
			}

			//make sure,there is no same name Log for this User
			var queryOfLog = _context.Logs.Where(q => q.BelongerId == model.UserId && q.Title == model.Title);
			if (!queryOfLog.Any())
			{
				//Create the Log;
				var log = new UserLog() { BelongerId = model.UserId, Title = model.Title };

				_context.Logs.Add(log);
				_context.SaveChanges();
				return "finished";
			}

			return "there is a same named log in your list,please create this log by another log name";



			
		}
		[HttpPost]
		public string EditLogTest(EditLogModel model)
		{
			if (!ModelState.IsValid)
			{
				return "failded";
			}
			var queryOfLog = _context.Logs.Where(l => l.Id == model.Id);
			
			if (!queryOfLog.Any())
			{
				return "failed";
			}
			var log = queryOfLog.First();
			log.Content = model.Content;
			log.Title = model.Title;
			//_context.Logs.Remove(log);


			_context.SaveChanges();



			return "finished";
		}
		//this is test action designed for linux
		[HttpGet]
		public string GetEnvironment()
		{
			return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
		}
	}
}
