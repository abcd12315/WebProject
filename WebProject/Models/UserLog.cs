using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Models
{
	public class UserLog
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }

		public ApplicationUser Belonger { get; set; }

		public string BelongerId { get; set; }

		public DateTime RegistrationTime { get; set; }
	}
}
