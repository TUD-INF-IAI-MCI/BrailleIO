//Autor:    Stephanie Schöne
//          Jens Bornschein
// TU Dresden, Germany

using System;
using System.Collections.Generic;

namespace BrailleIO.Dialogs
{
    /// <summary>
    /// Class representing an entry inside a dialog.
    /// </summary>
    public class DialogEntry : IDialogComponent
    {
        #region Members

        #region Public Members

        /// <summary>
        /// An object for synchronizing the access to critical parts.
        /// </summary>
        public readonly Object SyncLock = new Object();

        private string _id;
        /// <summary>
        /// Gets or sets the unique identifier for this entry.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        virtual public string ID
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_id)) _id = "DE_" + this.GetHashCode();
                return _id;
            }
            private set
            {
                if (value.Equals(_id)) return;
                _id = value;
                fire_DialogEntryChanged("ID");
            }
        }

        private string _title;
        /// <summary>
        /// Gets or sets the visible text of this entry.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        virtual public string Title
        {
            get
            {
                if (_title == null) _title = String.Empty;
                return _title;
            }
            set
            {
                if (value.Equals(_title)) return;
                _title = value;
                fire_DialogEntryChanged("Title");
            }
        }

        private string _help = String.Empty;
        /// <summary>
        /// Gets or sets the help text for this entry.
        /// </summary>
        /// <value>
        /// The help text.
        /// </value>
        virtual public string Help
        {
            get
            {
                if (_help == null) _help = String.Empty;
                return _help;
            }
            set
            {
                if (value.Equals(_help)) return;
                _help = value;
                fire_DialogEntryChanged("Help");
            }
        }

        private DialogEntryType _type = DialogEntryType.Unknown;
        /// <summary>
        /// Gets or sets the (control)type of this entry.
        /// </summary>
        /// <value>
        /// The (control)type.
        /// </value>
        virtual public DialogEntryType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                Status = DialogEntryStatus.Normal;
                fire_DialogEntryChanged("Type");
            }
        }

        private DialogEntryStatus _status = DialogEntryStatus.Normal;
        /// <summary>
        /// Gets or sets the status of this entry.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        virtual public DialogEntryStatus Status
        {
            get { return _status; }
            set
            {
                var _oldStatus = _status;

                if (value.HasFlag(DialogEntryStatus.Disabled)) _status = DialogEntryStatus.Disabled;
                else
                {
                    if (value == DialogEntryStatus.Unknown) _status = value;
                    else if (value.HasFlag(DialogEntryStatus.Normal)) _status = DialogEntryStatus.Normal;

                    switch (Type)
                    {
                        case DialogEntryType.Activation:
                            if (value.HasFlag(DialogEntryStatus.Activated))
                            {
                                _status = DialogEntryStatus.Activated;
                                fire_DialogEntryActivated();
                            }
                            break;

                        case DialogEntryType.Submenu:
                            break;

                        case DialogEntryType.Group:
                            if (value.HasFlag(DialogEntryStatus.Activated)) _status = DialogEntryStatus.Activated;
                            else _status = DialogEntryStatus.Normal;
                            break;

                        case DialogEntryType.Checkbox:
                            if (value.HasFlag(DialogEntryStatus.Unchecked)) _status |= DialogEntryStatus.Unchecked;
                            else if (value.HasFlag(DialogEntryStatus.Checked))
                            {
                                _status = DialogEntryStatus.Checked;
                                fire_DialogEntryActivated();
                            }
                            else _status = DialogEntryStatus.Normal | DialogEntryStatus.Unchecked;
                            break;

                        case DialogEntryType.RadioButton:
                            if (value.HasFlag(DialogEntryStatus.Unchecked)) _status |= DialogEntryStatus.Unchecked;
                            else if (value.HasFlag(DialogEntryStatus.Checked))
                            {
                                _status = DialogEntryStatus.Checked;
                                fire_DialogEntryActivated();
                            }
                            else _status = DialogEntryStatus.Normal | DialogEntryStatus.Unchecked;
                            break;
                        case DialogEntryType.EditField:
                            if (value.HasFlag(DialogEntryStatus.Editing)) _status = DialogEntryStatus.Editing;
                            else _status = DialogEntryStatus.Normal;
                            break;
                        case DialogEntryType.Label:
                            if (value.HasFlag(DialogEntryStatus.Normal)) _status = DialogEntryStatus.Normal;
                            break;

                        default:
                            _status = value;
                            break;
                    }

                    if (value.HasFlag(DialogEntryStatus.Selected)) _status |= DialogEntryStatus.Selected;

                }

                if (_oldStatus != _status)
                    fire_DialogEntryChanged("Status");

            }
        }

        Dialog _parentDialog = null;
        /// <summary>
        /// Gets or sets the parent dialog.
        /// </summary>
        /// <value>
        /// The parent dialog.
        /// </value>
        virtual public Dialog ParentDialog
        {
            get { return _parentDialog; }
            set
            {
                if (value == _parentDialog) return;
                _parentDialog = value;
                //if (_parentDialog != null && _parentEntry != null) // cause infinite loop
                //{
                //    _parentDialog.AddEntry(this);
                //}

                if (ChildEntries != null && ChildEntries.Count > 0)
                {
                    lock (SyncLock)
                    {
                        foreach (var item in ChildEntries)
                        {
                            if (item != null)
                            {
                                if (item is Dialog)
                                    ((Dialog)item).Parent = _parentDialog;
                                else if (item is DialogEntry)
                                    ((DialogEntry)item).ParentDialog = _parentDialog;
                            }
                        }
                    }
                }
                fire_DialogEntryChanged("ParentDialog");
            }
        }

        DialogEntry _parentEntry = null;
        /// <summary>
        /// Gets or sets the parent entry.
        /// The parent Entry must be some kind of group type (<see cref="DialogEntryType.Group"/>).
        /// </summary>
        /// <value>
        /// The parent entry.
        /// </value>
        virtual public DialogEntry ParentEntry
        {
            get
            {
                if (_parentEntry == null ||
                    //_parentEntry.Type == DialogEntryType.Submenu ||
                    _parentEntry.Type == DialogEntryType.Group)
                {
                    return _parentEntry;
                }
                else
                {
                    _parentEntry = null;
                    fire_DialogEntryChanged("ParentEntry");
                    return _parentEntry;
                }
            }
            set
            {
                if (value != null && value.Equals(_parentEntry)) return;
                _parentEntry = value;

                if (_parentEntry != null) { _parentEntry.AddChild(this); }
                if (_parentEntry == value) // check if it was set and was accepted.
                    fire_DialogEntryChanged("ParentEntry");
            }
        }

        #endregion

        #region Private Members

        private readonly List<IDialogComponent> childEntries = new List<IDialogComponent>();
        /// <summary>
        /// Gets the optional child entries.
        /// This can be a Sub-dialog or other dialog entries, which should be grouped together.
        /// </summary>
        /// <value>
        /// The child entries.
        /// </value>
        protected List<IDialogComponent> ChildEntries
        {
            get
            {
                lock (SyncLock)
                {
                    return childEntries;
                }
            }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogEntry" /> class.
        /// </summary>
        /// <param name="ID">The unique identifier.</param>
        /// <param name="text">The text that is displayed.</param>
        /// <param name="help">The help text explaining the functionality behind this entry.</param>
        /// <param name="type">The type defining its behavior.</param>
        /// <param name="status">The initial status.</param>
        /// <param name="parentEntry">The parent entry (must be some kind of group type).</param>
        /// <param name="parentDialog">The parent dialog this entry is related to. Can be <c>null</c> - when added to a Dialog this Field will be set automatically.</param>
        public DialogEntry(
            string ID,
            string text,
            string help = "...",
            DialogEntryType type = DialogEntryType.Activation,
            DialogEntryStatus status = DialogEntryStatus.Normal,
            DialogEntry parentEntry = null,
            Dialog parentDialog = null)
        {
            this._id = ID;
            this._title = text;
            this._help = help;
            this.ParentEntry = parentEntry;
            this.ParentDialog = parentDialog;
            this.Type = type;
            this.Status = status;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogEntry"/> class.
        /// </summary>
        /// <param name="dialogEntry">The dialog entry.</param>
        public DialogEntry(DialogEntry dialogEntry)
        {
            this._id = dialogEntry.ID;
            this._title = dialogEntry.Title;
            this._help = dialogEntry.Help;
            this._type = dialogEntry.Type;
            this.ParentEntry = dialogEntry.ParentEntry;
            this.ParentDialog = dialogEntry.ParentDialog;

            this.Status = dialogEntry.Status;

            if (Type == DialogEntryType.Group)
            {
                childEntries = dialogEntry.GetChildEntryList();
            }
        }

        #endregion

        #region Sub-menu Handling

        /// <summary>
        /// Determines whether this instance has a further sub-menu.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has a sub-menu; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasSubmenu()
        {
            if (Type == DialogEntryType.Submenu)
            {
                if (ChildEntries.Count > 0)
                {
                    if (ChildEntries.Count == 1)
                    {
                        if (ChildEntries[0] is Dialog)
                            return true;
                        else ChildEntries.Clear();
                    }
                    else
                    { // clean up if it was a group before
                        IDialogComponent dlg = null;
                        dlg = ChildEntries.FindLast((item) => { return item is Dialog; });

                        ChildEntries.Clear(); // remove the not 

                        if (dlg != null) // if a dialog was found in all children, add them again.
                        {
                            ChildEntries.Add(dlg);
                            return ChildEntries.Count == 1;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the sub-menu.
        /// </summary>
        /// <returns></returns>
        public virtual Dialog GetSubmenu()
        {
            return HasSubmenu() ? ChildEntries[0] as Dialog : null;
        }

        /// <summary>
        /// Sets the sub-menu.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <returns></returns>
        public virtual bool SetSubmenu(Dialog dialog, Dialog parentDialog = null)
        {
            bool success = true;
            if (dialog != null)
            {
                if (Type != DialogEntryType.Submenu)
                {
                    Type = DialogEntryType.Submenu;
                }

                if (Type == DialogEntryType.Submenu)
                {
                    // set parent dialog
                    if (parentDialog != null)
                        dialog.Parent = parentDialog;
                    else if (this.ParentDialog != null)
                        dialog.Parent = parentDialog;

                    if (!dialog.Equals(GetSubmenu()))
                    {
                        ChildEntries.Clear();
                        ChildEntries.Add(dialog);
                        fire_DialogEntryChanged("Submenu");
                    }
                }
                else
                {
                    System.Console.WriteLine("DialogEntry " + this.ID + " has not DialogEntryType " + this.Type.ToString());
                    success = false;
                }
            }
            else if (Type == DialogEntryType.Submenu) // submenu reset
            {
                ChildEntries.Clear();
                Type = DialogEntryType.Unknown;
            }
            else
            {
                success = false;
            }
            return success;
        }

        #endregion

        #region Group Handling

        /// <summary>
        /// Gets the group list.
        /// </summary>
        /// <returns>The list of child-entries for this group.</returns>
        public virtual  List<IDialogComponent> GetChildEntryList()
        {
            List<IDialogComponent> children = new List<IDialogComponent>(ChildEntries);
            return children;
        }

        /// <summary>
        /// Determines whether this element is part of a group.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this element is within an group; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsInGroup()
        {
            return ParentEntry != null && ParentEntry.Type == DialogEntryType.Group;
        }

        /// <summary>
        /// Determines whether this instance has children.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has children; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasChildren()
        {
            return (Type == DialogEntryType.Group || Type == DialogEntryType.Unknown) && 
                childEntries != null && childEntries.Count > 0;
        }

        /// <summary>
        /// Adds a child to the list of children.
        /// Does only work for groups, submenus and user defined types.
        /// </summary>
        /// <param name="child">The child to add. The child have to have a specific class type
        /// regarding to the parents type.</param>
        /// <returns><c>true</c> if the child could be added. Otherwise, <c>false</c>.</returns>
        public virtual bool AddChild(IDialogComponent child)
        {
            if (child != null)
            {
                switch (Type)
                {
                    case DialogEntryType.Unknown:
                        if (!childEntries.Contains(child))
                        {
                            childEntries.Add(child);
                            return childEntries.Contains(child);
                        }
                        break;
                    //case DialogEntryType.Activation:
                    //    break;
                    case DialogEntryType.Submenu:
                        if (child is Dialog &&
                            !childEntries.Contains(child))
                        {
                            childEntries.Add(child);
                            return childEntries.Contains(child);
                        }
                        break;
                    case DialogEntryType.Group:
                        if (child is DialogEntry && 
                            !childEntries.Contains(child))
                        {
                            childEntries.Add(child);
                            return childEntries.Contains(child);
                        }
                        break;
                    //case DialogEntryType.Checkbox:
                    //    break;
                    //case DialogEntryType.RadioButton:
                    //    break;
                    //case DialogEntryType.Button:
                    //    break;
                    //case DialogEntryType.EditField:
                    //    break;
                    default:
                        break;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Searches for a child in the entries and child entries of 
        /// an entry for an entry with the given status.
        /// </summary>
        /// <param name="statusFlag">The status flag to search for.</param>
        /// <returns>The first entry having set this flag or <c>null</c>.</returns>
        public DialogEntry GetFirstChildElementByStatus(DialogEntryStatus statusFlag)
        {
            if (HasChildren())
            {
                {
                    lock (SyncLock)
                    {
                        try
                        {
                            foreach (var item in ChildEntries)
                            {
                                if (item != null && item is DialogEntry)
                                {
                                    if (((DialogEntry)item).Status.HasFlag(statusFlag))
                                        return item as DialogEntry;

                                    // child handling
                                    // if this element is another group etc ...
                                    if (((DialogEntry)item).HasChildren())
                                    {
                                        var child = ((DialogEntry)item).GetFirstChildElementByStatus(statusFlag);
                                        if (child != null) return child;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Searches for all child in the entries and child entries of 
        /// an entry for entries with the given status.
        /// </summary>
        /// <param name="statusFlag">The status flag to search for.</param>
        /// <returns>All entries having set this flag or <c>null</c>.</returns>
        public List<DialogEntry> GetAllChildElementsByStatus(DialogEntryStatus statusFlag)
        {
            List<DialogEntry> children = new List<DialogEntry>();
            if (HasChildren())
            {
                {
                    lock (SyncLock)
                    {
                        try
                        {
                            foreach (var item in ChildEntries)
                            {
                                if (item != null && item is DialogEntry)
                                {
                                    if (((DialogEntry)item).Status.HasFlag(statusFlag))
                                        children.Add(item as DialogEntry);

                                    // child handling
                                    // if this element is another group etc ...
                                    if (((DialogEntry)item).HasChildren())
                                    {
                                        children.AddRange(((DialogEntry)item).GetAllChildElementsByStatus(statusFlag));
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            return children;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "\t" + this.Title + "[" + this.ID + "|" + Status.ToString() + "]";
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when some properties or the state of this dialog entry was changed.
        /// </summary>
        public event EventHandler<EntryEventArgs> DialogEntryChanged;

        /// <summary>
        /// Fires the <see cref="DialogEntryChanged"/> event to its listeners.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        protected void fire_DialogEntryChanged(string propName = "")
        {
            if (String.IsNullOrEmpty(propName))
                fire_DialogEntryChanged(new EntryEventArgs(this));
            else
                fire_DialogEntryChanged(new DetailedEntryChangedEventArgs(this, propName));
        }

        /// <summary>
        /// Fires the <see cref="DialogEntryChanged"/> event to its listeners.
        /// </summary>
        /// <param name="e">The <see cref="EntryEventArgs"/> instance containing the event data.</param>
        protected void fire_DialogEntryChanged(EntryEventArgs e)
        {
            if (DialogEntryChanged != null)
            {
                try
                {
                    DialogEntryChanged.Invoke(this, e);
                }
                catch { }
            }
        }

        /// <summary>
        /// Occurs when a dialog entry was activated.
        /// </summary>
        public event EventHandler<EntryEventArgs> EntryActivated;

        /// <summary>
        /// Fires the <see cref="EntryActivated"/> event to its listeners.
        /// </summary>
        protected void fire_DialogEntryActivated()
        {
            if (EntryActivated != null)
            {
                try
                {
                    EntryActivated.Invoke(this, new EntryEventArgs(this));
                }
                catch { }
            }
        }


        #endregion

        #region Child Event Handling

        /// <summary>
        /// Registers to events of an child entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        protected virtual void registerToEvents(DialogEntry entry)
        {
            if (entry != null)
            {
                unregisterFromEvents(entry);
                entry.DialogEntryChanged += entry_DialogEntryChanged;
            }
        }

        /// <summary>
        /// Unregisters from events of an child entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        protected virtual void unregisterFromEvents(DialogEntry entry)
        {
            if (entry != null)
            {
                try
                {
                    entry.DialogEntryChanged -= entry_DialogEntryChanged;
                }
                catch { }
            }
        }

        protected virtual void entry_DialogEntryChanged(object sender, EntryEventArgs e)
        {
            if (e != null && e.Entry != null)
            {
                if (e is DetailedEntryChangedEventArgs)
                {
                    switch (((DetailedEntryChangedEventArgs)e).ChangedProperty)
                    {
                        case "ChildEntries":
                            handleChildChildEntrieEvent(e.Entry);
                            break;
                        default:
                            break;
                    }
                }

                fire_DialogEntryChanged(e);
            }
        }

        protected virtual void handleChildChildEntrieEvent(DialogEntry dialogEntry)
        {
            if (dialogEntry != null)
            {
                var children = dialogEntry.GetChildEntryList();
                if (children != null && children.Count > 0)
                {
                    foreach (var item in children)
                    {
                        registerToEvents(item as DialogEntry);
                    }
                }
            }
        }

        #endregion

        #region Renderer




        #endregion
    }

    #region Enum
    /// <summary>
    /// Status flags indicating the current state of a <see cref="DialogEntry"/>.
    /// </summary>
    [Flags]
    public enum DialogEntryStatus : int
    {
        /// <summary>
        /// Unknown / not initialized
        /// </summary>
        Unknown = 1,
        /// <summary>
        /// Normal Status
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Is currently activated 
        /// </summary>
        Activated = 2,
        /// <summary>
        /// Is Disabled - can't be activated
        /// </summary>
        Disabled = 4,
        /// <summary>
        /// currently selected (has focus)
        /// </summary>
        Selected = 8,
        /// <summary>
        /// Is Checked
        /// </summary>
        Checked = 16,
        /// <summary>
        /// Is unchecked
        /// </summary>
        Unchecked = 32,
        /// <summary>
        /// Currently editing
        /// </summary>
        Editing = 64,
        /// <summary>
        /// Currently aborting
        /// </summary>
        Aborting = 128
    }


    /// <summary>
    /// Typed defining the behavior of a <see cref="DialogEntry"/>
    /// </summary>
    public enum DialogEntryType : int
    {
        /// <summary>
        /// unknown or user defined type
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// common menu entry that calls a function if activated.
        /// </summary>
        Activation = 1,
        /// <summary>
        /// Menu entry that opens another dialog. 
        /// </summary>
        Submenu = 2,
        /// <summary>
        /// grouping entry for semantic or functional relation building.
        /// </summary>
        Group = 3,
        /// <summary>
        /// CheckBox control allows the user to toggle an option on or off. 
        /// Multiple selections are allowed.
        /// </summary>
        Checkbox = 4,
        /// <summary>
        /// The RadioButton control present a set of two or more mutually 
        /// exclusive choices to the user.
        /// </summary>
        RadioButton = 5,
        /// <summary>
        /// Like an <see cref="Activation"/> 
        /// </summary>
        Button = 6,
        /// <summary>
        /// A text edit field for entering text.
        /// </summary>
        EditField = 7,
        /// <summary>
        /// A Label for non-interactable information fields.
        /// </summary>
        Label = 8,
        /// <summary>
        /// The edit field label
        /// </summary>
        EditField_Label = 9,
    }

    #endregion

    #region Event Args

    /// <summary>
    /// Event arguments for events related to a <see cref="DialogEnty"/>.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class DetailedEntryChangedEventArgs : EntryEventArgs
    {

        /// <summary>
        /// The changed property
        /// </summary>
        public readonly String ChangedProperty = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogEntyEventArgs" /> class.
        /// </summary>
        /// <param name="entry">The related entry.</param>
        /// <param name="propertyName">Name of the property that was changed.</param>
        public DetailedEntryChangedEventArgs(DialogEntry entry, string propertyName = "")
            : base(entry)
        {
            ChangedProperty = propertyName;
        }
    }

    #endregion

}
