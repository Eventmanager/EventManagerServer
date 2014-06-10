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
        public int Width { get; set; }
        public float[] Longitudes { get; set; }
        public float[] Latitudes { get; set; }
        public Color Fill { get; set; }
        public Color Stroke { get; set; }
    }
}
