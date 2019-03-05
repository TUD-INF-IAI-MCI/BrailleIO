using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BrailleIO.Interaction
{
    /// <summary>
    /// Helper class for basic interaction handling on <see cref="BrailleIOViewRange"/> 
    /// or content.
    /// </summary>
    public static class BrailleIOInteractionHelper
    {

        #region Zooming

        #region Zoom by Factor

        /// <summary>
        /// Zooms the specified view by the given factor.
        /// </summary>
        /// <param name="screenName">Name of the screen.</param>
        /// <param name="viewRangeName">Name of the view range inside the screen.</param>
        /// <param name="changeFactor">The change factor for zoom.</param>
        /// <param name="render">if set to <c>true</c> a new rendering is forced.</param>
        /// <returns>
        ///   <c>true</c> if the zoom factor of the view was changed successfully.
        /// </returns>
        /// <remarks>
        /// Views will change their presentation only after calling <see cref="BrailleIOMediator.Instance.RenderDisplay()" />.
        /// Call the <c>RenderDisplay()</c> function after you have done all your changes to see the results.
        /// </remarks>
        public static bool Zoom(string screenName, string viewRangeName, double changeFactor, bool render = false)
        {
            var IO = BrailleIOMediator.Instance;
            if (IO != null)
            {
                var view = IO.GetView(screenName);
                if (view != null)
                {
                    if (view is BrailleIOScreen)
                    {
                        BrailleIOScreen screen = IO.GetView(screenName) as BrailleIOScreen;
                        return Zoom(screen, viewRangeName, changeFactor, render);
                    }
                    else if (view is BrailleIOViewRange)
                    {
                        return Zoom(view as BrailleIOViewRange, changeFactor, render);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Zooms the specified view by the given factor.
        /// </summary>
        /// <param name="screen">The screen containing the view.</param>
        /// <param name="viewRangeName">Name of the view range inside the screen.</param>
        /// <param name="changeFactor">The change factor for zoom.</param>
        /// <param name="render">if set to <c>true</c> a new rendering is forced.</param>
        /// <returns>
        ///   <c>true</c> if the zoom factor of the view was changed successfully.
        /// </returns>
        /// <remarks>
        /// Views will change their presentation only after calling <see cref="BrailleIOMediator.Instance.RenderDisplay()" />.
        /// Call the <c>RenderDisplay()</c> function after you have done all your changes to see the results.
        /// </remarks>
        private static bool Zoom(BrailleIOScreen screen, string viewRangeName, double changeFactor, bool render = false)
        {
            if (screen != null)
            {
                BrailleIOViewRange vr = screen.GetViewRange(viewRangeName);
                if (vr != null)
                    return Zoom(vr, changeFactor, render);
            }
            return false;
        }
        /// <summary>
        /// Zooms the specified view by the given factor.
        /// </summary>
        /// <param name="vr">The view to change the zoom factor.</param>
        /// <param name="changeFactor">The change factor for zoom.</param>
        /// <param name="render">if set to <c>true</c> a new rendering is forced.</param>
        /// <returns>
        ///   <c>true</c> if the zoom factor of the view was changed successfully.
        /// </returns>
        /// <exception cref="System.ArgumentException">Zoom-factor must be not 0! - changeFactor</exception>
        /// <remarks>
        /// Views will change their presentation only after calling <see cref="BrailleIOMediator.Instance.RenderDisplay()" />.
        /// Call the <c>RenderDisplay()</c> function after you have done all your changes to see the results.
        /// </remarks>
        public static bool Zoom(BrailleIOViewRange vr, double changeFactor, bool render = false)
        {
            if (changeFactor == 0.0) throw new ArgumentException("Zoom-factor must be not 0!", "changeFactor");

            if (vr != null)
            {
                if (vr.GetZoom() > 0)
                {
                    var oldZoom = vr.GetZoom();
                    var newZoom = oldZoom * changeFactor;
                    return ZoomTo(vr, newZoom, render);
                }
            }
            return false;
        }

        #endregion

        #region Zoom to Factor

        /// <summary>
        /// Set the Zooms factor of the view to the given factor.
        /// </summary>
        /// <param name="screenName">Name of the screen.</param>
        /// <param name="viewRangeName">Name of the view range inside the screen.</param>
        /// <param name="factor">The new zoom-factor.</param>
        /// <param name="render">if set to <c>true</c> a new rendering is forced.</param>
        /// <returns><c>true</c> if the zoom factor of the view was changed successfully.</returns>
        /// <remarks>Views will change their presentation only after calling <see cref="BrailleIOMediator.Instance.RenderDisplay()"/>.
        /// Call the <c>RenderDisplay()</c> function after you have done all your changes to see the results.</remarks>
        public static bool ZoomTo(string screenName, string viewRangeName, double factor, bool render = false)
        {
            var IO = BrailleIOMediator.Instance;
            if (IO != null)
            {
                var view = IO.GetView(screenName);
                if (view != null)
                {
                    if (view is BrailleIOScreen)
                    {
                        BrailleIOScreen screen = IO.GetView(screenName) as BrailleIOScreen;
                        return ZoomTo(screen, viewRangeName, factor, render);
                    }
                    else if (view is BrailleIOViewRange)
                    {
                        return ZoomTo(view as BrailleIOViewRange, factor, render);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Set the Zooms factor of the view to the given factor.
        /// </summary>
        /// <param name="screen">The screen containing the view.</param>
        /// <param name="viewRangeName">Name of the view range inside the screen.</param>
        /// <param name="factor">The new zoom-factor.</param>
        /// <param name="render">if set to <c>true</c> a new rendering is forced.</param>
        /// <returns>
        ///   <c>true</c> if the zoom factor of the view was changed successfully.
        /// </returns>
        /// <remarks>
        /// Views will change their presentation only after calling <see cref="BrailleIOMediator.Instance.RenderDisplay()" />.
        /// Call the <c>RenderDisplay()</c> function after you have done all your changes to see the results.
        /// </remarks>
        private static bool ZoomTo(BrailleIOScreen screen, string viewRangeName, double factor, bool render = false)
        {
            if (screen != null)
            {
                BrailleIOViewRange vr = screen.GetViewRange(viewRangeName);
                if (vr != null)
                    return Zoom(vr, factor, render);
            }
            return false;
        }
        /// <summary>
        /// Set the Zooms factor of the view to the given factor.
        /// </summary>
        /// <param name="vr">The view to change the zoom factor.</param>
        /// <param name="factor">The new zoom-factor.</param>
        /// <param name="render">if set to <c>true</c> a new rendering is forced.</param>
        /// <returns>
        ///   <c>true</c> if the zoom factor of the view was changed successfully.
        /// </returns>
        /// <exception cref="System.ArgumentException">Zoom-factor must be not 0! - factor</exception>
        /// <remarks>
        /// Views will change their presentation only after calling <see cref="BrailleIOMediator.Instance.RenderDisplay()" />.
        /// Call the <c>RenderDisplay()</c> function after you have done all your changes to see the results.
        /// </remarks>
        public static bool ZoomTo(BrailleIOViewRange vr, double factor, bool render = false)
        {
            if (factor == 0.0) throw new ArgumentException("Zoom-factor must be not 0!", "factor");

            if (vr != null)
            {
                var oldZoom = vr.GetZoom();
                var newZoom = factor;
                var oldvrdin = vr.ViewBox;
                Point oldcenter = new Point(
                    (int)Math.Round(((double)oldvrdin.Width / 2) + (vr.GetXOffset() * -1)),
                    (int)Math.Round(((double)oldvrdin.Height / 2) + (vr.GetYOffset() * -1))
                    );

                Point newCenter = new Point(
                    (int)Math.Round(oldcenter.X * newZoom / oldZoom),
                    (int)Math.Round(oldcenter.Y * newZoom / oldZoom)
                    );

                Point newOffset = new Point(
                    (int)Math.Round((newCenter.X - ((double)oldvrdin.Width / 2)) * -1),
                    (int)Math.Round((newCenter.Y - ((double)oldvrdin.Height / 2)) * -1)
                    );

                vr.SetZoom(newZoom);

                vr.SetXOffset(newOffset.X);
                vr.SetYOffset(newOffset.Y);

                if (render && BrailleIOMediator.Instance != null) BrailleIOMediator.Instance.RenderDisplay();
                return oldZoom != vr.GetZoom();
            }
            return false;
        }

        #endregion

        #endregion


        #region Panning

        #region Horizontal

        /// <summary>
        /// Moves the content of the view in horizontal direction.
        /// </summary>
        /// <param name="screenName">Name of the screen.</param>
        /// <param name="viewRangeName">Name of the view range inside the screen.</param>
        /// <param name="pins">The pins to move in horizontal direction. Negative values will move the content to the right; positive values to the left.</param>
        /// <param name="render">if set to <c>true</c> a new rendering is forced.</param>
        /// <returns>
        /// <c>true</c> if the content was moved successfully.
        /// </returns>
        /// <remarks>
        /// Views will change their presentation only after calling <see cref="BrailleIOMediator.Instance.RenderDisplay()" />.
        /// Call the <c>RenderDisplay()</c> function after you have done all your changes to see the results.
        /// </remarks>
        public static bool MoveHorizontal(string screenName, string viewRangeName, int pins, bool render = false)
        {
            var IO = BrailleIOMediator.Instance;
            if (IO != null)
            {
                var view = IO.GetView(screenName);
                if (view != null)
                {
                    if (view is BrailleIOScreen)
                    {
                        BrailleIOScreen screen = IO.GetView(screenName) as BrailleIOScreen;
                        return MoveHorizontal(screen, viewRangeName, pins, render);
                    }
                    else if (view is BrailleIOViewRange)
                    {
                        return MoveHorizontal(view as BrailleIOViewRange, pins, render);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Moves the content of the view in horizontal direction.
        /// </summary>
        /// <param name="screen">The screen the view is located in.</param>
        /// <param name="viewRangeName">Name of the view range inside the screen.</param>
        /// <param name="pins">The pins to move in horizontal direction. Negative values will move the content to the right; positive values to the left.</param>
        /// <param name="render">if set to <c>true</c> a new rendering is forced.</param>
        /// <returns>
        ///   <c>true</c> if the content was moved successfully.
        /// </returns>
        /// <remarks>
        /// Views will change their presentation only after calling <see cref="BrailleIOMediator.Instance.RenderDisplay()" />.
        /// Call the <c>RenderDisplay()</c> function after you have done all your changes to see the results.
        /// </remarks>
        public static bool MoveHorizontal(BrailleIOScreen screen, string viewRangeName, int pins, bool render = false)
        {
            if (screen != null)
            {
                BrailleIOViewRange vr = screen.GetViewRange(viewRangeName);
                if (vr != null)
                    return MoveHorizontal(vr, pins, render);
            }
            return false;
        }
        /// <summary>
        /// Moves the content of the view in horizontal direction.
        /// </summary>
        /// <param name="vr">The view to move the content of.</param>
        /// <param name="pins">The pins to move in horizontal direction. Negative values will move the content to the right; positive values to the left.</param>
        /// <param name="render">if set to <c>true</c> a new rendering is forced.</param>
        /// <returns>
        ///   <c>true</c> if the content was moved successfully.
        /// </returns>
        /// <remarks>
        /// Views will change their presentation only after calling <see cref="BrailleIOMediator.Instance.RenderDisplay()" />.
        /// Call the <c>RenderDisplay()</c> function after you have done all your changes to see the results.
        /// </remarks>
        public static bool MoveHorizontal(BrailleIOViewRange vr, int pins, bool render = false)
        {
            if (vr != null)
            {
                int oldO = vr.GetXOffset();
                vr.MoveHorizontal(pins);
                if (render && BrailleIOMediator.Instance != null) BrailleIOMediator.Instance.RenderDisplay();
                return oldO != vr.GetXOffset();
            }
            return false;
        }

        #endregion

        #region Vertical

        /// <summary>
        /// Moves the content of the view in vertical direction.
        /// </summary>
        /// <param name="screenName">Name of the screen.</param>
        /// <param name="viewRangeName">Name of the view range inside the screen.</param>
        /// <param name="pins">The pins to move in vertical direction. Negative values will move the content to the right; positive values to the left.</param>
        /// <param name="render">if set to <c>true</c> a new rendering is forced.</param>
        /// <returns>
        /// <c>true</c> if the content was moved successfully.
        /// </returns>
        /// <remarks>
        /// Views will change their presentation only after calling <see cref="BrailleIOMediator.Instance.RenderDisplay()" />.
        /// Call the <c>RenderDisplay()</c> function after you have done all your changes to see the results.
        /// </remarks>
        public static bool MoveVertical(string screenName, string viewRangeName, int pins, bool render = false)
        {
            var IO = BrailleIOMediator.Instance;
            if (IO != null)
            {
                var view = IO.GetView(screenName);
                if (view != null)
                {
                    if (view is BrailleIOScreen)
                    {
                        BrailleIOScreen screen = IO.GetView(screenName) as BrailleIOScreen;
                        return MoveVertical(screen, viewRangeName, pins, render);
                    }
                    else if (view is BrailleIOViewRange)
                    {
                        return MoveVertical(view as BrailleIOViewRange, pins, render);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Moves the content of the view in vertical direction.
        /// </summary>
        /// <param name="screen">The screen the view is located in.</param>
        /// <param name="viewRangeName">Name of the view range inside the screen.</param>
        /// <param name="pins">The pins to move in vertical direction. Negative values will move the content to the right; positive values to the left.</param>
        /// <param name="render">if set to <c>true</c> a new rendering is forced.</param>
        /// <returns>
        ///   <c>true</c> if the content was moved successfully.
        /// </returns>
        /// <remarks>
        /// Views will change their presentation only after calling <see cref="BrailleIOMediator.Instance.RenderDisplay()" />.
        /// Call the <c>RenderDisplay()</c> function after you have done all your changes to see the results.
        /// </remarks>
        public static bool MoveVertical(BrailleIOScreen screen, string viewRangeName, int pins, bool render = false)
        {
            if (screen != null)
            {
                BrailleIOViewRange vr = screen.GetViewRange(viewRangeName);
                if (vr != null)
                    return MoveVertical(vr, pins, render);
            }
            return false;
        }
        /// <summary>
        /// Moves the content of the view in vertical direction.
        /// </summary>
        /// <param name="vr">The view to move the content of.</param>
        /// <param name="pins">The pins to move in vertical direction. Negative values will move the content to the right; positive values to the left.</param>
        /// <param name="render">if set to <c>true</c> a new rendering is forced.</param>
        /// <returns>
        ///   <c>true</c> if the content was moved successfully.
        /// </returns>
        /// <remarks>
        /// Views will change their presentation only after calling <see cref="BrailleIOMediator.Instance.RenderDisplay()" />.
        /// Call the <c>RenderDisplay()</c> function after you have done all your changes to see the results.
        /// </remarks>
        public static bool MoveVertical(BrailleIOViewRange vr, int pins, bool render = false)
        {
            if (vr != null)
            {
                int oldO = vr.GetYOffset();
                vr.MoveVertical(pins);
                if (render && BrailleIOMediator.Instance != null) BrailleIOMediator.Instance.RenderDisplay();
                return oldO != vr.GetYOffset();
            }
            return false;
        }

        #endregion

        #endregion
    }
}
