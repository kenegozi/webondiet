using System.Collections.Generic;
using System.Linq;

namespace WebOnDiet.Framework.Routes
{
	public class RoutesCollection
	{
		List<IRoute> _routes = new List<IRoute>();
		public void Add(IRoute route)
		{
			_routes.Add(route);
		}

		public void AddRange(IEnumerable<IRoute> routes)
		{
			_routes.AddRange(routes);
		}

		public RouteMatch Match(string appRelativeUrl)
		{
			var matches = from route in _routes
						  let m = route.Match(appRelativeUrl)
						  where m.Score > int.MinValue
						  orderby m.Score descending
						  select m;

			return matches.FirstOrDefault();
		}
	}
}