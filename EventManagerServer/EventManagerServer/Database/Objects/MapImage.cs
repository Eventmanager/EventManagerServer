using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;

namespace EventManagerServer.Database.Objects {
    class MapImage {
        public ObjectId Id { get; set; }
        public int Width { get; set; }
        public float Rotation { get; set; }
        public string ImageName { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
    }
}
