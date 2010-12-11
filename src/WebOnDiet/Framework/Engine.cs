using System;
using System.Collections;
using System.Web;
using NTemplate;

namespace WebOnDiet.Framework
{
	public interface IContext
	{
		
	}
	public class Context : IContext
	{
		
	}

	public class Renderer
	{
		readonly NTemplateEngine _templateEngine;

		public Renderer(NTemplateEngine templateEngine)
		{
			_templateEngine = templateEngine;
		}

		public void Template(string templateName, IDictionary parameters)
		{
			HttpContext.Current.Response.Output.Write(_templateEngine.Render(templateName, parameters));
		}
		public void Template(string templateName)
		{
			HttpContext.Current.Response.Output.Write(_templateEngine.Render(templateName));
		}
	}
}