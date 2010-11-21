using System.Linq;
using System.Text.RegularExpressions;

namespace WebOnDiet.Framework.Routes
{
	public class PatternRoute : AbstractRoute
	{
		readonly string _pattern;
		readonly Regex _regex;
		readonly string[] _routeParameterNames;

		public PatternRoute(string pattern)
		{
			_pattern = pattern;
			var parts = _pattern.Split('/');

			var regexPattern = parts
				.Select(p => p.StartsWith(":") ? "(?<" + p.Substring(1).ToLower() + ">[^/\\?]+)" : p)
				.Aggregate((a, b) => a + "/" + b);
			_regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_routeParameterNames = parts
				.Where(p => p.StartsWith(":"))
				.Select(p => p.Substring(1).ToLower())
				.ToArray();
		}

		public override RouteMatch Match(string path)
		{
			var match = _regex.Match(path);
			if (match.Success == false)
				return new RouteMatch { Score = int.MinValue, Route = this };

			var routeMatch = new RouteMatch {Route = this};
			foreach (var routeParameterName in _routeParameterNames)
			{
				routeMatch.RouteParameters.Add(routeParameterName, new[] {match.Groups[routeParameterName].Value});
				routeMatch.Score++;
			}

			return routeMatch;
		}
	}
}