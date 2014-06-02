using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;

namespace EventManagerServer.Database.Objects
{
	class NewsPost
	{
		public ObjectId Id { get; set; }
		public string Title { get; set; }
		public DateTime Date { get; set; }
		public string Contents { get; set; }

		public override string ToString()
		{
			return Date.ToString() + " - " + Title; 
		}
	}
}
