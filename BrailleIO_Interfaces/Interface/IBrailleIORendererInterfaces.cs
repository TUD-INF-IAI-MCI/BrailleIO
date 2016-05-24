
namespace BrailleIO.Interface
{
    public interface IBrailleIORendererInterfaces : IBrailleIOContentRenderer
    {
        /// <summary>
        /// Renders a content object into an boolean matrix;
        /// while <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// </summary>
        /// <param name="view">The frame to render in. This gives access to the space to render and other parameters. Normally this is a <see cref="BrailleIOViewRange"/>.</param>
        /// <param name="matrix">The content to render.</param>
        /// <returns>
        /// A two dimensional boolean M x N matrix (bool[M,N]) where M is the count of rows (this is height)
        /// and N is the count of columns (which is the width). 
        /// Positions in the Matrix are of type [i,j] 
        /// while i is the index of the row (is the y position) 
        /// and j is the index of the column (is the x position). 
        /// In the matrix <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins</returns>
        bool[,] RenderMatrix(IViewBoxModel view, bool[,] matrix);
    }

    /// <summary>
    /// Generic interface for a renderer for content of a <see cref="BrailleIOViewRange"/> has to implement. 
    /// Then the instance can be added as a content renderer.
    /// </summary>
    public interface IBrailleIOContentRenderer
    {
        /// <summary>
        /// Renders a content object into an boolean matrix;
        /// while <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// </summary>
        /// <param name="view">The frame to render in. This gives access to the space to render and other parameters. Normally this is a <see cref="BrailleIOViewRange"/>.</param>
        /// <param name="content">The content to render.</param>
        /// <returns>
        /// A two dimensional boolean M x N matrix (bool[M,N]) where M is the count of rows (this is height)
        /// and N is the count of columns (which is the width). 
        /// Positions in the Matrix are of type [i,j] 
        /// while i is the index of the row (is the y position) 
        /// and j is the index of the column (is the x position). 
        /// In the matrix <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins</returns>
        bool[,] RenderMatrix(IViewBoxModel view, object content);
    }

    #region Hook

    /// <summary>
    /// Interface that a renderer has to implement if he wants to allow hooking
    /// </summary>
    public interface IBrailleIOHookableRenderer
    {
        /// <summary>
        /// Register a hook.
        /// </summary>
        /// <param name="hook">The hook.</param>
        void RegisterHook(IBailleIORendererHook hook);
        /// <summary>
        /// Unregisters a hook.
        /// </summary>
        /// <param name="hook">The hook.</param>
        void UnregisterHook(IBailleIORendererHook hook);
    }

    /// <summary>
    /// Interface a renderer hook has to implement
    /// </summary>
    public interface IBailleIORendererHook
    {
        /// <summary>
        /// This hook function is called by an IBrailleIOHookableRenderer before he starts his rendering.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="content">The content.</param>
        /// <param name="additionalParams">Additional parameters.</param>
        void PreRenderHook(ref IViewBoxModel view, ref object content, params object[] additionalParams);
        /// <summary>
        /// This hook function is called by an IBrailleIOHookableRenderer after he has done his rendering before returning the result.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="content">The content.</param>
        /// <param name="result">The result matrix, may be manipulated. Addressed in [y, x] notation.</param>
        /// <param name="additionalParams">Additional parameters.</param>
        void PostRenderHook(IViewBoxModel view, object content, ref bool[,] result, params object[] additionalParams);
    }


    #endregion
}
