using System.Collections.Generic;
using System.Collections.Concurrent;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Abstract basis class for a renderer that enables hooking
    /// </summary>
    public abstract class BrailleIOHookableRendererBase : IBrailleIOHookableRenderer
    {
        #region Member

        protected readonly ConcurrentDictionary<int, IBailleIORendererHook> hooks = new ConcurrentDictionary<int, IBailleIORendererHook>();

        #endregion

        /// <summary>
        /// Register a hook.
        /// </summary>
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
        /// <param name="hook">The hook.</param>
        public virtual void UnregisterHook(IBailleIORendererHook hook)
        {
            if (hooks.ContainsKey(hook.GetHashCode()))
            {
                hooks.TryRemove(hook.GetHashCode(), out hook);
            }
        }

        /// <summary>
        /// Calls all registered pre-renderer hooks.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="content">The content.</param>
        /// <param name="additionalParams">The additional params.</param>
        protected virtual void callAllPreHooks(ref IViewBoxModel view, ref object content, params object[] additionalParams)
        {
            if (hooks.Count > 0)
            {
                try
                {
                    foreach (IBailleIORendererHook hook in hooks.Values)
                    {
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
        /// <param name="view">The view.</param>
        /// <param name="content">The content.</param>
        /// <param name="result">The result.</param>
        /// <param name="additionalParams">The additional params.</param>
        protected virtual void callAllPostHooks(IViewBoxModel view, object content, ref bool[,] result, params object[] additionalParams)
        {
            if (hooks.Count > 0)
            {
                try
                {
                    foreach (IBailleIORendererHook hook in hooks.Values)
                    {
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
