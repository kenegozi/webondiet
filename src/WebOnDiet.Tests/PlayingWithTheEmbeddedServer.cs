using System;
using System.IO;
using System.Net;
using NUnit.Framework;
using WebOnDiet.Embedded;

namespace WebOnDiet.Tests
{
	[TestFixture, Ignore("the embedded server is still off")]
	public class PlayingWithTheEmbeddedServer
	{
		[Test]
		public void A()
		{
			var server = new Server();
			server.Start();

			string r;
			try
			{
				r = new WebClient().DownloadString("http://localhost:1234/abc");
			}
			catch (Exception ex)
			{
				r = ex.ToString();
			}
			finally
			{
				server.Stop();
			}

			Console.WriteLine(r);

		}

	}
}
