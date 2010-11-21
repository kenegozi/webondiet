namespace WebOnDiet.Tests.Views.Models
{
	public interface ISomeModel
	{
		string name { get; set; }
	}

	public class SomeModel : ISomeModel
	{
		public string name { get; set; }
	}
}