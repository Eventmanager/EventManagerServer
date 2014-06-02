using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EventManagerServer.Database;
using System.Security.Principal;
using System.Diagnostics;

namespace EventManagerServer
{
	class EventManagerServer
	{
		private HttpHandler httpHandler;
		private RequestManager requestManager;
		private DatabaseWrapper databaseWrapper;

		public void Start()
		{
			httpHandler = new HttpHandler();
			databaseWrapper = new DatabaseWrapper();
			databaseWrapper.Connect();
			requestManager = new RequestManager(databaseWrapper, httpHandler);

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
