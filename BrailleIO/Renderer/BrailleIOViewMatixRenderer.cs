using System;
using System.Drawing;
using BrailleIO.Interface;

namespace BrailleIO.Renderer
{
    /// <summary>
    /// Place a content-matrix in a matrix that fits in a given view with aware of the BoxModel.
    /// </summary>
		/// <remarks> </remarks>
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
		/// <remarks> </remarks>
        /// <param name="view">The view witch holds the BoxModel. If the view is IPannable than the offset is also considered.</param>
        /// <param name="contentMatrix">The content matrix. Holds the content that should be placed in the view.</param>
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
		/// <remarks> </remarks>
        /// <param name="view">The view with holds the BoxModel. If the view is IPannable than the offset is also considered.</param>
        /// <param name="contentMatrix">The content matrix. Holds the content that should be placed in the view.</param>
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
            int b = Math.Abs(view.ViewBox.Bottom - (cb.Bottom-(cb.Height + cb.Y))); //TODO: semms to be wrong

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

                //for (int x = 0; x < cb.Width; x++)
                System.Threading.Tasks.Parallel.For(0, cb.Width, x =>
                {
                    int cX = oX + x;
                    if (cX >= 0 && contentMatrix.GetLength(1) > cX)
                    {
                        for (int y = 0; y < cb.Height; y++)
                        //System.Threading.Tasks.Parallel.For(0, cb.Height, y =>
                        {
                            int cY = oY + y;
                            if (cY >= 0 && contentMatrix.GetLength(0) > cY)
                            {
                                if ((x + l) >= 0 && (y + t) >= 0)
                                {
                                    viewMatrix[y + t, x + l] = contentMatrix[cY, cX];
                                }
                            }
                        }//);
                    }
                });
            }

            //call post hooks
            callAllPostHooks(view, contentMatrix, ref viewMatrix, handlePanning);

            return viewMatrix;
        }

        /// <summary>
        /// Renders a content object into an boolean matrix;
        /// while <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="view">The frame to render in. This gives access to the space to render and other parameters. Normally this is a <see cref="BrailleIOViewRange"/>.</param>
        /// <param name="content">The content to render.</param>
        /// <returns>
        /// A two dimensional boolean M x N matrix (bool[M,N]) where M is the count of rows (this is height)
        /// and N is the count of columns (which is the width).
        /// Positions in the Matrix are of type [i,j]
        /// while i is the index of the row (is the y position)
        /// and j is the index of the column (is the x position).
        /// In the matrix <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// </returns>
        public bool[,] RenderMatrix(IViewBoxModel view, object content)
        {
            return RenderMatrix(view, content as bool[,]);
        }
    }
}
