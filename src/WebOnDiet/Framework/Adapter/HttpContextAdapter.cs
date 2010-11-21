using System;
using System.Collections;
using System.Collections.Specialized;
using System.Security.Policy;
using System.Web;

namespace WebOnDiet.Framework.Adapter
{
	public class HttpContextAdapter : IHttpContext
	{
		private readonly HttpContext _inner;

		public HttpContextAdapter(HttpContext inner)
		{
			_inner = inner;
			Request = new HttpRequestAdapter(inner.Request);
			Response = new HttpResponseAdapter(inner.Response);
		}

		public IRequest Request { get; private set; }

		public IResponse Response { get; private set; }

		public IDictionary Items
		{
			get { return _inner.Items; }
		}
	}

	public class HttpResponseAdapter : IResponse
	{
		private readonly HttpResponse _inner;

		public HttpResponseAdapter(HttpResponse inner)
		{
			_inner = inner;
		}

		public void Write(string value)
		{
			_inner.Write(value);
		}

		public void Clear()
		{
			_inner.Clear();
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
			_inner.Flush();
		}
	}

	public class HttpRequestAdapter : IRequest
	{
		private readonly HttpRequest _inner;

		public HttpRequestAdapter(HttpRequest inner)
		{
			_inner = inner;
		}

		public string ApplicationPath
		{
			get { return _inner.ApplicationPath; }
		}

		public string Path
		{
			get { return _inner.Path; }
		}

		public Uri Url
		{
			get { return _inner.Url; }
		}

		public NameValueCollection Form
		{
			get { return _inner.Form; }
		}

		public NameValueCollection QueryString
		{
			get { return _inner.QueryString; }
		}
	}
}