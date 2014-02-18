using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO
{
    public class BrailleIODevice
    {
        public readonly int DeviceSizeX;
        public readonly int DeviceSizeY;
        public readonly String Name;
        public readonly bool has_keys;
        public readonly bool has_touch;
        public readonly int RefreshRate;

        public BrailleIODevice(int size_x, int size_y, String device_name, bool has_keys, bool has_touch, int hertz)
        {
            this.DeviceSizeX = size_x;
            this.DeviceSizeY = size_y;
            this.Name = device_name;
            this.has_keys = has_keys;
            this.has_touch = has_touch;
            this.RefreshRate = hertz;        
        }
    }
}
