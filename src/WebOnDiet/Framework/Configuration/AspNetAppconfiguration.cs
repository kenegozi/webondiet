using System.Collections.Generic;
using System.Reflection;

namespace WebOnDiet.Framework.Configuration
{
	public class AspNetAppConfiguration : IConfiguration
	{
		readonly List<Assembly> _routesAssemblies = new List<Assembly>();
		public void AddRoutesAssembly(Assembly routesAssembly)
		{
			_routesAssemblies.Add(routesAssembly);
		}

		public IEnumerable<Assembly> RouteAssemblies { get { return _routesAssemblies; } }
	}
}