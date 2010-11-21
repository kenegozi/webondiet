using System.Collections.Generic;
using System.Reflection;

namespace WebOnDiet.Framework.Configuration
{
	public interface IConfiguration
	{
		void AddRoutesAssembly(Assembly routesAssembly);
		IEnumerable<Assembly> RouteAssemblies { get; }
	}
}