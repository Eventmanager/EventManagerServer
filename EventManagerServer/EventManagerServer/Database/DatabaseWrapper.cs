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
        private MongoCollection<MapShape> mapShapeCollection;

		public void Connect()
		{
			var connectionString = "mongodb://jgeluk.net:22217";
			client = new MongoClient(connectionString);
			server = client.GetServer();
			database = server.GetDatabase("eventmanager");
			newsPostCollection = database.GetCollection<NewsPost>("news");
			eventCollection = database.GetCollection<Event>("events");
            mapShapeCollection = database.GetCollection<MapShape>("mapShape");
            
            /*
            mapShapeCollection.Insert(new MapShape() { 
                width = 2,
                latitudes = new float[] {20, 40, 0},
                longitudes = new float[] {20, 40, 40},
                fill = new Color(128, 255, 0, 0),
                stroke = new Color(255, 0, 0, 0)
            });

            mapShapeCollection.Insert(new MapShape() {
                width = 5,
                latitudes = new float[] { 0, -40, -40, 0 },
                longitudes = new float[] { 0, 0, -40, -40 },
                fill = new Color(128, 0, 0, 255),
                stroke = new Color(255, 255, 255, 255)
            });
            
            newsPostCollection.Insert(new NewsPost() { Contents = "very first test post", Date = DateTime.Parse("2009-02-20 14:39:54"), Title = "THE VERY FIRST TEST POST" });
            eventCollection.Insert(new Event() { Title = "TestEvent1", Description = "Description of Testevent1 Lorum", StartTime = DateTime.Parse("2014-05-06 12:30:00"), EndTime = DateTime.Parse("2014-05-06 14:00:00"), Stage = "TestStage1" });
            eventCollection.Insert(new Event() { Title = "TestEvent2", Description = "Description of Testevent2 Lorum Ipsem", StartTime = DateTime.Parse("2014-05-06 14:00:01"), EndTime = DateTime.Parse("2014-05-06 15:30:00"), Stage = "TestStage1" });
            eventCollection.Insert(new Event() { Title = "TestEvent3", Description = "Description of Testevent3 Lorum Ipsem dolar sit amet", StartTime = DateTime.Parse("2014-05-06 13:00:00"), EndTime = DateTime.Parse("2014-05-06 14:15:00"), Stage = "TestStage2" });
            eventCollection.Insert(new Event() { Title = "TestEvent4", Description = "Description of Testevent4 Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa.", StartTime = DateTime.Parse("2014-05-06 14:15:01"), EndTime = DateTime.Parse("2014-05-06 15:40:00"), Stage = "TestStage2" });
            eventCollection.Insert(new Event() { Title = "TestEvent5", Description = "Description of Testevent5 Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi. Aenean vulputate eleifend tellus. Aenean leo ligula, porttitor eu, consequat vitae, eleifend ac, enim. Aliquam lorem ante, dapibus in, viverra quis, feugiat a, tellus. Phasellus viverra nulla ut metus varius laoreet. Quisque rutrum. Aenean imperdiet. Etiam ultricies nisi vel augue. Curabitur ullamcorper ultricies nisi. Nam eget dui. ", StartTime = DateTime.Parse("2014-05-06 12:00:00"), EndTime = DateTime.Parse("2014-05-06 16:20:00"), Stage = "TestStage3" });
        */}

		internal string GetEvents()
		{
            var entities = eventCollection.FindAllAs<Event>();

            var postObject = new List<JObject>();
            foreach(var entity in entities) { 
                var obj = new JObject(
                    new JProperty("id", entity.Id.ToString()),
                    new JProperty("title", entity.Title),
                    new JProperty("description", entity.Description),
                    new JProperty("starttime", entity.StartTime.ToString("yyyy-MM-dd HH:mm:ss")),
                    new JProperty("endtime", entity.EndTime.ToString("yyyy-MM-dd HH:mm:ss")),
                    new JProperty("stage", entity.Stage)
                );
                postObject.Add(obj);
            }

            var response = new JObject(
                /*new JProperty("success", true),
                new JProperty("result", new JObject(*/
                new JProperty("events", new JArray(
                    postObject.ToArray())
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
				/*new JProperty("success", true),
				new JProperty("result", new JObject(*/
				new JProperty("posts", new JArray(
					postObjects.ToArray())
				)
			);

			var jsonString = response.ToString();
			return jsonString;
		}

        internal string GetMapItems() {
            var mapShapes = mapShapeCollection.FindAllAs<MapShape>();

            var images = new List<JObject>();
            //images.Add(new JObject("key", "value"));

            var shapes = new List<JObject>();
            foreach(MapShape shape in mapShapes) {
                var obj = new JObject(
                    new JProperty("width", shape.width),
                    new JProperty("latitudes",
                        new JArray(shape.latitudes)
                    ),
                    new JProperty("longitudes",
                        new JArray(shape.longitudes)
                    ),
                    new JProperty("color", new JObject(
                        new JProperty("r", shape.fill.r),
                        new JProperty("g", shape.fill.g),
                        new JProperty("b", shape.fill.b),
                        new JProperty("a", shape.fill.a)
                    )),
                    new JProperty("stroke", new JObject(
                        new JProperty("r", shape.stroke.r),
                        new JProperty("g", shape.stroke.g),
                        new JProperty("b", shape.stroke.b),
                        new JProperty("a", shape.stroke.a)
                    ))
                );
                shapes.Add(obj);
            }
            
            var response = new JObject(
                new JProperty("images", new JArray(
                    images.ToArray()
                )),
                new JProperty("shapes", new JArray(
                    shapes.ToArray()
                ))
            );

            var jsonString = response.ToString();
            return jsonString;
        }
	}
}
