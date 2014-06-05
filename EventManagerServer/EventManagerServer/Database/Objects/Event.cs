using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;

namespace EventManagerServer.Database.Objects
{
	class Event
	{
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Stage { get; set; }

        public override string ToString() {
            return Title + " " + StartTime.ToString() + " - " + EndTime.ToString();
        }
	}
}
