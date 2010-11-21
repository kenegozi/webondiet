using System;
using System.Web;

namespace WebOnDiet.Framework.Handlers
{
	public class NotFoundHandler : IHttpHandler
	{
		public static readonly NotFoundHandler Instance = new NotFoundHandler();
		public static string GetLookedUpResource(IHttpContext context)
		{
			return (context.Items["NotFoundHandler.LookedUpResource"] as string) ?? string.Empty;
		}

		public static void SetLookedUpResource(IHttpContext context, string lookedUpResource)
		{
			context.Items["NotFoundHandler.LookedUpResource"] = lookedUpResource;
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
			context.Response.Clear();
			context.Response.StatusCode = 404;
			context.Response.ContentType = "text/plain";
			context.Response.Write(string.Format("The requested resource [{0}] was not found", GetLookedUpResource(context)));
			context.Response.Flush();
		}
	}
}