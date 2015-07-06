using System;
using System.Collections;
using System.Collections.Generic;
using BrailleIO.Renderer.Structs;

namespace BrailleIO.Renderer
{
    public partial class MatrixBrailleRenderer : ITouchableRenderer
    {

        #region RenderElements

        /// <summary>
        /// List of rendered elements - used to get feedback about positions and structure
        /// </summary>
        LinkedList<RenderElement> elements = new LinkedList<RenderElement>();

        /// <summary>
        /// Reset the list of rendered elements
        /// </summary>
        void clearRenderElements()
        {
            elements.Clear();
        }

        //TODO: lock this against rendering

        private readonly Object _rendererLock = new Object();
        /// <summary>
        /// Get a list of the rendered element structures.
        /// </summary>
        /// <returns>A copy of the rendered elememt list</returns>
        public LinkedList<RenderElement> GetAllRenderElements()
        {
            lock (_rendererLock)
            {
                return new LinkedList<RenderElement>(elements); 
            }
        }

        #endregion

        #region ITouchableRenderer

        /// <summary>
        /// Return the first found RenderElement
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public object GetContentAtPosition(int x, int y)
        {
            if (elements != null && elements.Count > 0)
            {
                foreach (RenderElement e in elements)
                {
                    if (e.ContainsPoint(x, y))
                    {
                        return e;
                    }
                } 
            }

            return null;
        }


        /// <summary>
        /// Get all Objects inside (or at least partial) the given area.
        /// </summary>
        /// <param name="left">Left border of the region to test (X).</param>
        /// <param name="right">Right border of the region to test (X + width).</param>
        /// <param name="top">Top border of the region to test (Y).</param>
        /// <param name="bottom">Bottom border of the region to test (Y + heigh).</param>
        /// <returns>
        /// A list of elements inside or at least partial inside the requested area.
        /// </returns>
        public IList GetAllContentInArea(int left, int right, int top, int bottom)
        {
            List<RenderElement> eL = new List<RenderElement>();

            if (elements != null && elements.Count > 0)
            {
                foreach (RenderElement e in elements)
                {
                    if (e.IsInArea(left, right, top, bottom))
                    {
                        eL.Add(e);
                    }
                }
            }

            return eL;
        }

        #endregion
    
    }

    /// <summary>
    /// Enum to specify the RenderElements
    /// </summary>
    public enum BrailleRendererPartType
    {
        NONE,
        UNKOWN,
        DOCUMENT,
        PAGE,
        PARAGRAPH,
        LINE,
        WORD,
        WORD_PART,
        CHAR
    }


}
