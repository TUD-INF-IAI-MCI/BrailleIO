using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrailleIO.Interface;
using BrailleRenderer.BrailleInterpreter;
using BrailleRenderer;

namespace BrailleIO.Renderer
{
    /// <summary>
    /// A new Basic Braille renderer wrapping the external BrailleRendereProject so it can be used as default text renderer 
    /// </summary>
    class NewBrailleRenderer : BrailleIOHookableRendererBase, IBrailleIOContentRenderer
    {

        #region Members

        /// <summary>
        /// Interpreter for wrapping string in Braille dot pattern and vise versa
        /// </summary>
        readonly IBraileInterpreter brailleInterpreter;
        /// <summary>
        /// the new basic braille renderer
        /// </summary>
        readonly MatrixBrailleRenderer renderer;

        #endregion

        public NewBrailleRenderer()
        {
            brailleInterpreter = new SimpleBrailleInterpreter();
            renderer = new MatrixBrailleRenderer(brailleInterpreter);
        }


        #region IBrailleIOContentRenderer

        /// <summary>
        /// Renders a content object into an boolean matrix;
        /// while <c>true</c> values indicating raised pins and <c>false</c> values indicating lowerd pins
        /// </summary>
        /// <param name="view">The frame to render in. This gives acces to the space to render and other paramenters. Normaly this is a <see cref="BrailleIOViewRange"/>.</param>
        /// <param name="content">The content to render.</param>
        /// <returns>
        /// A two dimensional boolean M x N matrix (bool[M,N]) where M is the count of rows (this is height)
        /// and N is the count of columns (which is the width).
        /// Positions in the Matrix are of type [i,j]
        /// while i is the index of the row (is the y position)
        /// and j is the index of the column (is the x position).
        /// In the matrix <c>true</c> values indicating raised pins and <c>false</c> values indicating lowerd pins
        /// </returns>
        public bool[,] RenderMatrix(IViewBoxModel view, object content)
        {
            callAllPreHooks(ref view, ref content);

            if (renderer != null)
            {

                int width = view.ContentBox.Width;
                if (renderer != null)
                {
                    bool scrolleBars = false;

                    //if (view is IPannable && ((IPannable)view).ShowScrollbars)
                    //{
                    //    if (renderer.EstimateNeedOfScrollBar(content.ToString(), view.ContentBox.Width, view.ContentBox.Height))
                    //    {
                    //        //scrolleBars = true;
                    //    }
                    //}

                    var matrix = renderer.RenderMatrix(width, content, scrolleBars);

                    view.ContentHeight = matrix.GetLength(0);
                    view.ContentWidth = matrix.GetLength(1);

                    callAllPostHooks(view, content, ref matrix);

                    return matrix;
                }
            }

            return null;
        }

        #endregion
    }
}
