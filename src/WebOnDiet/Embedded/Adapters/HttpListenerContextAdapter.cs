using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using WebOnDiet.Framework;
using System.Linq;

namespace WebOnDiet.Embedded.Adapters
{
	public class HttpListenerContextAdapter : IHttpContext
	{
		private readonly HttpListenerContext _inner;

		public HttpListenerContextAdapter(HttpListenerContext inner)
		{
			_inner = inner;
			Request = new HttpListenerRequestAdapter(inner.Request);
			Response = new HttpListenerResponseAdapter(inner.Response);
			Items = new HybridDictionary(true);
		}

		public IRequest Request { get; private set; }

		public IResponse Response { get; private set; }

		public IDictionary Items { get; private set; }
	}

	public class HttpListenerResponseAdapter : IResponse, IDisposable
	{
		readonly HttpListenerResponse _inner;

		readonly TextWriter _writer;

		public HttpListenerResponseAdapter(HttpListenerResponse inner)
		{
			_inner = inner;
			_writer = new StreamWriter(_inner.OutputStream);
		}

		public void Write(string value)
		{
			_writer.Write(value);
		}

		public void Clear()
		{
			
		}

		public int StatusCode
		{
			get { return _inner.StatusCode; }
			set { _inner.StatusCode = value; }
		}

		public string ContentType
		{
			get { return _inner.ContentType; }
			set { _inner.ContentType = value; }
		}

		public void Flush()
		{
			_writer.Flush();
		}

		public void Dispose()
		{
			if (_writer == null) return;
			try
			{
				_writer.Flush();
				_writer.Close();
			}
			catch
			{
			}
			finally
			{
				try
				{
					_writer.Dispose();
				}
				catch
				{
				}
			}
		}
	}

	public class HttpListenerRequestAdapter : IRequest
	{
		private readonly HttpListenerRequest _inner;
		static readonly string[] MethodsWithBody = new[] { "POST", "PUT" };

		public HttpListenerRequestAdapter(HttpListenerRequest inner)
		{
			_inner = inner;

			if (MethodsWithBody.Contains(inner.HttpMethod.ToUpperInvariant()))
			{
				string formContent;
				using (var reader = new StreamReader(inner.InputStream))
				{
					formContent = reader.ReadToEnd();
				}

				Form = HttpUtility.ParseQueryString(formContent);
			}
			var parts = inner.RawUrl.Split(new[] {'?'}, 2);
			if (parts.Length == 2)
			{
				QueryString = HttpUtility.ParseQueryString(parts[1]);
			}
		}

		public string ApplicationPath
		{
			get { return "/"; }
		}

		public NameValueCollection Form { get; private set; }

		public NameValueCollection QueryString{ get; private set; }

		public string Path
		{
			get { return _inner.Url.AbsolutePath; }
		}

		public Uri Url
		{
			get { return _inner.Url; }
		}
	}
}