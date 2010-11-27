using System.Collections.Generic;
using NUnit.Framework;
using WebOnDiet.Framework.Routes;

namespace WebOnDiet.Tests.Framework.Routes
{
	[TestFixture]
	public class ParameterBinderTests
	{
		[Test]
		public void Bind_NoValue_ReturnDefault()
		{
			var binder = new StubBinder();

			var key = "name";
			
			var actual = binder.Bind(key, typeof(object));

			Assert.That(actual, Is.EqualTo(default(object)));
			Assert.That(binder.GivenValues.Count, Is.EqualTo(0));
		}

		[Test]
		public void Bind_SingleValueNotMatching_ReturnDefault()
		{
			var binder = new StubBinder();

			var key = "name";
			var anotherKey = "anotherKey";
			var value = "Ken";

			var bag1 = new Dictionary<string, string[]> { { anotherKey, new[] { value } } };

			var actual = binder.Bind(key, typeof(object), bag1);

			Assert.That(actual, Is.EqualTo(default(object)));
			Assert.That(binder.GivenValues.Count, Is.EqualTo(0));
		}

		[Test]
		public void Bind_SingleMatchingValue_BindsCorrectly()
		{
			var binder = new StubBinder();

			var key = "name";
			var value = "Ken";
			var bag1 = new Dictionary<string, string[]> {{key, new[] {value}}};

			var actual = binder.Bind(key, typeof (object), bag1);

			Assert.That(actual, Is.EqualTo(value));
			Assert.That(binder.GivenValues.Count, Is.EqualTo(1));
			Assert.That(binder.GivenValues[0], Is.EqualTo(value));
		}

		[Test]
		public void Bind_MultipleMatchingValueForNonEnumerable_BindsCorrectly()
		{
			var binder = new StubBinder();

			var key = "name";
			var value = "Ken";
			var someOtherValue = "Oren";
			var bag1 = new Dictionary<string, string[]> { { key, new[] { value, someOtherValue } } };

			var actual = binder.Bind(key, typeof(object), bag1);

			Assert.That(actual, Is.EqualTo(value));
			Assert.That(binder.GivenValues.Count, Is.EqualTo(1));
			Assert.That(binder.GivenValues[0], Is.EqualTo(value));
		}

		[Test]
		public void Bind_MatchingValuesInSecondaryBag_BindsCorrectly()
		{
			var binder = new StubBinder();

			var key = "name";
			var anotherKey = "anotherKey";
			var value = "Ken";
			var someOtherValue = "Oren";
			var bag1 = new Dictionary<string, string[]> {{anotherKey, new[] {someOtherValue}}};
			var bag2 = new Dictionary<string, string[]> {{key, new[] {value}}};

			var actual = binder.Bind(key, typeof(object), bag1, bag2);

			Assert.That(actual, Is.EqualTo(value));
			Assert.That(binder.GivenValues.Count, Is.EqualTo(1));
			Assert.That(binder.GivenValues[0], Is.EqualTo(value));
		}

		[Test]
		public void Bind_MatchingValuesInMultipleBags_SignificantBagWins()
		{
			var binder = new StubBinder();

			var key = "name";
			var value = "Ken";
			var someOtherValue = "Oren";
			var bag1 = new Dictionary<string, string[]> { { key, new[] { value } } };
			var bag2 = new Dictionary<string, string[]> { { key, new[] { someOtherValue } } };

			var actual = binder.Bind(key, typeof(object), bag1, bag2);

			Assert.That(actual, Is.EqualTo(value));
			Assert.That(binder.GivenValues.Count, Is.EqualTo(1));
			Assert.That(binder.GivenValues[0], Is.EqualTo(value));
		}
	}

	internal class StubBinder : AbstractParameterBinder<object>
	{
		public readonly List<string> GivenValues = new List<string>();
		protected override object Convert(string value)
		{
			GivenValues.Add(value);
			return value;
		}
	}
}