using System;
using System.Reflection;
using System.Web;
using WebOnDiet.Results;
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
			var instance = WebOnDietHttpHandlerFactory.Container.Resolve(target.DeclaringType);
			var parameters = _routeMatch.ExtractParameters();
			var result = target.Invoke(instance, parameters);
			if (result == null) return;
			if (result is IResult)
			{
				Handle((IResult) result, context);
				return;
			}
			context.Response.Write(result.ToString());
		}

		private void Handle(IResult result, IHttpContext context)
		{
			var viewResult = result as IViewResult;
			if (viewResult != null)
			{
				Handle(viewResult, context);
				return;
			}

			var redirectResult = result as RedirectResult;
			if (redirectResult != null)
			{
				Handle(redirectResult, context);
				return;
			}
		}

		private void Handle(IViewResult result, IHttpContext context)
		{
			wod.Render.Template(result.TemplateName, result.Parameters);
		}

		private void Handle(RedirectResult result, IHttpContext context)
		{
			Action<string, bool> redirectMethod;
			if (result.IsPermanent)
				redirectMethod = context.Response.RedirectPermanent;
			else
				redirectMethod = context.Response.Redirect;

			redirectMethod(result.Target, true);
		}
	}
}