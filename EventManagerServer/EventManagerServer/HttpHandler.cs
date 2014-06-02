using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Threading;

namespace EventManagerServer
{
	interface IRequestGenerator
	{
		event EventHandler<RequestContainer> OnPOST;
		event EventHandler<RequestContainer> OnGET;
	}
	struct RequestContainer
	{
		public HttpListenerContext Context;
		public StreamWriter Writer;
		public StreamReader Reader;

		public RequestContainer(HttpListenerContext context, StreamReader reader, StreamWriter writer){
			Context = context;
			Reader = reader;
			Writer = writer;
		}
	}
	class HttpHandler : IRequestGenerator
	{
		private HttpListener listener;
		private HttpListenerContext context;
		private const int port = 8778;
		private bool stopRequested = false;
		private bool isListening = false;

		public event EventHandler<RequestContainer> OnPOST;
		public event EventHandler<RequestContainer> OnGET;

		public HttpHandler()
		{
			listener = new HttpListener();
		}

		public void StartServer()
		{
			string prefix = string.Format("http://*:{0}/", port);
			listener.Prefixes.Add(prefix);
			listener.Start();
			ThreadPool.QueueUserWorkItem(new WaitCallback(StartListening));
			Console.ReadLine();
			stopRequested = true;
			// Wait for listener to exit.
			listener.Stop();
			while (isListening == true) {
				// Let this thread sleep so the listener thread gets priority.
				Thread.Sleep(10);
			}
		}

		private void StartListening(object o)
		{
			isListening = true;
			Logger.Log("HTTP Server Started. Press Enter to exit.", LogLevel.Info);
			while (!stopRequested) {
				try {
					context = listener.GetContext();
					HandleRequest(context);
				} catch (HttpListenerException e) {
					if (e.ErrorCode == 995) {
						Logger.Log("listener.GetContext() call aborted as the listener has stopped.");
					}
				}
			}
			Logger.Log("HTTP Listener shutdown.", LogLevel.Debug);
			isListening = false;
		}

		private void HandleRequest(HttpListenerContext context)
		{
			// Whatever the case, the server will always return UTF-8-encoded JSON, if it returns anything at all.
			context.Response.ContentEncoding = Encoding.UTF8;
			context.Response.ContentType = "application/json";

			StreamReader reader = new StreamReader(context.Request.InputStream);
			StreamWriter writer = new StreamWriter(context.Response.OutputStream);

			RequestContainer container = new RequestContainer(context, reader, writer);

			switch (context.Request.HttpMethod) {
				case "POST":
					if (OnPOST != null) OnPOST(this, container);
					break;
				case "GET":
					if (OnGET != null) OnGET(this, container);
					break;
				default:
					Logger.Log("Unknown HTTP method received: {0}. Returning HTTP 400");
					context.Response.StatusCode = 400;
					context.Response.StatusDescription = "Bad Request";
					break;
			}
			writer.Close();
		}
	}
}
