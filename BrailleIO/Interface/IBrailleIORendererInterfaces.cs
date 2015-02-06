using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace BrailleIO.Interface
{
    public interface IBrailleIORendererInterfaces : IBrailleIOContentRenderer
    {
        bool[,] RenderMatrix(IViewBoxModel view, bool[,] matrix);
    }

    public interface IBrailleIOContentRenderer
    {
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
        /// <param name="result">The result matrix, may be manipulated. Adressed in [y, x] notation.</param>
        /// <param name="additionalParams">Additional parameters.</param>
        void PostRenderHook(IViewBoxModel view, object content, ref bool[,] result, params object[] additionalParams);
    }


    #endregion
}
