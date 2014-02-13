using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TangramSkApp
{
    class ViewRegion
    {
        int x_offset;
        int y_offset;
        int width;
        int height;
        int device_width;
        int device_height;
        float[,] matrix;

        public ViewRegion(int x_offset, int y_offset, int width, int height,int device_width, int device_height)
        {
            this.x_offset = x_offset;
            this.y_offset = y_offset;
            this.width = width;
            this.height = height;
            this.device_width = device_width;
            this.device_height = device_height;
        }

        public float[,] getMatrix()
        {
            return matrix;
        }

        public bool setMatrix(float[,] matrix)
        {
            if (matrix.GetLength(0) == this.width && matrix.GetLength(1) == this.height)
            {
                this.matrix = matrix;
                return true;
            }
            else return false;
        }


        public int getXOffset()
        {
            return x_offset;
        }

        public int getYOffset()
        {
            return y_offset;
        }

        public void setXOffset(int value)
        {
            if (value < device_width)
                x_offset = value;
            if (x_offset + width > device_width)
                width = device_width - x_offset;
        }

        public void setYOffset(int value)
        {
            if (value < device_height)
                y_offset = value;
            if (y_offset + height > device_height)
                width = device_height - y_offset;
        }



        public int getWidth()
        {
            return width;
        }

        public void setWidth(int value)
        {
            if (x_offset + value < device_width)
                width = value;
            else
                width = device_width - x_offset;
        }

        public int getHeight()
        {
            return height;
        }

        public void setHeight(int value)
        {
            if (y_offset + value < device_height)
                height = value;
            else
                height = device_width - y_offset;
        }

    }
}
