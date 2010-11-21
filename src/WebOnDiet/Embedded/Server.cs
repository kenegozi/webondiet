using System;
using System.Net;
using System.Threading;
using System.Web;
using WebOnDiet.Embedded.Adapters;
using WebOnDiet.Framework;
using WebOnDiet.Framework.Configuration;

namespace WebOnDiet.Embedded
{
	public class Server : IWebOnDietApplication
	{
		private HttpListener _httpListener;
		private ManualResetEvent waitEvent;
		private ManualResetEvent stopEvent;
		public void Start()
		{
			var runner = new Thread(() =>
			                        	{
			                        		_httpListener = new HttpListener();
			                        		_httpListener.Prefixes.Add("http://localhost:1234/");

			                        		_httpListener.Start();

			                        		waitEvent = new ManualResetEvent(false);
			                        		stopEvent = new ManualResetEvent(false);
			                        		//while (stopEvent..IsSet == false)
			                        		//{
			                        			_httpListener.BeginGetContext(HandleRequest, _httpListener);
			                        			waitEvent.WaitOne();
			                        			waitEvent.Reset();
			                        		//}
			                        		//stopEvent.Wait();
			                        	});
			runner.Start();
		}

		private void HandleRequest(IAsyncResult ar)
		{
			var listener = (HttpListener) ar.AsyncState;
			var context = new HttpListenerContextAdapter(listener.EndGetContext(ar));
			CurrentContext = context;
			Current = this;
			//listener.BeginGetContext(new AsyncCallback(HandleRequest), listener);
			waitEvent.Set();

			var hanlderFactory = new WebOnDietHttpHandlerFactory();
			var handler = hanlderFactory.GetHandler(context, "blah", context.Request.Url.ToString(), null);

			handler.ProcessRequest(context);


		}


		public void Stop()
		{
			_httpListener.Stop();
			stopEvent.Set();
		}

		public static IHttpContext CurrentContext { get; private set; }

		public static Server Current 
		{ 
			get { return (Server) CurrentContext.Items["CURRENT_EMBEDDED_SERVER_INSTANCE"]; }
			private set { CurrentContext.Items["CURRENT_EMBEDDED_SERVER_INSTANCE"] = value; }
		}

		public virtual void Configure(IConfiguration configuration)
		{

		}
	}
}