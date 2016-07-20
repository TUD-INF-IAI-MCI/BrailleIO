using BrailleIO.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Renderer
{
    /// <summary>
    /// Interface for caching rendering results.
    /// </summary>
    public interface ICacheingRenderer
    {

        /// <summary>
        /// Gets or sets a value indicating whether content changed or not to check if a new rendering is necessary.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [content has changed]; otherwise, <c>false</c>.
        /// </value>
        bool ContentChanged { get; set; }
        /// <summary>
        /// Gets the time stamp for the last content rendering.
        /// </summary>
        /// <value>
        /// The last time stamp of content rendering rendered.
        /// </value>
        DateTime LastRendered { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is currently rendering.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is currently rendering; otherwise, <c>false</c>.
        /// </value>
        bool IsRendering { get; }

        /// <summary>
        /// Informs the renderer that the content the or view has changed.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="content">The content.</param>
        void ContentOrViewHasChanged(IViewBoxModel view, object content);

        /// <summary>
        /// Renders the current content
        /// </summary>
        void PrerenderMatrix(IViewBoxModel view, object content);

        /// <summary>
        /// Gets the previously rendered and cached matrix.
        /// </summary>
        /// <returns>The cached rendering result</returns>
        bool[,] GetCachedMatrix();

    }
}
