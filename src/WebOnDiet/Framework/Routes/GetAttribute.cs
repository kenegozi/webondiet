using System;

namespace WebOnDiet.Framework.Routes
{
	public interface IRouteAttribute
	{
		int Precedence { get; }
		string Route { get; }
	}

	public abstract class AbstractRouteAttribute : Attribute, IRouteAttribute
	{
		public int Precedence { get; set; }
		public string Route { get; protected set; }
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class GetAttribute : AbstractRouteAttribute
	{
		public GetAttribute(string route)
		{
			Route = route;
		}
	}
}