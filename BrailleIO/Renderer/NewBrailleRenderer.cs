using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrailleIO.Interface;
using BrailleRenderer.BrailleInterpreter;
using BrailleRenderer;

namespace BrailleIO.Renderer
{
    class NewBrailleRenderer : BrailleIOHookableRendererBase, IBrailleIOContentRenderer
    {

        #region Members

        readonly IBraileInterpreter brailleInterpreter;
        readonly MatrixBrailleRenderer renderer;

        #endregion

        public NewBrailleRenderer()
        {
            brailleInterpreter = new SimpleBrailleInterpreter();
            renderer = new MatrixBrailleRenderer(brailleInterpreter);
        }


        #region IBrailleIOContentRenderer

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
