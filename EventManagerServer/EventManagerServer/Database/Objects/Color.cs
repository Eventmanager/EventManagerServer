using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;

namespace EventManagerServer.Database.Objects {
    class Color {
        public byte a { get; set; }
        public byte r { get; set; }
        public byte g { get; set; }
        public byte b { get; set; }

        public Color(byte _a, byte _r, byte _g, byte _b) {
            a = _a;
            r = _r;
            g = _g;
            b = _b;
        }
    }
}
