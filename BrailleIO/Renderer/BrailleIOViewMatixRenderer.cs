﻿using BrailleIO.Interface;
using System.Drawing;
using System;

namespace BrailleIO.Renderer
{
    /// <summary>
    /// Place a content-matrix in a matrix that fits in a given view with aware of the BoxModel.
    /// </summary>
    public class BrailleIOViewMatixRenderer : BrailleIOHookableRendererBase, IBrailleIORendererInterfaces
    {
        /// <summary>
        /// Puts the given content-matrix in a matrix that fits in the given view.
        /// The content-matrix placement is with aware of the given Box model and panning offsets.
        /// Borders are not rendered. If the content-matrix don't fit in the
        /// view, the overlapping content is ignored.
        /// If the content-matrix is smaller than the view, the rest is set to false.
        /// This renderer takes also care about the panning, which is set in the view if they is IPannable.
        /// </summary>
        /// <param name="view">The view witch holds the BoxModel. If the view is IPannable than the offset is also considered.</param>
        /// <param name="contentMatrix">The content matrix. Holds the conten that should be placed in the view.</param>
        /// <returns>a bool[view.ViewBox.Width,view.ViewBox.Height] matrix holding the content with aware of the views' BoxModel.</returns>
        public bool[,] RenderMatrix(IViewBoxModel view, bool[,] contentMatrix) { return RenderMatrix(view, contentMatrix, false); }

        /// <summary>
        /// Puts the given content-matrix in a matrix that fits in the given view.
        /// The content-matrix placement is with aware of the given Box model.
        /// Borders are not rendered. If the content-matrix don't fit in the
        /// view, the overlapping content is ignored.
        /// If the content-matrix is smaller than the view, the rest is set to false.
        /// This renderer takes also care about the panning, which is set in the view if they is IPannable.
        /// </summary>
        /// <param name="view">The view with holds the BoxModel. If the view is IPannable than the offset is also considered.</param>
        /// <param name="contentMatrix">The content matrix. Holds the conten that should be placed in the view.</param>
        /// <param name="handlePanning">Handle the panning of the content matrix or not</param>
        /// <returns>a bool[view.ViewBox.Width,view.ViewBox.Height] matrix holding the content with aware of the views' BoxModel.</returns>
        public bool[,] RenderMatrix(IViewBoxModel view, bool[,] contentMatrix, bool handlePanning)
        {
            //call pre hooks
            object cM = contentMatrix as object;
            callAllPreHooks(ref view, ref cM, handlePanning);
            contentMatrix = cM as bool[,];


            if (view == null) return null;
            bool[,] viewMatrix = new bool[view.ViewBox.Height, view.ViewBox.Width];

            Rectangle cb = view.ContentBox;
            int t = cb.Y;
            int l = cb.X;

            int r = view.ViewBox.Width - view.ViewBox.Right;
            int b = Math.Abs(view.ViewBox.Bottom - cb.Bottom);

            int oX = 0;
            int oY = 0;

            if (view is IPannable)
            {
                if (handlePanning)
                {
                    oX = ((IPannable)view).GetXOffset() * -1;
                    oY = ((IPannable)view).GetYOffset() * -1;
                }
                if (((IPannable)view).ShowScrollbars)
                {
                    bool scucess = BrailleIOScrollbarRenderer.DrawScrollbars(view, ref viewMatrix, oX, oY);
                }
            }

            if (contentMatrix != null)
            {

                int cw = contentMatrix.GetLength(1);
                int ch = contentMatrix.GetLength(0);

                System.Threading.Tasks.Parallel.For(0, viewMatrix.GetLength(1) - l, x =>
                {
                    int cX = oX + x;
                    if (cX >= 0 && contentMatrix.GetLength(1) > cX)
                    {
                        System.Threading.Tasks.Parallel.For(0, viewMatrix.GetLength(0) - t, y =>
                        {
                            int cY = oY + y;
                            if (cY >= 0 && contentMatrix.GetLength(0) > cY)
                            {
                                if ((x + l) >= 0 && (y + t) >= 0)
                                {
                                    viewMatrix[y + t, x + l] = contentMatrix[cY, cX];
                                }
                            }
                        });
                    }
                });
            }

            //call post hooks
            callAllPostHooks(view, contentMatrix, ref viewMatrix, handlePanning);

            return viewMatrix;
        }

        public bool[,] RenderMatrix(IViewBoxModel view, object content)
        {
            return RenderMatrix(view, content as bool[,]);
        }
    }
}
