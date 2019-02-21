﻿using System.Collections.Concurrent;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Abstract basis class for a renderer that enables hooking
    /// </summary>
		/// <remarks> </remarks>
    public abstract class BrailleIOHookableRendererBase : IBrailleIOHookableRenderer
    {
        #region Member

        /// <summary>
        /// The hooks that should be called when rendering the content.
        /// </summary>
		/// <remarks> </remarks>
        protected readonly ConcurrentDictionary<int, IBailleIORendererHook> hooks = new ConcurrentDictionary<int, IBailleIORendererHook>();

        #endregion

        #region IBrailleIOHookableRenderer

        /// <summary>
        /// Register a hook.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="hook">The hook.</param>
        public virtual void RegisterHook(IBailleIORendererHook hook)
        {
            if (!hooks.ContainsKey(hook.GetHashCode()))
            {
                hooks[hook.GetHashCode()] = hook;
            }
        }

        /// <summary>
        /// Unregisters a hook.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="hook">The hook.</param>
        public virtual void UnregisterHook(IBailleIORendererHook hook)
        {
            if (hooks.ContainsKey(hook.GetHashCode()))
            {
                hooks.TryRemove(hook.GetHashCode(), out hook);
            }
        }

        #endregion

        /// <summary>
        /// Calls all registered pre-renderer hooks.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="view">The view.</param>
        /// <param name="content">The content.</param>
        /// <param name="additionalParams">The additional parameters.</param>
        protected virtual void callAllPreHooks(ref IViewBoxModel view, ref object content, params object[] additionalParams)
        {
            if (hooks.Count > 0)
            {
                try
                {
                    foreach (IBailleIORendererHook hook in hooks.Values)
                    {
                        if (!(hook is IActivatable) || ((IActivatable)hook).Active) 
                            hook.PreRenderHook(ref view, ref content, additionalParams);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception in pre renderer hook: " + ex);
                }
            }
        }

        /// <summary>
        /// Calls all registered post-renderer hooks.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="view">The view.</param>
        /// <param name="content">The content.</param>
        /// <param name="result">The result.</param>
        /// <param name="additionalParams">The additional parameters.</param>
        protected virtual void callAllPostHooks(IViewBoxModel view, object content, ref bool[,] result, params object[] additionalParams)
        {
            if (hooks.Count > 0)
            {
                try
                {
                    foreach (IBailleIORendererHook hook in hooks.Values)
                    {
                        if (!(hook is IActivatable) || ((IActivatable)hook).Active) 
                         hook.PostRenderHook(view, content, ref result, additionalParams);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception in pre renderer hook: " + ex);
                }
            }
        }
    }
}
