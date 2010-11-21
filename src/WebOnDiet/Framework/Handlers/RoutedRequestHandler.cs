using System;
using System.Reflection;
using System.Web;
using WebOnDiet.Framework.Routes;

namespace WebOnDiet.Framework.Handlers
{
	public class RoutedRequestHandler : IHttpHandler
	{
		private readonly RouteMatch _routeMatch;

		public RoutedRequestHandler(RouteMatch routeMatch)
		{
			_routeMatch = routeMatch;
		}

		public void ProcessRequest(HttpContext context)
		{
			ProcessRequest(context.GetCurrentContext());
		}

		public bool IsReusable
		{
			get { return false; }
		}

		public void ProcessRequest(IHttpContext context)
		{
			var target = _routeMatch.Route.Target;
			var instance = Activator.CreateInstance(target.DeclaringType);
			var parameters = _routeMatch.ExtractParameters();
			var result = target.Invoke(instance, parameters);
			context.Response.Write(result.ToString());
		}
	}
}