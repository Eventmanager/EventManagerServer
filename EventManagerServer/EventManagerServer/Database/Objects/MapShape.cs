using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;

namespace EventManagerServer.Database.Objects
{
    class MapShape
    {
        public ObjectId Id { get; set; }
        public int width { get; set; }
        public float[] longitudes { get; set; }
        public float[] latitudes { get; set; }
        public Color fill { get; set; }
        public Color stroke { get; set; }
    }
}
