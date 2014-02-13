using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//TODO: is that necessary? 
namespace BrailleIO
{
    public class BrailleIOTouchPoint
    {
        private int offset_x
        {
            get;
            set;
        }
        private int offset_y
        {
            get;
            set;
        }
        private bool[,] point
        {
            get;
            set;
        }

        public BrailleIOTouchPoint(int offset_x, int offset_y, bool[,] point)
        {
            this.offset_x = offset_x;
            this.offset_y = offset_y;
            this.point = point;
        }
    }

    public class BrailleIOTouchPath
    {

        private List<BrailleIOTouchPoint> touchpath = new List<BrailleIOTouchPoint>();
        private BrailleIOTouchPoint touchpoint;
        private int interval = 200;
        //TODO: Event Listener

        public BrailleIOTouchPath()
        {
            // TODO: EventListener subscribe

        }

        internal void pushPoint(BrailleIOTouchPoint p)
        {
            touchpath.Add(p);
            this.touchpoint = p;
        }

        public BrailleIOTouchPoint getCurrentTouchPoint()
        {
            return this.touchpoint;
        }

        public List<BrailleIOTouchPoint> getTouchPath()
        {
            return this.touchpath;
        }

        public List<BrailleIOTouchPoint> stopTouch()
        {
            return this.touchpath;
        }

        //TODO: generic EventHandler
    }
}
