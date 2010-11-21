using WebOnDiet.Framework.Routes;

namespace TestSite
{
	public class Actions
	{
		[Get("/")]
		public string Testing()
		{
			return "Hello";
		}
	}
}