using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Models
{
	public class DeleteLogModel
	{
		public string Id { get; set; }//Log Id  

		//这里没有添加用户Id,与Add Log的想法不一样。
	}
}
