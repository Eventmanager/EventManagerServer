using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EventManagerServer.Database;

namespace EventManagerServer
{
	class RequestManager
	{
		private DatabaseWrapper databaseWrapper;
		private const int DEFAULT_COUNT = 10;

		public RequestManager(DatabaseWrapper databaseWrapper, IRequestGenerator generator)
		{
			generator.OnPOST += HandlePost;
			generator.OnGET += HandleGet;
			this.databaseWrapper = databaseWrapper;
		}

		private void HandlePost(object sender, RequestContainer args)
		{

		}

		public static String ConstructQueryString(System.Collections.Specialized.NameValueCollection parameters)
		{
			List<string> items = new List<string>();

			foreach (String name in parameters)
				items.Add(String.Concat(name, "=", parameters[name]));

			return String.Join("&", items.ToArray());
		}

		private void HandleGet(object sender, RequestContainer args)
		{
			string path = args.Context.Request.Url.AbsolutePath;
			// Remove leading slash
			path = path.Substring(1);

			if(path.EndsWith("/")){
				path = path.Substring(0, path.Length-1);
			}

			var qs = args.Context.Request.QueryString;

			Logger.Log("GET {0}?{1}", LogLevel.Debug, path, ConstructQueryString(qs));

			switch (path) 
			{
				case "events":
					HandleGetEvents(args);
					break;
				case "news":
					HandleGetNews(args);
					break;
			}
		}

		private void HandleGetNews(RequestContainer args)
		{
			var afterStr = args.Context.Request.QueryString["after"];
			var countStr = args.Context.Request.QueryString["count"];

			int count;
			if (!int.TryParse(countStr, out count) || count < 0) {
				// Use the default value if the request does not specify a value for count, or if the specified value is invalid.
				count = DEFAULT_COUNT;
			}
			DateTime after;
			if (afterStr == null) {
				after = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				var posts = databaseWrapper.GetNews(count, after);
				args.Writer.WriteLine(posts);
			} else {
				if (!DateTime.TryParse(afterStr, null, System.Globalization.DateTimeStyles.AssumeUniversal, out after)) {
					args.Context.Response.StatusCode = 400;
				} else {
					var posts = databaseWrapper.GetNews(count, after);
					args.Writer.WriteLine(posts);
				}
			}
		}
		private void HandleGetEvents(RequestContainer args)
		{
			var events = databaseWrapper.GetEvents();
            args.Writer.WriteLine(events);
		}
	}
}
