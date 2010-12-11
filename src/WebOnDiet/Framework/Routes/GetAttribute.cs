using System;
using WebOnDiet.Framework.Routes;

// ReSharper disable CheckNamespace
namespace WebOnDiet
// ReSharper restore CheckNamespace
{
	[AttributeUsage(AttributeTargets.Method)]
	public class GetAttribute : AbstractRouteAttribute
	{
		public GetAttribute(string route)
		{
			Route = route;
		}
	}
}