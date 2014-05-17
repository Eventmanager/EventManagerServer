using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Principal;
using System.Diagnostics;

namespace EventManagerServer
{
	class EventManagerServer
	{
		private HttpHandler httpHandler;

		public void Start()
		{
			httpHandler = new HttpHandler();
			httpHandler.OnGET += (sender, container) =>
			{
				Logger.Log("Handling GET request");
				container.Writer.Write(
@"{
	'success': true,
	'result':
	{
		'requested-page': '"
+ container.Context.Request.Url.AbsolutePath +
@"'
	}
}");
			};
			httpHandler.OnPOST += (sender, container) =>
			{
				Logger.Log("Handling POST request");
				container.Writer.Write(container.Reader.ReadToEnd());
			};

			httpHandler.StartServer();

			Logger.Log("Goodbye.", LogLevel.Info);
		}

		static void Main(string[] args)
		{
			if (!Debugger.IsAttached && !new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)) {
				var startInfo = new ProcessStartInfo()
				{
					FileName = Process.GetCurrentProcess().ProcessName + ".exe",
					Verb = "runas"
				};
				Process.Start(startInfo);
			} else {
				new EventManagerServer().Start();
			}
		}
	}
}
