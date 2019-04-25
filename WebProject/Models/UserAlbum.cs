using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Models
{
	public class UserAlbum
	{

		public string Id { get; set; }

		public string Name { get; set; }

		public int Count { get; set; }

		public string AlbumPath { get; set; }

		public string FrontPicturePath { get; set; }
		public bool IsPublic { get; set; }

		public ApplicationUser Belonger { get; set; }

		public string BelongerId { get; set; }
	}
}
