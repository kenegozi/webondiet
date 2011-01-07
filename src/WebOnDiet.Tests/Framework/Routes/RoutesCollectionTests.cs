using NUnit.Framework;
using WebOnDiet.Framework.Routes;

namespace WebOnDiet.Tests.Framework.Routes
{
	[TestFixture]
	public class RoutesCollectionTests
	{
		[Test]
		public void SingleExactRoute_Matches()
		{
			var routes = new RoutesCollection();
			var route = new ExactMatchRoute("/a/b/c"); 
			routes.Add(route);

			var match = routes.Match("/a/b/c");

			Assert.That(match.Route, Is.SameAs(route));
		}

		[Test]
		public void SinglePatternRoute_Matches()
		{
			var routes = new RoutesCollection();
			var route = new PatternRoute("/a/:b/c");
			routes.Add(route);

			var match = routes.Match("/a/b/c");

			Assert.That(match.Route, Is.SameAs(route));
		}

		[Test]
		public void TwoExacts_MatchesTheCorrectOne()
		{
			var routes = new RoutesCollection();
			var route1 = new ExactMatchRoute("/a/b/c");
			var route2 = new ExactMatchRoute("/a/b/c/d");
			routes.Add(route1);
			routes.Add(route2);

			var match = routes.Match("/a/b/c");

			Assert.That(match.Route, Is.SameAs(route1));
		}

		[Test]
		public void TwoPatterns_MatchesTheCorrectOne()
		{
			var routes = new RoutesCollection();
			var route1 = new PatternRoute("/a/:b/c");
			var route2 = new PatternRoute("/a/:b/d");
			routes.Add(route1);
			routes.Add(route2);

			var match = routes.Match("/a/b/c");

			Assert.That(match.Route, Is.SameAs(route1));
		}

		[Test]
		public void TwoPatternsThatMatch_MatchesTheOneWithHigherScore()
		{
			var routes = new RoutesCollection();
			var route1 = new PatternRoute("/a/:b/c");
			var route2 = new PatternRoute("/a/:b/:c");
			routes.Add(route1);
			routes.Add(route2);

			var match = routes.Match("/a/b/c");

			Assert.That(match.Route, Is.SameAs(route1));
		}

		[Test]
		public void ExactAndPatternsThatMatch_MatchesTheExact()
		{
			var routes = new RoutesCollection();
			var route1 = new ExactMatchRoute("/a/b/c");
			var route2 = new PatternRoute("/a/:b/c");
			routes.Add(route1);
			routes.Add(route2);

			var match = routes.Match("/a/b/c");

			Assert.That(match.Route, Is.SameAs(route1));
		}
	}
}