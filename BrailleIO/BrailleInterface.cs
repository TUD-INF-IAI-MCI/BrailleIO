using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
namespace TangramSkApp
{
    class BrailleInterface
    {
        int device_width = 0;
        int device_height = 0;
        int device_hertz = 0;
        OrderedDictionary regions = new OrderedDictionary();

        #region Device Information


        #endregion


        


        #region Input

        #endregion



        #region Output
        private bool _setMatrix(float[,] matrix) // Applying on Device
        {

            return true;
        }

        public bool setMatrix(float[,] matrix)
        {

            return true;
        }

        public bool setAllPins(float value)
        {
            float[,] matrix = new float[device_width, device_height];
            for (int i = 0; i < device_width; i++)
                for (int j = 0; j < device_height; j++)
                    matrix[device_width, device_height] = value;
            return _setMatrix(matrix);
        }



        public void deleteRegion(String name)
        {
            regions.Remove(name);
        }

        public void defineRegion(String name, int x_offset, int y_offset, int width, int height)
        {
            if (regions.Contains(name))
            {
                regions[name] = new ViewRegion(x_offset, y_offset, width, height, device_width, device_height);
            }
            else
            {
                regions.Add(name, new ViewRegion(x_offset, y_offset, width, height, device_width, device_height));
            }
        }

        public bool setRegion(String name, float[,] matrix)
        {
            return ((ViewRegion)regions[name]).setMatrix(matrix);
        }

        public ViewRegion getRegion(String name)
        {
            if (regions.Contains(name))
                return ((ViewRegion)regions[name]);
            else
                return null;
        }

        public OrderedDictionary getAllRegions()
        {
            return regions;
        }

        public bool setAllPinsDown()
        {
            return setAllPins(0);
        }

        public bool setAllPinsUp()
        {
            return setAllPins(1);
        }


        #endregion
    }
}
