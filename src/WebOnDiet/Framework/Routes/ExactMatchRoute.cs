using System;

namespace WebOnDiet.Framework.Routes
{
	public class ExactMatchRoute : AbstractRoute
	{
		readonly string _path;

		public ExactMatchRoute(string path)
		{
			_path = path;
		}

		public override RouteMatch Match(string path)
		{
			if (_path.Equals(path, StringComparison.InvariantCultureIgnoreCase))
				return new RouteMatch { Score = int.MaxValue, Route = this };

			return new RouteMatch {Score = int.MinValue, Route = this};
		}
	}
}