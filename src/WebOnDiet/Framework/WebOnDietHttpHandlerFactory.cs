using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using NTemplate;
using WebOnDiet.Framework.Adapters;
using WebOnDiet.Framework.Configuration;
using WebOnDiet.Framework.Container;
using WebOnDiet.Framework.Handlers;
using WebOnDiet.Framework.Routes;

namespace WebOnDiet.Framework
{
	public interface IWebOnDietApplication
	{
		void Configure(IConfiguration configuration);
	}
	public class WebOnDietHttpHandlerFactory : IHttpHandlerFactory
	{
		private static IConfiguration Configuration;
		readonly static RoutesCollection Routes = new RoutesCollection();

		public static Kernel Container { get; private set; }

		private static bool initialized;
		static WebOnDietHttpHandlerFactory()
		{
			Initialize();
		}
		internal static void Initialize()
		{
			if (initialized) return;
			initialized = true;
			Action<IConfiguration> configure = x => { };
			if (HttpContext.Current != null)
			{
				var x = AppDomain.CurrentDomain.SetupInformation.CachePath;
				var dir = HttpContext.Current.Server.MapPath("~/bin");
				Configuration = new AspNetAppConfiguration();
				var dllFiles = (from f in Directory.GetFiles(dir, "*.dll", SearchOption.TopDirectoryOnly)
				                select f)
					.ToDictionary(f => Path.GetFileName(f).ToUpper());

				var target = Path.Combine(x, "manual");
				if (Directory.Exists(target))
					Directory.Delete(target, true);
				Directory.CreateDirectory(target);
				foreach (var assemblyFilename in Directory.GetFiles(x, "*.dll", SearchOption.AllDirectories))
				{
					if (dllFiles.ContainsKey(Path.GetFileName(assemblyFilename).ToUpper()))
					{
						Configuration.AddRoutesAssembly(Assembly.LoadFile(assemblyFilename));
						dllFiles.Remove(Path.GetFileName(assemblyFilename).ToUpper());
					}
				}
				foreach (var missing in dllFiles.Values)
				{
					var targetDll = Path.Combine(target, Path.GetFileName(missing));
					File.Copy(missing, targetDll);
					Configuration.AddRoutesAssembly(Assembly.LoadFile(targetDll));
				}
				var currentApp = HttpContext.Current.ApplicationInstance as IWebOnDietApplication;
				if (currentApp != null)
				{
					configure = currentApp.Configure;
					Configuration.AddRoutesAssembly(currentApp.GetType().BaseType.Assembly);
				}

			}
			else
			{
				if (Embedded.Server.CurrentContext != null)
				{
					Configuration = new AspNetAppConfiguration();
					configure = Embedded.Server.Current.Configure;
				}
				else
					throw new Exception("No hosting found");
			}
			configure(Configuration);

			var routedMethods = (from assembly in Configuration.RouteAssemblies
								from type in assembly.GetExportedTypes()
								where type.IsInterface == false
									  && type.IsAbstract == false
								from method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
								from attr in method.GetCustomAttributes(true)
								let routeAttribute = attr as IRouteAttribute
								where routeAttribute != null
								orderby routeAttribute.Precedence descending
								select new { RouteAttribute = routeAttribute, Method = method }).ToArray();
			var routes = from r in routedMethods
						 let route = r.RouteAttribute.Route.IndexOf(':') > 0
									   ? (IRoute)new PatternRoute(r.RouteAttribute.Route) { Target = new TargetMethod(r.Method) }
									   : (IRoute)new ExactMatchRoute(r.RouteAttribute.Route) { Target = new TargetMethod(r.Method) }
						 select route;

			Routes.AddRange(routes);

			var routeClasses = routedMethods.Select(m => m.Method.DeclaringType).Distinct().ToArray();

			Container = new Kernel();
			foreach (var routeClass in routeClasses)
			{
				Container.Register(routeClass, routeClass);
			}

			var templateEngine = new NTemplateEngine(AppDomain.CurrentDomain.SetupInformation.PrivateBinPath);
			templateEngine.Initialize();
			wod.TemplateEngine = templateEngine;
			wod.Render = new Renderer(templateEngine);
		}


		public System.Web.IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
		{
			var abstractContext = new HttpContextAdapter(context);
			context.SetCurrentContext(abstractContext);
			return GetHandler(abstractContext, requestType, url, pathTranslated);
		}

		public void ReleaseHandler(System.Web.IHttpHandler handler)
		{

		}

		public IHttpHandler GetHandler(IHttpContext context, string requestType, string url, string pathTranslated)
		{
			var appPath = (context.Request.ApplicationPath ?? string.Empty).TrimEnd('/');
			var appRelativeUrl = context.Request.Path;
			if (appPath.Length > 0)
				appRelativeUrl = url != appPath
					? url.Substring(appPath.Length)
					: "/";

			if (appRelativeUrl == string.Empty) appRelativeUrl = "/";
			if (appRelativeUrl != "/") appRelativeUrl = appRelativeUrl.TrimEnd('/');

			RouteMatch routeMatch;
			try
			{
				routeMatch = GetRoutedMethod(appRelativeUrl);
			}
			catch (Exception ex)
			{

				throw;
			}
			if (routeMatch != null)
			{
				routeMatch.Context = context;
				return new RoutedRequestHandler(routeMatch);
			}

			NotFoundHandler.SetLookedUpResource(context, appRelativeUrl);
			return NotFoundHandler.Instance;
		}

		static RouteMatch GetRoutedMethod(string appRelativeUrl)
		{
			return Routes.Match(appRelativeUrl);
		}

		public void ReleaseHandler(IHttpHandler handler)
		{

		}
	}

	public interface IHttpHandler : System.Web.IHttpHandler
	{
		void ProcessRequest(IHttpContext context);
	}

	public interface IHttpContext
	{
		IRequest Request { get; }
		IResponse Response { get; }
		IDictionary Items { get; }
	}
	public interface IRequest
	{
		string ApplicationPath { get; }
		string Path { get; }
		Uri Url { get; }
		NameValueCollection Form { get; }
		NameValueCollection QueryString { get; }
	}
	public interface IResponse
	{
		void Write(string value);
		void Clear();
		int StatusCode { get; set; }
		string ContentType { get; set; }
		void RedirectPermanent(string target, bool endResponse);
		void Redirect(string target, bool endResponse);
		void Flush();
	}

	public static class HttpContextExtensions
	{
		private const string CURRENT_CONTEXT_KEY = "CURRENT_CONTEXT_KEY";
		public static IHttpContext GetCurrentContext(this HttpContext context)
		{
			return (IHttpContext)context.Items[CURRENT_CONTEXT_KEY];
		}
		public static void SetCurrentContext(this HttpContext context, IHttpContext abstractContext)
		{
			context.Items[CURRENT_CONTEXT_KEY] = abstractContext;
		}
	}
}