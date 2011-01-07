using System.Linq;
using System.Text.RegularExpressions;

namespace WebOnDiet.Framework.Routes
{
	public class PatternRoute : AbstractRoute
	{
		readonly string _pattern;
		readonly Regex _regex;
		readonly string[] _routeParameterNames;
		readonly int _staticPartsCount;

		public PatternRoute(string pattern)
		{
			pattern = pattern.TrimEnd('/');
			_pattern = pattern;
			var parts = _pattern.Split('/');
			_staticPartsCount = parts.Count(p => string.IsNullOrEmpty(p) == false && p.StartsWith(":") == false);

			var regexPattern = parts
				.Select(p => p.StartsWith(":") ? "(?<" + p.Substring(1).ToLower() + ">[^/\\?]+)" : p)
				.Aggregate((a, b) => a + "/" + b);
			_regex = new Regex("^" + regexPattern + "$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
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

			var routeMatch = new RouteMatch {Route = this, Score = _staticPartsCount*100};
			foreach (var routeParameterName in _routeParameterNames)
			{
				routeMatch.RouteParameters.Add(routeParameterName, new[] {match.Groups[routeParameterName].Value});
				routeMatch.Score++;
			}

			return routeMatch;
		}
	}
}