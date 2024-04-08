using AgriculturalForum.Web.Models;

namespace AgriculturalForum.Web.ModelViews
{
	public class HomeViewModel
	{
		public List<Post> Post { get; set; }
		public List<Product> Products { get; set; }
	}
}
