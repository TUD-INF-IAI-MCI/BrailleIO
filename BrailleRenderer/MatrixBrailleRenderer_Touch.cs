using System;
using System.Collections;
using System.Collections.Generic;
using BrailleIO.Renderer.Structs;

namespace BrailleIO.Renderer
{
    /// <summary>
    /// Renderer for transforming strings int a Braille text bool matrix representation.
    /// </summary>
		/// <remarks> </remarks>
    /// <seealso cref="BrailleIO.Renderer.AbstractCachingRendererBase" />
    /// <seealso cref="BrailleIO.Renderer.ITouchableRenderer" />
    public partial class MatrixBrailleRenderer : ITouchableRenderer
    {

        #region RenderElements

        /// <summary>
        /// List of rendered elements - used to get feedback about positions and structure
        /// </summary>
		/// <remarks> </remarks>
        LinkedList<RenderElement> elements = new LinkedList<RenderElement>();

        /// <summary>
        /// Reset the list of rendered elements
        /// </summary>
		/// <remarks> </remarks>
        void clearRenderElements()
        {
            elements.Clear();
        }

        //TODO: lock this against rendering

        private readonly Object _rendererLock = new Object();
        /// <summary>
        /// Get a list of the rendered element structures.
        /// </summary>
		/// <remarks> </remarks>
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

        /// <summary>Return the first found RenderElement</summary>
        /// <param name="x">The x position in the content matrix.</param>
        /// <param name="y">The y position in the content matrix.</param>
        /// <returns>An object at the requester position in the content or <c>null</c></returns>
        public object GetContentAtPosition(int x, int y)
        {
            LinkedList<RenderElement> elementsCopy = GetAllRenderElements();
            if (elementsCopy != null && elementsCopy.Count > 0)
            {
                foreach (RenderElement e in elementsCopy)
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
		/// <remarks> </remarks>
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
            LinkedList<RenderElement> elementsCopy = GetAllRenderElements();
            
            if (elementsCopy != null && elementsCopy.Count > 0)
            {
                foreach (RenderElement e in elementsCopy)
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
		/// <remarks> </remarks>
    public enum BrailleRendererPartType
    {
        /// <summary>
        /// nothing
        /// </summary>
		/// <remarks> </remarks>
        NONE,
        /// <summary>
        /// unknown
        /// </summary>
		/// <remarks> </remarks>
        UNKNOWN,
        /// <summary>
        /// The basic document
        /// </summary>
		/// <remarks> </remarks>
        DOCUMENT,
        /// <summary>
        /// A page
        /// </summary>
		/// <remarks> </remarks>
        PAGE,
        /// <summary>
        /// A paragraph
        /// </summary>
		/// <remarks> </remarks>
        PARAGRAPH,
        /// <summary>
        /// A line of text inside a paragraph
        /// </summary>
		/// <remarks> </remarks>
        LINE,
        /// <summary>
        /// A word
        /// </summary>
		/// <remarks> </remarks>
        WORD,
        /// <summary>
        /// The part of a word - if it is divided
        /// </summary>
		/// <remarks> </remarks>
        WORD_PART,
        /// <summary>
        /// a character
        /// </summary>
		/// <remarks> </remarks>
        CHAR
    }

}
