using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebProject.Data;
using WebProject.Models;

namespace WebProject.Controllers {
	[Authorize]
	public class UserController : Controller {

		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IHostingEnvironment _hostingEnvironment;
		private readonly IConfiguration _configuration;
		public UserController (ApplicationDbContext context, 
			UserManager<ApplicationUser> userManager,
			IConfiguration configuration,
			IHostingEnvironment hostingEnvironment
		) {
			_context = context;
			_userManager = userManager;
			_hostingEnvironment = hostingEnvironment;
			_configuration = configuration;
		}
		public IActionResult Index () {
			return View ();
		}

		[HttpGet]
		public async Task<IActionResult> Log () {

			//get all the logs that current user has;
			var user = await _userManager.GetUserAsync (User);
			var logs = _context.Logs.Where(q => q.BelongerId == user.Id).OrderBy(q=>q.RegistrationTime);//按照时间排序
	
			//if (logs == null)
			//{
			//	logs = new IQueryable<UserLog>();
			//}
			
			return View(logs);
		}

		[HttpGet]
		public async Task<IActionResult> AddLog (string returnUrl = null) {
			ViewData["ReturnUrl"] = returnUrl;
			var user = await _userManager.GetUserAsync (User);

			var model = new AddLogModel () { BelongerId = user.Id };

			return View (model);
		}

