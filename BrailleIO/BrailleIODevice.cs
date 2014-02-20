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
        public readonly bool HasKeys;
        public readonly bool HasTouch;
        public readonly int RefreshRate;
        public readonly string AdapterType;

        public BrailleIODevice(int size_x, int size_y, String device_name, bool has_keys, bool has_touch, int hertz, string adapterType)
        {
            this.DeviceSizeX = size_x;
            this.DeviceSizeY = size_y;
            this.Name = device_name;
            this.HasKeys = has_keys;
            this.HasTouch = has_touch;
            this.RefreshRate = hertz;
            this.AdapterType = adapterType;
        }
    }
}
