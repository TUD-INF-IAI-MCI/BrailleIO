using System;

namespace BrailleIO
{
    public class BrailleIODevice
    {
        /// <summary>
        /// Count of pins in horizontal direction (width)
        /// </summary>
        public readonly int DeviceSizeX;
        /// <summary>
        /// Count of pins in vertical direction (height)
        /// </summary>
        public readonly int DeviceSizeY;
        /// <summary>
        /// Name of the device (try to keep it unique)
        /// </summary>
        public readonly String Name;
        /// <summary>
        /// indicates if the device has keys or not
        /// </summary>
        public readonly bool HasKeys;
        /// <summary>
        /// indicates if the device is touch sensitive or not
        /// </summary>
        public readonly bool HasTouch;
        /// <summary>
        /// rate for refreshing the display in hz = count/sec
        /// </summary>
        public readonly int RefreshRate;
        /// <summary>
        /// String representation for the Type of the adapter 
        /// </summary>
        public readonly string AdapterType;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIODevice"/> class.
        /// </summary>
        /// <param name="size_x">Count of pins in horizontal direction (width)</param>
        /// <param name="size_y">Count of pins in vertical direction (height)</param>
        /// <param name="device_name">Name of the device (try to keep it unique)</param>
        /// <param name="has_keys">indicates if the device has keys or not</param>
        /// <param name="has_touch">indicates if the device is touch sensitive or not</param>
        /// <param name="hertz">rate for refreshing the display in hz = count/sec</param>
        /// <param name="adapterType">String representation for the Type of the adapter</param>
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
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIODevice"/> class.
        /// </summary>
        /// <param name="size_x">Count of pins in horizontal direction (width)</param>
        /// <param name="size_y">Count of pins in vertical direction (height)</param>
        /// <param name="device_name">Name of the device (try to keep it unique)</param>
        /// <param name="has_keys">indicates if the device has keys or not</param>
        /// <param name="has_touch">indicates if the device is touch sensitive or not</param>
        /// <param name="hertz">rate for refreshing the display in hz = count/sec</param>
        /// <param name="adapter">the implemented adapter for this device</param>
        public BrailleIODevice(int size_x, int size_y, String device_name, bool has_keys, bool has_touch, int hertz, object adapter)
        {
        this.DeviceSizeX = size_x;
        this.DeviceSizeY = size_y;
        this.Name = device_name;
        this.HasKeys = has_keys;
        this.HasTouch = has_touch;
        this.RefreshRate = hertz;
        this.AdapterType = adapter.GetType().ToString();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIODevice"/> class.
        /// </summary>
        /// <param name="size_x">Count of pins in horizontal direction (width)</param>
        /// <param name="size_y">Count of pins in vertical direction (height)</param>
        /// <param name="device_name">Name of the device (try to keep it unique)</param>
        /// <param name="has_keys">indicates if the device has keys or not</param>
        /// <param name="has_touch">indicates if the device is touch sensitive or not</param>
        /// <param name="hertz">rate for refreshing the display in hz = count/sec</param>
        /// <param name="adapterType">Type of the implemented adapter for this device</param>
        public BrailleIODevice(int size_x, int size_y, String device_name, bool has_keys, bool has_touch, int hertz, Type adapterType)
        {
            this.DeviceSizeX = size_x;
            this.DeviceSizeY = size_y;
            this.Name = device_name;
            this.HasKeys = has_keys;
            this.HasTouch = has_touch;
            this.RefreshRate = hertz;
            this.AdapterType = adapterType.ToString();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIODevice"/> class.
        /// </summary>
        /// <param name="size_x">Count of pins in horizontal direction (width)</param>
        /// <param name="size_y">Count of pins in vertical direction (height)</param>
        /// <param name="device_name">Name of the device (try to keep it unique)</param>
        /// <param name="has_keys">indicates if the device has keys or not</param>
        /// <param name="has_touch">indicates if the device is touch sensitive or not</param>
        /// <param name="hertz">rate for refreshing the display in hz = count/sec</param>
        public BrailleIODevice(int size_x, int size_y, String device_name, bool has_keys, bool has_touch, int hertz)
        {
            this.DeviceSizeX = size_x;
            this.DeviceSizeY = size_y;
            this.Name = device_name;
            this.HasKeys = has_keys;
            this.HasTouch = has_touch;
            this.RefreshRate = hertz;
            this.AdapterType = typeof(Object).ToString();
        }
    }
}
