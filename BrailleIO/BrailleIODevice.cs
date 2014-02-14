using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO
{
    public abstract class BrailleIODevice
    {
        public readonly int screensize_x;
        public readonly int screensize_y;
        public readonly String device_name;
        public readonly bool has_keys;
        public readonly bool has_touch;
        public readonly int hertz;

        public BrailleIODevice(int size_x, int size_y, String device_name, bool has_keys, bool has_touch, int hertz)
        {
            this.screensize_x = size_x;
            this.screensize_y = size_y;
            this.device_name = device_name;
            this.has_keys = has_keys;
            this.has_touch = has_touch;
            this.hertz = hertz;        
        }
    }
}