		[HttpPost]
		public async Task<IActionResult> AddLog (AddLogModel model, string returnUrl = null) {
			ViewData["ReturnUrl"] = returnUrl;
			if (ModelState.IsValid) {

				var user = await _userManager.GetUserAsync (User);
				if (user == null || user.Id != model.BelongerId) {
					return View (model); //这里没有错误信息。
				}
				//看有没有重名的log
				var queryLog = _context.Logs.Where (q => q.BelongerId == model.BelongerId && q.Title == model.Title);
				if (queryLog.Any ()) //有重名的log
				{
					return View (model); //这里也没有错误信息。
				}

				//Add a Log
				_context.Logs.Add (new UserLog () { Title = model.Title, BelongerId = model.BelongerId,RegistrationTime=DateTime.Now});
				_context.SaveChanges ();

				return Redirect (returnUrl); //成功返回

			}
			return View (model);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteLog (DeleteLogModel model) {
			//只有当这个Log存在,并且属于当前用户时才能删除
			if (ModelState.IsValid) {
				var user = await _userManager.GetUserAsync (User);
				if (user == null) {
					throw new Exception ("error"); //能到这里,user不可能不存在吧?
				}
				var query = _context.Logs.Where (q => q.Id == model.Id && q.BelongerId == user.Id);
				if (query.Any ()) {
					var log = query.First ();
					_context.Remove (log);
					_context.SaveChanges ();
					return RedirectToAction ("Log");

				}

				throw new Exception ("unable to find such log needed to be deleted ");

			}
			throw new Exception ("error");

		}

		[HttpGet]
		public async Task<IActionResult> EditLog (string logId, string returnUrl = null) {

			if (ModelState.IsValid) {
				ViewData["ReturnUrl"] = returnUrl;
				//如果这个Log存在,并且属于当前用户
				var user = await _userManager.GetUserAsync (User);
				var query = _context.Logs.Where (q => q.Id == logId && q.BelongerId == user.Id);
				if (query.Any ()) {
					var log = query.First ();
					var model = new EditLogModel () { Id = log.Id, Content = log.Content, Title = log.Title };
					return View (model);
				}
				throw new Exception ("error");

			}

			throw new Exception ("error");

		}

		[HttpPost]
		public async Task<IActionResult> EditLog (EditLogModel model, string returnUrl) {
			if (ModelState.IsValid) {
				ViewData["ReturnUrl"] = returnUrl;
				//如果这个Log存在,并且属于当前用户
				var user = await _userManager.GetUserAsync (User);
				var query = _context.Logs.Where (q => q.Id == model.Id && q.BelongerId == user.Id);
				if (query.Any ()) {
					var log = query.First ();
					log.Title = model.Title;
					log.Content = model.Content;
					_context.SaveChanges ();

					return Redirect (returnUrl);
				}
				return View (model); //没有添加出错信息

			}

			return View (model); //没有添加出错信息

		}

		[HttpGet]
		public async Task<IActionResult> EditProfilePhoto () {

			var user = await _userManager.GetUserAsync (User);

			return View (user);
		}
		//[HttpPost]
		//public async Task<IActionResult> EditProfilePhoto()
		//{

		//}
		[HttpPost]
		public async Task<IActionResult> EditProfilePhoto (List<IFormFile> Photos) {
			var file = Photos[0]; //I don't know how to work it out without using "List"
			Console.WriteLine (file.Name);

			string fileName = file.FileName;
			string Path = _hostingEnvironment.WebRootPath + @"/images" + "/ProfilePhotos" + $@"/{fileName}";
			using (FileStream MyFileStream = System.IO.File.Create (Path)) {
				file.CopyTo (MyFileStream);
				MyFileStream.Flush ();
			}
			//then modify the data(Profile Photo Path of User) of database
			var user = await _userManager.GetUserAsync (User);
			user.ProfilePhotoPath = $"/images/ProfilePhotos/{fileName}";
			_context.SaveChanges ();
			return View (user);
		}

		public async Task<IActionResult> Album () {
			//show 当前用户所拥有的所有相册,并展示
			var user = await _userManager.GetUserAsync (User);
			var albums = _context.Albums.Where (q => q.BelongerId == user.Id);

			return View (albums);
		}

		[HttpGet]
		public IActionResult AddAlbum (string returnUrl = null) {
			ViewData["ReturnUrl"] = returnUrl;

			return View ();
		}

		[HttpPost]
		public async Task<IActionResult> AddAlbum (AddAlbumModel model, string returnUrl = null) {

			if (string.IsNullOrEmpty (model.Name)) //这里应该是要前端检查的。
			{
				return View (model);
			}

			if (ModelState.IsValid) {
				ViewData["ReturnUrl"] = returnUrl;
				var user = await _userManager.GetUserAsync (User);
				if (user == null) {
					return View (model); //失败,因为用户不存在

				}
				//查看是否有重名Album
				var query = _context.Albums.Where (q => q.BelongerId == user.Id && q.Name == model.Name);
				if (query.Any ()) {
					return View (model); //有重名,失败
				}

				//创建album

				var album = new UserAlbum () { Name = model.Name, BelongerId = user.Id };
				_context.Albums.Add (album);
				_context.SaveChanges ();

				//为该相册创建目录
				var webRootPath = _hostingEnvironment.WebRootPath;
				var albumPath = $"{webRootPath}/images/Albums/{album.Id}";
				Directory.CreateDirectory (albumPath);

				album.AlbumPath = $"/images/Albums/{album.Id}";
				_context.SaveChanges (); //之所以访问两遍数据库时,是因为第一次访问数据库后,album.Id才能确定。
				return Redirect (returnUrl);

			}

			return View (model); //失败
		}

		public async Task<IActionResult> DeleteAlbum (DeleteAlbumModel model) {
			if (ModelState.IsValid) {
				var query = _context.Albums.Where (q => q.Id == model.Id);
				if (query.Any ()) {
					var album = query.First ();

					var user = await _userManager.GetUserAsync (User);

					if (album.BelongerId == user.Id) {
						_context.Albums.Remove (album);
						_context.SaveChanges ();

						var webRootPath = _hostingEnvironment.WebRootPath;
						var albumPath = $"{webRootPath}/images/Albums/{model.Id}";
						if (Directory.Exists (albumPath)) {
							Directory.Delete (albumPath, true);
						}
						return RedirectToAction ("Album");
					}

					throw new Exception ("invalid action,you are not the belonger of this album");
				}

				throw new Exception ("the album doesn't exist!");

			}

			throw new Exception ("error");
		}

		[HttpGet]
		public IActionResult ShowAlbum (string albumId, string returnUrl = null) {
			ViewData["AlbumId"] = albumId;
			ViewData["returnUrl"] = returnUrl;

			var model = new ShowAlbumModel ();
			var query = _context.Albums.Where (q => q.Id == albumId);
			if (query.Any ()) {
				var album = query.First ();
				model.Path = album.AlbumPath;
			}
			return View (model);
		}

		[HttpPost]
		public IActionResult ShowAlbum (ShowAlbumModel model, int Page) {
			if (string.IsNullOrEmpty (model.Path)) {
				return Json (new { });
			}
			var pictureList = new List<string> ();
			var albumPathTemp = model.Path;
			var rootPath = _hostingEnvironment.WebRootPath;
			var albumPath = $"{rootPath}/{albumPathTemp}";



			//先得到albumPath下的所有图片
			var allPicturesUnordered = Directory.GetFiles (albumPath);

			//按照上传时间排序
			var allPictures=allPicturesUnordered.OrderBy(q => new FileInfo(q).CreationTime).ToList();

			int countOfPhotos = GetCountOfPhotos();
					 
			ViewData["CountOfPhotos"] = countOfPhotos.ToString();
			//取第page*30+30张图片
			for (int i = (Page - 1) * countOfPhotos; i < Page * countOfPhotos && i < allPictures.Count(); i++) {
				allPictures[i]=allPictures[i].Replace('\\', '/');
				var index = allPictures[i].LastIndexOf ('/') + 1;
				var pictureName = allPictures[i].Substring (index);
				pictureList.Add (pictureName);
			}

			return Json (new { pictures = pictureList, count = allPictures.Count () });
		}

		[HttpPost]
		public async Task ChangeFrontPhoto (string path, string frontPicturePath) {

			//Path为相册的路径,这个也能当主键
			if (ModelState.IsValid) {
				var query = _context.Albums.Where (q => q.AlbumPath == path);
				if (query.Any ()) {
					var album = query.First ();
					var user = await _userManager.GetUserAsync (User);
					if (user.Id == album.BelongerId) {
						if (string.IsNullOrEmpty (frontPicturePath)) {
							return;
						}
						//检查frontPicturePath是否存在

						//https://localhost:44398/images/Albums/51f7c92b-666d-4f21-8929-c7fbbc052a39/MFC%20(3).jpg
						//get /images/Albums/...
						var index = frontPicturePath.IndexOf ("/images/Albums");
						frontPicturePath = frontPicturePath.Substring (index);
						album.FrontPicturePath = frontPicturePath;
						_context.SaveChanges ();
						return;

					}

					throw new Exception ("error");

				}
				throw new Exception ("error");
				 
			}
			throw new Exception ("error");

		}

		[HttpPost]
		public async Task<IActionResult> AddPhoto (List<IFormFile> photos, string path) {
			if (ModelState.IsValid) {
				if (string.IsNullOrEmpty (path)) {
					throw new Exception ("error");
				}

				//首先得到path对应Album,如果是当前用户的Album,再增加
				var query = _context.Albums.Where (q => q.AlbumPath == path);
				if (query.Any ()) {
					var album = query.First ();
					var user = await _userManager.GetUserAsync (User);
					if (album.BelongerId == user.Id) {

						//增加图片
						foreach (var photo in photos) {

							string photoPath = $"{_hostingEnvironment.WebRootPath}/{album.AlbumPath}/{photo.FileName}";
							using (FileStream MyFileStream = System.IO.File.Create (photoPath)) {
								photo.CopyTo (MyFileStream);
								MyFileStream.Flush ();
							}
						}
						return Json(new {status="success"});
						//return View ("ShowAlbum", new ShowAlbumModel () { Path = path });
					}

					throw new Exception ("error");

				}

				throw new Exception ("error");
			}
			throw new Exception ("error");
		}
		[HttpPost]
		public async Task<IActionResult> DeletePhoto(string albumPath, string pictureName)
		{
			
			if (string.IsNullOrEmpty(albumPath) || string.IsNullOrEmpty(pictureName))
			{
				throw new Exception("error");
			}


			if (ModelState.IsValid)
			{
				var query = _context.Albums.Where(q => q.AlbumPath == albumPath);
				if (query.Any())
				{
					var album = query.First();
					var user = await _userManager.GetUserAsync(User);
					if (user.Id == album.BelongerId)
					{
						
						//delete photo,maybe you 
						var picturePath = $"{_hostingEnvironment.WebRootPath}/{albumPath}/{pictureName}";
						System.IO.File.Delete(picturePath);
						return View("ShowAlbum", new ShowAlbumModel { Path = albumPath });

					}

				}

				throw new Exception("error");



			}
			throw new Exception("error");

			//test

			
			//test


		}
		[HttpGet]
		public IActionResult ShowAlbumWithCarousel(ShowAlbumModel model,int page,int index)
		{
			//return Json (new { pictures = pictureList, count = allPictures.Count () });
			if (string.IsNullOrEmpty(model.Path))
			{
				return Json(new { });
			}
			var pictureList = new List<string>();
			var albumPathTemp = model.Path;
			var rootPath = _hostingEnvironment.WebRootPath;
			var albumPath = $"{rootPath}/{albumPathTemp}";



			//先得到albumPath下的所有图片
			var allPicturesUnordered = Directory.GetFiles(albumPath);

			//按照上传时间排序
			var allPictures = allPicturesUnordered.OrderBy(q => new FileInfo(q).CreationTime).ToList();

			int countOfPhotos = GetCountOfPhotos();
			ViewData["CountOfPhotos"] = countOfPhotos;


			//取第page*30+30张图片
			for (int i = (page - 1) * countOfPhotos; i < page * countOfPhotos && i < allPictures.Count(); i++)
			{
				allPictures[i] = allPictures[i].Replace('\\', '/');
				//  D:/asdasdad/asdsad/asdasd/images/46f060ffffffss/1.jpg
				var _index = allPictures[i].LastIndexOf('/') ;
				var pictureName = allPictures[i].Substring(_index);
				var picturePath = model.Path + pictureName;
				pictureList.Add(picturePath);
			}


			return View(new ShowAlbumWithCarouselModel { Pictures=pictureList,Index=index});



		}
		[HttpGet]
		public async Task<IActionResult> Profile()
		{
			var user = await _userManager.GetUserAsync(User);
			
			return View(user);

		}
		[HttpPost]
		public async Task<IActionResult> Profile(ApplicationUser user, bool EmailConfirmed)
		{
			if (ModelState.IsValid)
			{
				var user2 = await _userManager.GetUserAsync(User);
				if (user2.Id == user.Id)
				{
					var query = _context.Users.Where(q => q.Id == user.Id);
					var userInDb=query.First();
					userInDb.UserName = user.UserName;
					userInDb.Gender = user.Gender;
					userInDb.BirthDate = user.BirthDate;
					userInDb.EmailConfirmed = user.EmailConfirmed;
					_context.SaveChanges();
					return View(user);
				}
				throw new Exception("error");

			}
			throw new Exception("error");

			
		}
		[HttpGet]
		public IActionResult ShowAlbumWithCircle(ShowAlbumModel model,int page)
		{
			var pictureList = new List<string>();
			var albumPath = $"{_hostingEnvironment.WebRootPath}/{model.Path}";

			//先得到albumPath下的所有图片
			var allPicturesUnordered = Directory.GetFiles(albumPath);

			//按照上传时间排序
			var allPictures = allPicturesUnordered.OrderBy(q => new FileInfo(q).CreationTime).ToList();

			int countOfPhotos = GetCountOfPhotos();
			ViewData["countOfPhotos"] = countOfPhotos;
			//取第page*30+30张图片
			for (int i = (page - 1) * countOfPhotos; i < page * countOfPhotos && i < allPictures.Count(); i++)
			{
				allPictures[i] = allPictures[i].Replace('\\', '/');
				//  D:/asdasdad/asdsad/asdasd/images/46f060ffffffss/1.jpg
				var _index = allPictures[i].LastIndexOf('/');
				var pictureName = allPictures[i].Substring(_index);
				var picturePath = model.Path + pictureName;
				pictureList.Add(picturePath);
			}


			
			return View(pictureList);
		}
		public int GetCountOfPhotos()
		{
			
			int? count=_configuration.GetValue(typeof(int), "CountOfPhotos") as int?;
			
			if (count != null)
			{
				return (int)count;
			}
			throw new Exception("invalid value of the count of photos!");
		}

		[HttpPost]
		public async Task<IActionResult> ModifyAlbumName(ModifyAlbumNameModel model)
		{
			if (string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.AlbumName))
			{
				throw new Exception("error");
			}

			if (ModelState.IsValid)
			{
				//如果这个album存在且是当前用户的album,则修改名字
				var query = _context.Albums.Where(q => q.Id == model.Id);
				if (query.Any())
				{
					var album = query.First();
					var user = await _userManager.GetUserAsync(User);

					if (album.BelongerId == user.Id)
					{
						album.Name = model.AlbumName;
						_context.SaveChanges();
						return RedirectToAction("Album");


					}
					throw new Exception("error");


				}

				throw new Exception("error");


			}

			throw new Exception("error");


		}
	}
}