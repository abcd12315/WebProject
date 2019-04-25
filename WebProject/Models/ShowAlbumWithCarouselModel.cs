using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Models
{
	public class ShowAlbumWithCarouselModel
	{
		public List<string> Pictures { get; set; }
		public int Index { get; set; }//最开始选择的图片。
	}
}
