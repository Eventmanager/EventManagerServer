using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using MongoDB.Bson.Serialization;
using EventManagerServer.Database.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EventManagerServer.Database
{
	class DatabaseWrapper
	{
		private MongoClient client;
		private MongoServer server;
		private MongoDatabase database;
		private MongoCollection<NewsPost> newsPostCollection;
		private MongoCollection<Event> eventCollection;

		public void Connect()
		{
			var connectionString = "mongodb://jgeluk.net:22217";
			client = new MongoClient(connectionString);
			server = client.GetServer();
			database = server.GetDatabase("eventmanager");
			newsPostCollection = database.GetCollection<NewsPost>("news");
			eventCollection = database.GetCollection<Event>("events");

			//newsPostCollection.Insert(new NewsPost() { Contents = "very first test post", Date = DateTime.Parse("2009-02-20 14:39:54"), Title = "THE VERY FIRST TEST POST" });
            //eventCollection.Insert(new Event() { Title = "TestEvent1", StartTime = DateTime.Parse("2014-05-06 12:30:00"), EndTime = DateTime.Parse("2014-05-06 14:00:00"), Stage = "TestStage1" });
        }

		internal object GetEvents()
		{
            var entities = eventCollection.FindAllAs<Event>();

            var postObject = new List<JObject>();
            foreach(var entity in entities) { 
                var obj = new JObject(
                    new JProperty("id", entity.Id.ToString()),
                    new JProperty("title", entity.Title),
                    new JProperty("starttime", entity.StartTime.ToString("yyyy-MM-dd HH:mm:ss")),
                    new JProperty("endtime", entity.EndTime.ToString("yyyy-MM-dd HH:mm:ss")),
                    new JProperty("stage", entity.Stage)
                );
                postObject.Add(obj);
            }

            var response = new JObject(
                new JProperty("success", true),
                new JProperty("result", new JObject(
                    new JProperty("events", new JArray(
                        postObject.ToArray())
                        ))
                    )
                );

            var jsonString = response.ToString();
            return jsonString;
		}

		internal string GetNews(int count, DateTime after)
		{
			var query = Query<NewsPost>.GT(post => post.Date, after);
			var entities = newsPostCollection.Find(query).OrderBy(post => post.Date);
			var result = entities.Skip(Math.Max(0, entities.Count() - count));

			var postObjects = new List<JObject>();
			foreach (var entity in result) {
				var obj = new JObject(
					new JProperty("title", entity.Title),
					new JProperty("id", entity.Id.ToString()),
					new JProperty("date", entity.Date.ToString("yyyy-MM-dd HH:mm:ss")),
					new JProperty("contents", entity.Contents)
					);
				postObjects.Add(obj);
			}
			var response = new JObject(
				new JProperty("success", true),
				new JProperty("result", new JObject(
					new JProperty("posts", new JArray(
						postObjects.ToArray())
					))
				)
			);

			var jsonString = response.ToString();
			return jsonString;
		}
	}
}
