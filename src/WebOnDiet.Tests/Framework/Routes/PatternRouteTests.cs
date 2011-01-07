using NUnit.Framework;
using WebOnDiet.Framework.Routes;

namespace WebOnDiet.Tests.Framework.Routes
{
	[TestFixture]
	public class PatternRouteTests
	{
		[Test]
		public void ExactMatch_Matches()
		{
			var pattern = new PatternRoute("/a/:b/c");

			var match = pattern.Match("/a/b/c");

			Assert.That(match.Score, Is.EqualTo(201));
		}

		[Test]
		public void ExactMatch_TrailingSlashDoesNotMatter()
		{
			var pattern1 = new PatternRoute("/a/:b/c");
			var pattern2 = new PatternRoute("/a/:b/c/");
			var url = "/a/b/c";

			var match1 = pattern1.Match(url);
			var match2 = pattern2.Match(url);

			Assert.That(match1.Score, Is.EqualTo(201));
			Assert.That(match2.Score, Is.EqualTo(201));
		}

		[Test]
		public void ExactMatchButWithPrefix_DoesNotMatches()
		{
			var pattern = new PatternRoute("/a/:b/c");

			var match = pattern.Match("/blah/a/b/c");

			Assert.That(match.Score, Is.EqualTo(int.MinValue));
		}

		[Test]
		public void ExactMatchButWithPostfix_DoesNotMatches()
		{
			var pattern = new PatternRoute("/a/:b/c");

			var match = pattern.Match("/a/b/c/moo");

			Assert.That(match.Score, Is.EqualTo(int.MinValue));
		}


		[Test]
		public void StaticPartsScoreHigherThanParameters()
		{
			var pattern1 = new PatternRoute("/a/:b/c");
			var pattern2 = new PatternRoute("/a/:b/:c");

			var match1 = pattern1.Match("/a/b/c");
			var match2 = pattern2.Match("/a/b/c");

			Assert.That(match1.Score, Is.GreaterThan(match2.Score));
		}
	}
}