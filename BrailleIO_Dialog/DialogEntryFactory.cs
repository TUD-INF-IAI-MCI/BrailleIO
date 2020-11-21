using System;
using System.Collections.Generic;

namespace BrailleIO.Dialogs
{
    /// <summary>
    /// A factory class allowing for an easy creation of specialized 
    /// <see cref="DialogEntry"/> implementations based on a requested <see cref="DialogEntryType"/>.
    /// So you do not have to implement special behaviors by your on but can use 
    /// predefined ones.
    /// </summary>
    public static class DialogEntryFactory
    {
        /// <summary>
        /// The callback forwarder list. 
        /// This list is used to keep the callback forwarder 
        /// alive (holding a reference so the GC will not delete them).
        /// </summary>
        static readonly List<EntryCallbackSender> _callbackForwarderList = new List<EntryCallbackSender>();

        /// <summary>
        /// Builds a specialized dialog entry, based on the given <see cref="DialogEntryType"/>.
        /// </summary>
        /// <param name="type">The type of the entry.</param>
        /// <param name="id">The unique identifier. 
        /// If an <see cref="Dialog"/> was set, the id is checked and adapted 
        /// to be unique. So check after returning the element if you are 
        /// not 100% sure that the id is unique.</param>
        /// <param name="title">The title of the entry (text that is displayed).</param>
        /// <param name="help">The help text for the entry. Giving details about the function or the behavior of the entry.</param>
        /// <param name="status">The initial status of the entry.</param>
        /// <param name="parent">The parent group if exist.</param>
        /// <param name="dialog">The dialog this entry should bee added.</param>
        /// <param name="callback">The callback function to call after activation.</param>
        /// <returns>An specialized <see cref="DialogEntryType"/>.</returns>
        public static DialogEntry BuildDialogEntry(
            DialogEntryType type,
            String id,
            String title,
            String help = "...",
            DialogEntryStatus status = DialogEntryStatus.Normal,
            DialogEntry parent = null,
            Dialog dialog = null,
            EntryActivation callback = null
            )
        {
            DialogEntry entry = null;

            if (dialog == null && parent != null && parent.ParentDialog != null) 
                dialog = parent.ParentDialog;

            id = check4uniqueID(id, parent, dialog);

            switch (type)
            {
                //case DialogEntryType.Unknown:
                //    break;
                //case DialogEntryType.Activation:
                //    break;
                //case DialogEntryType.Submenu:
                //    break;
                case DialogEntryType.Group:
                    entry = new Group_DialogEntry(id, title, help, parent, dialog);
                    break;
                //case DialogEntryType.Checkbox:
                //    break;
                case DialogEntryType.RadioButton:
                    entry = new RadioButton_DialogEntry(id, title, help, status, parent, dialog);
                    break;
                //case DialogEntryType.Button:
                //    break;
                //case DialogEntryType.EditField:
                //    break;
                default:
                    entry = new SelfRenderingDialogEntry(id, title, help, type, status, parent, dialog);
                    break;
            }

            if(entry == null) return null;

            RegisterActivationCallbackToEntry(entry, callback);

            // parent handling
            if (parent != null){ parent.AddChild(entry); }

            return entry;
        }

        /// <summary>
        /// Check recursively if the identifier is unique inside the dialog and the parent group if set.
        /// The id will be extended with '*'.
        /// </summary>
        /// <param name="id">The original identifier.</param>
        /// <param name="parent">The parent group.</param>
        /// <param name="dialog">The dialog.</param>
        /// <returns>An more unique identifier.</returns>
        private static string check4uniqueID(String id, DialogEntry parent, Dialog dialog)
        {
            if (dialog != null)
            {
                while (!dialog.IsUniqueID(id)) { id += "*"; }
            }
            if (parent != null && parent.HasChildren())
            {
                foreach (var item in parent.GetChildEntryList())
                {
                    if (item != null && item.ID.Equals(id))
                    {
                        id += "*";
                        id = check4uniqueID(id, parent, dialog);
                        return id;
                    }
                }
            }
            return id;
        }

        /// <summary>
        /// Callback function structure for <see cref="DialogEntry"/> activation 
        /// notification callbacks.
        /// </summary>
        /// <param name="entry">The entry that was activated.</param>
        public delegate void EntryActivation (DialogEntry entry);

        /// <summary>
        /// Registers the activation callback function to a specific entry.
        /// </summary>
        /// <param name="entry">The entry to observe for activation.</param>
        /// <param name="callback">The callback function to call after activation.</param>
        /// <returns><c>true</c> if the callback could be registered; otherwise, <c>false</c>.</returns>
        public static bool RegisterActivationCallbackToEntry(DialogEntry entry, EntryActivation callback)
        {
            bool success = false;
            if (entry != null && callback != null)
            {
                _callbackForwarderList.Add(new EntryCallbackSender(entry, callback));
                return true;
            }
            return success;
        }

        /// <summary>
        /// Small class wrapper for observing a specific <see cref="DialogEntry"/> 
        /// and call a related callback function if the entry was activated or checked.
        /// </summary>
        /// <seealso cref="System.IDisposable" />
        internal class EntryCallbackSender : IDisposable
        {
            DialogEntry entry;
            EntryActivation callback;
            DialogEntryStatus lastStatus = DialogEntryStatus.Unknown;

            /// <summary>
            /// Initializes a new instance of the <see cref="EntryCallbackSender"/> class.
            /// </summary>
            /// <param name="_entry">The entry to observe for activation.</param>
            /// <param name="_callback">The callback function to call after activation.</param>
            public EntryCallbackSender(DialogEntry _entry, EntryActivation _callback)
            {
                entry = _entry;
                callback = _callback;
                registerToEvents();
            }

            void registerToEvents(){
                if (entry != null)
                {
                    entry.DialogEntryChanged += entry_DialogEntryChanged;
                }
            }

            void unregisterToEvents()
            {
                if (entry != null)
                {
                    try { entry.DialogEntryChanged -= entry_DialogEntryChanged; }
                    catch { }
                }
            }

            void entry_DialogEntryChanged(object sender, EntryEventArgs e)
            {
                if(callback != null && e != null && e.Entry != null){
                    try
                    {
                        DialogEntryStatus status = e.Entry.Status;
                        if (status != lastStatus)
                        {
                            if (status.HasFlag(DialogEntryStatus.Disabled)) return;

                            // activation
                            if (status.HasFlag(DialogEntryStatus.Activated) && !lastStatus.HasFlag(DialogEntryStatus.Activated))
                            {
                                callback(e.Entry);
                            }
                            // checked
                            else if (status.HasFlag(DialogEntryStatus.Checked) && !lastStatus.HasFlag(DialogEntryStatus.Checked))
                            {
                                callback(e.Entry);
                            }
                        }
                    }
                    catch (Exception) { }
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, 
            /// releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                // clean up references
                entry = null;
                callback = null;
                unregisterToEvents();
                try
                {
                    _callbackForwarderList.Remove(this);
                }
                catch { }
            }
        }
    }
}
