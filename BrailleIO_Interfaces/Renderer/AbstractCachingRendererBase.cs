﻿using BrailleIO.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BrailleIO.Renderer
{
    /// <summary>
    /// Abstract implementation for renderer that allow for a caching. 
    /// This renderer can hold a prerendered result. If the content doesn't change and a 
    /// rendering request force them for a rendering, they will return the cached result 
    /// without any new rendering.
    /// </summary>
    /// <remarks> </remarks>
    /// <seealso cref="BrailleIO.Interface.BrailleIOHookableRendererBase" />
    /// <seealso cref="BrailleIO.Renderer.ICacheingRenderer" />
    /// <seealso cref="BrailleIO.Interface.IBrailleIORendererInterfaces" />
    public abstract class AbstractCachingRendererBase : BrailleIOHookableRendererBase, ICacheingRenderer, IBrailleIOPanningRendererInterfaces
    {

        #region Members

        /// <summary>
        /// Gets or sets a value indicating whether [call hooks on rendering for caching].
        /// </summary>
        /// <value>
        /// <c>true</c> if [call hooks on rendering for caching]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CallHooksOnCacherendering { get; set; }

        /// <summary>
        /// The cached rendered result matrix
        /// </summary>
        protected bool[,] _cachedMatrix;

        /// <summary>
        /// The last view used for rendering.
        /// </summary>
        protected String lastView;
        /// <summary>
        /// The last content
        /// </summary>
        protected object lastContent;

        /// <summary>
        /// The rendering wait timeout in ms for checking if it is currently rendering.
        /// </summary>
        protected int renderingWaitTimeout = 5;

        /// <summary>
        /// The maximum attempts for waiting for completing the rendering.
        /// </summary>
        protected int maxRenderingWaitTrys = 10;

        #endregion

        #region ICacheingRenderer

        /// <summary>
        /// Gets or sets a value indicating whether content changed or not to check if a new rendering is necessary.
        /// You have to call the PrerenderMatrix function manually if you want to have a cached result.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [content has changed]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool ContentChanged { get; set; }

        /// <summary>
        /// Gets the time stamp for the last content rendering.
        /// </summary>
        /// <value>
        /// The last time stamp of content rendering rendered.
        /// </value>
        public virtual DateTime LastRendered { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this instance is currently rendering.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is currently rendering; otherwise, <c>false</c>.
        /// </value>
        public bool IsRendering { get; protected set; }

        /// <summary>
        /// Informs the renderer that the content the or view has changed.
        /// You have to call the PrerenderMatrix function manually if you want to have a cached result.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="content">The content.</param>
        public virtual void ContentOrViewHasChanged(IViewBoxModel view, object content)
        {
            string viewString = viewToString(view);
            // if (!ViewBoxModelEquals(lastView, view))
            contentOrViewHasChanged(viewString, content);
        }

        private void contentOrViewHasChanged(string viewString, object content)
        {
            lastView = viewString; //Clone(view);
            lastContent = content;
            ContentChanged = true;
        }

        /// <summary>Renders the current content</summary>
        /// <param name="view">the view to render the content for</param>
        /// <param name="content">the content to render</param>
        public virtual void PrerenderMatrix(IViewBoxModel view, object content)
        {
            int trys = 0;
            Task t = new Task(() =>
            {
                while (IsRendering && trys++ < maxRenderingWaitTrys) { Thread.Sleep(renderingWaitTimeout); }
                IsRendering = true;
                ContentChanged = false;
                _cachedMatrix = renderMatrix(view, content, CallHooksOnCacherendering);
                LastRendered = DateTime.Now;
                IsRendering = false;
            });
            t.Start();
        }

        /// <summary>
        /// the rendering method used from the prerendering and rendering methods to produce the caching result.
        /// </summary>
        /// <param name="view">The view range</param>
        /// <param name="content">the content object to render</param>
        /// <param name="CallHooksOnCacherendering">flag determining if the hooks should be called or not while rendering</param>
        /// <returns>the rendering result.</returns>
        protected abstract bool[,] renderMatrix(IViewBoxModel view, object content, bool CallHooksOnCacherendering);


        /// <summary>
        /// Gets the previously rendered and cached matrix.
        /// </summary>
        /// <returns>
        /// The cached rendering result
        /// </returns>
        public virtual bool[,] GetCachedMatrix()
        {
            int trys = 0;
            while (IsRendering && trys++ < maxRenderingWaitTrys) { Thread.Sleep(renderingWaitTimeout); }
            //TODO: throw warning or something?!
            return _cachedMatrix;
        }

        #endregion

        #region IBrailleIORendererInterfaces

        /// <summary>
        /// Renders a content object into an boolean matrix;
        /// while <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// </summary>
        /// <param name="view">The frame to render in. This gives access to the space to render and other parameters. Normally this is a BrailleIOViewRange.</param>
        /// <param name="matrix">The content to render.</param>
        /// <returns>
        /// A two dimensional boolean M x N matrix (bool[M,N]) where M is the count of rows (this is height)
        /// and N is the count of columns (which is the width).
        /// Positions in the Matrix are of type [i,j]
        /// while i is the index of the row (is the y position)
        /// and j is the index of the column (is the x position).
        /// In the matrix <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// </returns>
        public virtual bool[,] RenderMatrix(IViewBoxModel view, bool[,] matrix)
        {
            return RenderMatrix(view, matrix, true);
        }

        /// <summary>
        /// Renders a content object into an boolean matrix;
        /// while <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// </summary>
        /// <param name="view">The frame to render in. This gives access to the space to render and other parameters. Normally this is a BrailleIOViewRange.</param>
        /// <param name="content">The content to render.</param>
        /// <returns>
        /// A two dimensional boolean M x N matrix (bool[M,N]) where M is the count of rows (this is height)
        /// and N is the count of columns (which is the width).
        /// Positions in the Matrix are of type [i,j]
        /// while i is the index of the row (is the y position)
        /// and j is the index of the column (is the x position).
        /// In the matrix <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// </returns>
        public virtual bool[,] RenderMatrix(IViewBoxModel view, object content)
        {
            callAllPreHooks(ref view, ref content, null);

            ContentChanged = DidContentOrViewHaveChanged(view, content);

            if (ContentChanged)
            {
                _cachedMatrix = renderMatrix(view, content, CallHooksOnCacherendering);
                LastRendered = DateTime.Now;
                ContentChanged = false;
            }

            bool[,] output = (GetCachedMatrix() != null) ? GetCachedMatrix().Clone() as bool[,] : new bool[0, 0];
            callAllPostHooks(view, content, ref output, null);


            view.ContentHeight = output.GetLength(0);
            view.ContentWidth = output.GetLength(1);

            return output;
        }
        
        /// <summary>
        /// Determine if the content or some important variables of the view have changed or not.
        /// </summary>
        /// <param name="view">The view the content is presented in.</param>
        /// <param name="content">The content to render.</param>
        /// <returns><c>true</c> if some important variables has been changed since the last rendering; otherwise, <c>false</c>.</returns>
        protected virtual bool DidContentOrViewHaveChanged(IViewBoxModel view, object content)
        {
            string viewString = viewToString(view);
            if (!viewString.Equals(lastView))// !ViewBoxModelEquals(lastView, view))
            {
                contentOrViewHasChanged(viewString, content);
            }
            else if (!lastContent.Equals(content))
            {
                ContentOrViewHasChanged(view, content);
            }
            return ContentChanged;
        }


        /// <summary>
        /// Renders a content object into an boolean matrix;
        /// while <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// ATTENTION: have to be implemented. check for the 
        /// </summary>
        /// <param name="view">The frame to render in. This gives access to the space to render and other parameters. Normally this is a BrailleIOViewRange.</param>
        /// <param name="content">The content to render.</param>
        /// <param name="callHooks">if set to <c>true</c> [call the pre- and post-rendering hooks].</param>
        /// <returns>
        /// A two dimensional boolean M x N matrix (bool[M,N]) where M is the count of rows (this is height)
        /// and N is the count of columns (which is the width).
        /// Positions in the Matrix are of type [i,j]
        /// while i is the index of the row (is the y position)
        /// and j is the index of the column (is the x position).
        /// In the matrix <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual bool[,] RenderMatrix(IViewBoxModel view, object content, bool callHooks = true)
        {
            if (callHooks) return RenderMatrix(view, content);
            else
            {
                var cache = GetCachedMatrix();
                if (cache == null)
                {
                    PrerenderMatrix(view, content);
                }

                bool[,] output = (GetCachedMatrix() != null) ? GetCachedMatrix().Clone() as bool[,] : new bool[0, 0];
                return output;
            }
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Determines if the two <see cref="IViewBoxModel"/> are equal or not.
        /// </summary>
        /// <param name="a">One IViewBoxModel</param>
        /// <param name="b">Another IViewBoxModel</param>
        /// <returns><c>true</c> if both IViewBoxModels are equal; otherwise, <c>false</c>.</returns>
        public static bool ViewBoxModelEquals(IViewBoxModel a, IViewBoxModel b)
        {
            if (a != null && b != null
                && a.ContentBox.Equals(b.ContentBox)
                && a.ViewBox.Equals(b.ViewBox)
                && a.ContentHeight == b.ContentHeight
                && a.ContentWidth == b.ContentWidth
                )
            {
                if (a is IZoomable && b is IZoomable
                    && ((IZoomable)a).GetZoom() != ((IZoomable)b).GetZoom()
                    )
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Function to turn the view into a 
        /// </summary>
        /// <remarks> </remarks>
        /// <param name="a"></param>
        /// <returns>a string representing the main features for the view range</returns>
        protected virtual string viewToString(IViewBoxModel a)
        {
            string unique = string.Empty;

            if (a != null)
            {
                unique = a.ContentBox.Width + "," + a.ContentBox.Height + "," + a.ContentBox.X + "," + a.ContentBox.Y +
                     "_" + a.ViewBox.Width + "," + a.ViewBox.Height + "," + a.ViewBox.X + "," + a.ViewBox.Y +
                     "_" + a.ContentHeight + a.ContentWidth
                    ;

                if (a is IZoomable) unique += "_" + ((IZoomable)a).GetZoom();
            }

            return unique;
        }


        #endregion

        #region IBrailleIOPanningRendererInterfaces

        bool _doesPanning = false;
        /// <summary>
        /// Indicates to the combining renderer if this renderer handles panning by its own or not.
        /// <c>true</c> means the renderer has already handled panning (offsets) and returns the correct result.
        /// <c>false</c> means the render does not handle panning (offset), returns the whole rendering result
        /// and the combination renderer has to take care about the panning (offsets)
        /// </summary>
        /// <value>
        ///   <c>true</c> if the renderer does handle panning by it self; otherwise, <c>false</c>.</value>
        public virtual bool DoesPanning
        {
            get { return _doesPanning; }
            protected set { _doesPanning = value; }
        }

        #endregion
    }
}
