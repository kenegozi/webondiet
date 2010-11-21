using System.Reflection;

namespace WebOnDiet.Framework.Routes
{
	public interface IRoute
	{
		RouteMatch Match(string path);
		TargetMethod Target { get; }
	}
}