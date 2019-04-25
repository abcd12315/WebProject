using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Models
{
	public class ApplicationUser : IdentityUser
	{

		public IEnumerable<UserLog> Logs { get; set; }
		public IEnumerable<UserAlbum> Albums { get; set; }

		
		public string ProfilePhotoPath { get; set; }
		public string Gender { get; set; }//这里用字符串代替
		public DateTime BirthDate { get; set; }

		

	}
}
