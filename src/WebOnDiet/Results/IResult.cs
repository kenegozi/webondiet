using System.Collections;

namespace WebOnDiet.Results
{
	public interface IResult
	{
		
	}

	public interface IViewResult:IResult
	{
		string TemplateName { get; }
		IDictionary Parameters { get; }
	}

	public class ViewResult : IViewResult
	{
		public string TemplateName { get; set; }
		public IDictionary Parameters { get; set; }
	}

	public class RedirectResult : IResult
	{
		public string Target { get; set; }
		public bool IsPermanent { get; set; }
	}
}