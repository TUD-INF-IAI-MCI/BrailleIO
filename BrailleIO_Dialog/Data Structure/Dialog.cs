//Autor:    Stephanie Schöne
//          Jens Bornschein
// TU Dresden, Germany

using BrailleIO.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using tud.mci.LanguageLocalization;


namespace BrailleIO.Dialogs
{
    /// <summary>
    /// Data structure for some kind of menu/dialog tree-based interaction representation.
    /// </summary>
    public class Dialog : IDialogComponent
    {
        #region Members

        #region Public Members

        private bool _active;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get { return _active; }
            set
            {
                if (value == _active) return;
                _active = value;
                if (_active) fire_Activated();
                else fire_Deactivated();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [help is shown].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [help is shown]; otherwise, <c>false</c>.
        /// </value>
        public bool HelpIsShown { get; set; }

        private string _helpText;
        /// <summary>
        /// The information text for the whole structure. Some kind of help text.
        /// </summary>
        public string Help
        {
            get
            {
                if (_helpText == null) _helpText = String.Empty;
                return _helpText;
            }
            set
            {
                if (value.Equals(_helpText)) return;
                _helpText = value;
                fire_DialogChanged();
            }
        }

        private string _title;
        /// <summary>
        /// Gets or sets the title of this dialog.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
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
                fire_DialogChanged();
            }
        }


        private string _id = "";
        /// <summary>
        /// Gets or sets the unique ID of this dialog.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public string ID
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_id)) { _id = GetHashCode().ToString(); }
                return _id;
            }
            set
            {
                if (value.Equals(_id)) return;
                _id = value;
                fire_DialogChanged();
            }
        }

        private DialogType _type;
        /// <summary>
        /// The type of the dialog
        /// </summary>
        public DialogType Type
        {
            get { return _type; }
            set
            {
                if (value.Equals(_type)) return;
                _type = value;
                fire_DialogChanged();
            }
        }

        private Dialog _parent;
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public Dialog Parent
        {
            get { return _parent; }
            set
            {
                if (value.Equals(_parent)) return;
                if (!checkForDialogCyclus(value))
                {
                    _parent = value;
                    fire_DialogChanged();
                }
                else
                {
                    String exMsg = "CYCLE FOUND! Dialog " + this.Title + " creates cycle with " + value.Title + " as Parent!";
                    System.Console.WriteLine(exMsg);
                    throw new ArgumentException(exMsg, "Parent");
                }
            }
        }

        /// <summary>
        /// Gets the amount of entries in this dialog.
        /// </summary>
        /// <value>
        /// The amount of entries.
        /// </value>
        public int EntryCount
        {
            get { return entryList.Count; }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// The entries of this dialog
        /// </summary>
        private readonly List<DialogEntry> entryList = new List<DialogEntry>();
        private readonly Dictionary<String, DialogEntry> entryDict = new Dictionary<string, DialogEntry>();


        private DialogEntry _activEntry;
        /// <summary>
        /// The activated entry
        /// </summary>
        public DialogEntry ActivatedEntry
        {
            get { return _activEntry; }
            private set
            {
                if (_activEntry == value) return;
                var oldValue = _activEntry;
                _activEntry = value;
                if (oldValue != null) fire_EntryDeactivated(oldValue);
                if (_activEntry != null) fire_EntryActivated(_activEntry);
            }
        }

        private DialogEntry _selectedEntry;
        /// <summary>
        /// Gets or sets the current selected entry.
        /// </summary>
        /// <value>
        /// The selected.
        /// </value>
        public DialogEntry SelectedEntry
        {
            get { return _selectedEntry; }
            private set
            {
                if (value == _selectedEntry) return;
                var oldVal = _selectedEntry;
                _selectedEntry = value;
                if (oldVal != null) fire_EntryDeselected(oldVal);
                if (_selectedEntry != null) fire_EntrySelected(_selectedEntry);
            }
        }

        /// <summary>
        /// ????????
        /// </summary>
        /// <value>
        /// The checked entries.
        /// </value>
        private List<DialogEntry> _checkedEntries;
        protected List<DialogEntry> CheckedEntries
        {
            get
            {
                if (_checkedEntries == null)
                    _checkedEntries = new List<DialogEntry>();
                return _checkedEntries;
            }
            set { _checkedEntries = value; }
        }

        private static LL ll = new LL(BrailleIO.Dialogs.Properties.Resources.Language);
        private readonly Semaphore _semaphore = new Semaphore(1, 2);

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Dialog"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="type">The type.</param>
        /// <param name="isActivated">if set to <c>true</c> [is activated].</param>
        public Dialog(
            string title,
            string id,
            DialogType type = DialogType.Default,
            bool isActivated = false,
            Dialog parentDialog = null)
        {
            _semaphore.WaitOne();
            try
            {
                entryList = new List<DialogEntry>();
                this._title = title;
                this._id = id;
                this._type = type;
                this._selectedEntry = null;
                this._activEntry = null;
                //this.Scrollheight = 0;
                this._active = isActivated; //ONLY ONE ISACTIVATED POSSIBLE IN LISTS
                this._parent = parentDialog;
                //  this.checkedEntries = new List<DialogEntry>();
            }
            finally { _semaphore.Release(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dialog"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="type">The type.</param>
        /// <param name="isActivated">if set to <c>true</c> [is activated].</param>
        public Dialog(Dialog dialog)
        {
            _semaphore.WaitOne();

            try
            {
                entryList = new List<DialogEntry>();
                this._title = dialog.Title;
                this._id = dialog.ID;
                this._type = dialog.Type;
                this._selectedEntry = dialog.GetSelectedEntry();
                this._activEntry = dialog.ActivatedEntry;
                //this.Scrollheight = dialog.Scrollheight;
                this._active = dialog.IsActive; //ONLY ONE ISACTIVATED POSSIBLE IN LISTS
                this._parent = dialog.Parent;
                // this.checkedEntries = new List<DialogEntry>();

            }
            finally { _semaphore.Release(); }
        }

        #endregion

        #region Events

        /// <summary>
        /// Child dialog was activated.
        /// </summary>
        public event EventHandler<DialogEventArgs> SwitchedToChildDialog;
        /// <summary>
        /// left this child dialog and switched back to the parent dialog.
        /// </summary>
        public event EventHandler<DialogEventArgs> SwitchedToParentDialog;

        /// <summary>
        /// Occurs when an entry gets selected.
        /// </summary>
        public event EventHandler<EntryEventArgs> EntrySelected;
        /// <summary>
        /// Occurs when an entry gets deselected.
        /// </summary>
        public event EventHandler<EntryEventArgs> EntryDeselected;
        /// <summary>
        /// Occurs when an entry was activated.
        /// </summary>
        public event EventHandler<EntryEventArgs> EntryActivated;
        /// <summary>
        /// Occurs when an entry was deactivated.
        /// </summary>
        public event EventHandler<EntryEventArgs> EntryDeactivated;
        /// <summary>
        /// Occurs when an checkable entry gets checked - such as a check-box or a radio button.
        /// </summary>
        public event EventHandler<EntryEventArgs> EntryChecked;
        /// <summary>
        /// Occurs when an checkable entry is not longer checked - such as a check-box or a radio button.
        /// </summary>
        public event EventHandler<EntryEventArgs> EntryUnchecked;

        /// <summary>
        /// Occurs when a new entry was added.
        /// </summary>
        public event EventHandler<EntryEventArgs> EntryAdded;
        /// <summary>
        /// Occurs when an entry was removed.
        /// </summary>
        public event EventHandler<EntryEventArgs> EntryRemoved;

        /// <summary>
        /// Occurs when this dialog gets activated.
        /// </summary>
        public event EventHandler<DialogEventArgs> Activated;

        /// <summary>
        /// Occurs when the dialog was deactivated.
        /// </summary>
        public event EventHandler<DialogEventArgs> Deactivated;

        /// <summary>
        /// Occurs when some properties of the dialog were changed.
        /// </summary>
        public event EventHandler<DialogEventArgs> DialogChanged;

        /// <summary>
        /// left active dialog and switched directly to incoming dialog.
        /// </summary>
        public event EventHandler<DialogEventArgs> SwitchedToDialog;

        #region event throwing

        private void fire_SwitchedToParentDialog()
        {
            if (SwitchedToParentDialog != null)
            {
                try
                {
                    SwitchedToParentDialog.Invoke(this, new DialogEventArgs(this));
                }
                catch { }
            }
        }

        private void fire_SwitchedToChildDialog()
        {
            if (SwitchedToChildDialog != null)
            {
                try
                {
                    SwitchedToChildDialog.Invoke(this, new DialogEventArgs(this));
                }
                catch { }
            }
        }

        private void fire_EntrySelected(DialogEntry entry)
        {
            if (EntrySelected != null)
            {
                try
                {
                    EntrySelected.Invoke(this, new EntryEventArgs(entry));
                }
                catch { }
            }
        }

        private void fire_EntryDeselected(DialogEntry entry)
        {
            if (EntryDeselected != null)
            {
                try
                {
                    EntryDeselected.Invoke(this, new EntryEventArgs(entry));
                }
                catch { }
            }
        }

        private void fire_EntryActivated(DialogEntry entry)
        {
            if (EntryActivated != null)
            {
                try
                {
                    EntryActivated.Invoke(this, new EntryEventArgs(entry));
                }
                catch { }
            }
        }

        private void fire_EntryDeactivated(DialogEntry entry)
        {
            if (EntryDeactivated != null)
            {
                try
                {
                    EntryDeactivated.Invoke(this, new EntryEventArgs(entry));
                }
                catch { }
            }
        }

        private void fire_EntryUnchecked(DialogEntry entry)
        {
            if (EntryUnchecked != null)
            {
                try
                {
                    EntryUnchecked.Invoke(this, new EntryEventArgs(entry));
                }
                catch { }
            }
        }

        private void fire_EntryChecked(DialogEntry entry)
        {
            if (EntryChecked != null)
            {
                try
                {
                    EntryChecked.Invoke(this, new EntryEventArgs(entry));
                }
                catch { }
            }
        }

        /// <summary>
        /// Fires the entry added.
        /// IMPORTANT: registers also to the entries events for keeping up the states. 
        /// </summary>
        /// <param name="entry">The entry.</param>
        private void fire_EntryAdded(DialogEntry entry)
        {
            if (entry != null)
            {
                unregisterFromEvent(entry);
                registerToEvents(entry);
            }

            if (EntryUnchecked != null)
            {
                try
                {
                    EntryAdded.Invoke(this, new EntryEventArgs(entry));
                }
                catch { }
            }
        }

        /// <summary>
        /// Fires if an entry was removed.
        /// IMPORTANT: also unregisters form events.
        /// </summary>
        /// <param name="entry">The entry.</param>
        private void fire_EntryRemoved(DialogEntry entry)
        {
            if (entry != null)
            {
                unregisterFromEvent(entry);
            }

            if (EntryUnchecked != null)
            {
                try
                {
                    EntryRemoved.Invoke(this, new EntryEventArgs(entry));
                }
                catch { }
            }
        }

        private void fire_DialogChanged()
        {
            if (DialogChanged != null)
            {
                try
                {
                    DialogChanged.Invoke(this, new DialogEventArgs(this));
                }
                catch { }
            }
        }

        private void fire_Activated()
        {
            if (Activated != null)
            {
                try
                {
                    Activated.Invoke(this, new DialogEventArgs(this));
                }
                catch { }
            }
        }

        private void fire_Deactivated()
        {
            if (Deactivated != null)
            {
                try
                {
                    Deactivated.Invoke(this, new DialogEventArgs(this));
                }
                catch { }
            }
        }

        private void fire_SwitchedToDialog(Dialog dlg)
        {
            if (SwitchedToDialog != null)
            {
                try
                {
                    SwitchedToDialog.Invoke(this, new DialogEventArgs(dlg));
                }
                catch { }
            }
        }

        #endregion

        #endregion

        #region Public Getter

        /// <summary>
        /// Gets all available entries.
        /// </summary>
        /// <returns>List of entries</returns>
        public List<DialogEntry> GetEntryList()
        {
            return new List<DialogEntry>(entryList);
        }

        /// <summary>
        /// Gets the activated entry from all dialogs.
        /// </summary>
        /// <returns></returns>
        public DialogEntry GetActivatedEntryFromAllDialogs()
        {
            if (ActivatedEntry != null) return ActivatedEntry;
            else return getActivatedEntryFromAllDialogsRecursion(getRootDialog());
        }

        /// <summary>
        /// Gets the selected entry.
        /// </summary>
        /// <returns></returns>
        public DialogEntry GetSelectedEntry()
        {
            if (SelectedEntry != null) return (SelectedEntry);
            else return null;
        }

        /// <summary>
        /// Gets the checkbox activated list.
        /// ????
        /// </summary>
        /// <returns></returns>
        public List<DialogEntry> GetCheckboxActivatedList()
        {
            return new List<DialogEntry>(CheckedEntries);
        }

        Dialog _lastActiveDialog = null;
        /// <summary>
        /// Gets the active dialog from the dialog tree.
        /// </summary>
        /// <returns>The active dialog from this tree or <c>null</c> if no is activated.</returns>
        public Dialog GetActiveDialog()
        {
            if (IsActive) return this;
            else
            {
                if (_lastActiveDialog == null || !_lastActiveDialog.IsActive)
                    _lastActiveDialog = getActiveDialogFromAllDialogs(getRootDialog());
            }
            return _lastActiveDialog;
        }

        #endregion

        #region Internal Getters

        private Dialog getRootDialog()
        {
            Dialog root = this;
            while (root.Parent != null) root = root.Parent;
            return root;
        }

        private Dialog getActiveDialogFromAllDialogs(Dialog dialog)
        {
            if (dialog.IsActive) return dialog;

            Dialog activeDialog = null;
            List<DialogEntry> dialogEntryList = dialog.GetEntryList();
            foreach (DialogEntry entry in dialogEntryList)
            {
                if (entry.Type == DialogEntryType.Submenu && entry.GetSubmenu() != null)
                {
                    if (entry.GetSubmenu().IsActive) return entry.GetSubmenu();
                    else if (activeDialog == null) activeDialog = getActiveDialogFromAllDialogs(entry.GetSubmenu());
                }
            }
            return activeDialog;
        }

        private DialogEntry getActivatedEntryFromAllDialogsRecursion(Dialog root)
        {
            DialogEntry rootActivated = root.ActivatedEntry;
            DialogEntry returnEntry = null;

            if (rootActivated != null) returnEntry = rootActivated;
            else
            {
                List<DialogEntry> rootEntryList = root.GetEntryList();

                foreach (DialogEntry entry in rootEntryList)
                {
                    if (entry.Type == DialogEntryType.Submenu && entry.GetSubmenu() != null)
                    {
                        DialogEntry recursiveReturnEntry = getActivatedEntryFromAllDialogsRecursion(entry.GetSubmenu());
                        if (recursiveReturnEntry != null) returnEntry = recursiveReturnEntry;
                    }
                }
            }
            return returnEntry;
        }

        #endregion

        #region Entry Focus Handling

        #region Requests

        /// <summary>
        /// Gets the index of the current selection inside the list of entries.
        /// </summary>
        /// <returns>the zero-based index of the currently selected entry or -1 if the index cannot bee achieved.</returns>
        public int GetCurrentSelectionIndex()
        {
            if (SelectedEntry == null) return -1;
            return entryList.IndexOf(SelectedEntry);
        }

        /// <summary>
        /// Determines whether there is a previous element to select.
        /// </summary>
        /// <returns>
        ///   <c>false</c> the currently selected element is the first; otherwise, <c>true</c>.
        /// </returns>
        public bool HasPreviousElement()
        {
            if (SelectedEntry != null)
            {
                int index = GetCurrentSelectionIndex();
                if (index > 0) { } // normal
                else if (index == 0) return false; // first
                else // ERROR
                {
                    // selected Entry is not longer part of the list of entries ... kill it!
                    unregisterFromEvent(SelectedEntry);
                    SelectedEntry = null;
                }
            }
            return entryList != null && entryList.Count > 0;
        }

        /// <summary>
        /// Determines whether there is a next element to select.
        /// </summary>
        /// <returns>
        ///   <c>false</c> if this is the last element; otherwise <c>true</c>.
        /// </returns>
        public bool HasNextElement()
        {
            if (SelectedEntry != null)
            {
                int index = GetCurrentSelectionIndex();
                if (index >= 0)
                {
                    if (index == (entryList.Count - 1)) return false; // last;
                    // normal 
                }
                else // ERROR
                {
                    // selected Entry is not longer part of the list of entries ... kill it!
                    unregisterFromEvent(SelectedEntry);
                    SelectedEntry = null;
                }
            }
            return entryList != null && entryList.Count > 0;
        }

        #endregion

        #region Selection Handling

        /// <summary>
        /// Selects the previous entry.
        /// </summary>
        /// <returns><c>true</c> if previous entry exists. Else <c>false</c>.</returns>
        public bool SelectPreviousEntry()
        {

            // TODO: handle


            if (SelectedEntry == null)
            {
                return SelectLastEntry();
            }
            else
            {
                //if (SelectedEntry.Type == DialogEntryType.Group && !SelectedEntry.IsFirstGroupEntrySelected())
                //{
                //    List<DialogEntry> groupEntryList = SelectedEntry.GetGroupList();
                //    DialogEntry selectedGroupEntry = SelectedEntry.GetSelectedGroupEntry();

                //    if (groupEntryList != null && groupEntryList.Count > 0)
                //    {
                //        if (selectedGroupEntry == null)
                //        {
                //            return selectGroupEntry(groupEntryList.ElementAt(groupEntryList.Count - 1));
                //        }
                //        else
                //        {
                //            int selectedIndex = groupEntryList.IndexOf(selectedGroupEntry);
                //            deselectGroupEntry(selectedGroupEntry);
                //            selectedIndex--;
                //            selectGroupEntry(groupEntryList.ElementAt(selectedIndex));
                //            return true;
                //        }
                //    }
                //}
                //else
                {
                    int selectedIndex = entryList.IndexOf(SelectedEntry);
                    if (0 <= selectedIndex - 1)
                    {
                        if (entryList.ElementAt(selectedIndex - 1) != null)
                        {
                            //TODO: implement
                            //if (SelectedEntry.Type == DialogEntryType.Group) deselectGroupEntry(SelectedEntry.GetSelectedGroupEntry());
                            //deselectEntry(SelectedEntry);

                            DialogEntry nextEntry = entryList.ElementAt(selectedIndex - 1);
                            //if (nextEntry.Type == DialogEntryType.Group)
                            //{
                            //    List<DialogEntry> groupList = nextEntry.GetGroupList();
                            //    if (groupList != null && groupList.Count > 0) selectGroupAndGroupEntry(nextEntry, groupList.ElementAt(groupList.Count - 1));
                            //}
                            //else 

                            selectEntry(nextEntry);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Selects the next entry.
        /// </summary>
        /// <returns><c>true</c> if next entry exists. Else <c>false</c>.</returns>
        public bool SelectNextEntry()
        {
            if (SelectedEntry == null)
            {
                return SelectFirstEntry();
            }
            else
            {
                //TODO: implement

                // if (SelectedEntry.Type == DialogEntryType.Group && )
                //{
                //    List<DialogEntry> groupEntryList = SelectedEntry.GetGroupList();
                //    DialogEntry selectedGroupEntry = SelectedEntry.GetSelectedGroupEntry();

                //    if (groupEntryList != null && groupEntryList.Count > 0)
                //    {
                //        if (selectedGroupEntry == null)
                //        {
                //            return selectGroupEntry(groupEntryList.ElementAt(0));
                //        }
                //        else
                //        {
                //            int selectedIndex = groupEntryList.IndexOf(selectedGroupEntry);
                //            deselectGroupEntry(selectedGroupEntry);
                //            selectedIndex++;
                //            selectGroupEntry(groupEntryList.ElementAt(selectedIndex));
                //            return true;
                //        }
                //    }
                //}
                //else
                {
                    int selectedIndex = entryList.IndexOf(SelectedEntry);

                    if (entryList.Count > selectedIndex + 1)
                    {
                        if (entryList.ElementAt(selectedIndex + 1) != null)
                        {
                            //TODO: implement
                            //    if (SelectedEntry.Type == DialogEntryType.Group) deselectGroupEntry(SelectedEntry.GetSelectedGroupEntry());
                            //    deselectEntry(SelectedEntry);

                            DialogEntry nextEntry = entryList.ElementAt(selectedIndex + 1);
                            //if (nextEntry.Type == DialogEntryType.Group)
                            //{
                            //    List<DialogEntry> groupList = nextEntry.GetGroupList();
                            //    if (groupList != null && groupList.Count > 0) 
                            //        selectGroupAndGroupEntry(nextEntry, groupList.ElementAt(0));
                            //}
                            //else 

                            selectEntry(nextEntry);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Selects the first selectable entry of this dialog.
        /// </summary>
        /// <returns><c>true</c> if an entry was selected.</returns>
        public virtual bool SelectFirstEntry()
        {
            if (entryList != null && entryList.Count > 0)
            {
                DialogEntry firstEntry = entryList.ElementAt(0);
                // FIXME: select the first entry of a group?
                return selectEntry(firstEntry);
            }
            return false;
        }

        /// <summary>
        /// Selects the last entry of this dialog.
        /// </summary>
        /// <returns><c>true</c> if an entry was selected.</returns>
        public virtual bool SelectLastEntry()
        {
            if (entryList != null && entryList.Count > 0)
            {
                DialogEntry lastEntry = entryList.ElementAt(entryList.Count - 1);

                if (lastEntry.Type == DialogEntryType.Group)
                {
                    if (lastEntry is IIteratable) // if it is a group
                    {
                        var realyLastEntry = ((IIteratable)lastEntry).GetLast() as DialogEntry;
                        if (realyLastEntry != null) lastEntry = realyLastEntry;
                    }
                    else if (lastEntry.HasChildren()) // if it is a user defined groupe type
                    {
                        var childs = lastEntry.GetChildEntryList();
                        if (childs != null && childs.Count > 0)
                        {
                            var realyLastEntry = childs[childs.Count - 1] as DialogEntry;
                            if (realyLastEntry != null) lastEntry = realyLastEntry;
                        }
                    }
                }
                selectEntry(lastEntry);
            }
            return false;
        }

        /// <summary>
        /// Selects an entry.
        /// </summary>
        /// <param name="dialogEntry">The entry to mark as selected.</param>
        /// <returns><c>true</c> if the entry could be selected.</returns>
        public virtual bool SelectEntry(DialogEntry dialogEntry)
        {
            if (dialogEntry != null && entryList.Contains(dialogEntry))
            {
                //if (IsInGroup(dialogEntry) && (dialogEntry.Type == DialogEntryType.Checkbox || dialogEntry.Type == DialogEntryType.RadioButton))
                //{
                //    DialogEntry entryGroup = getGroupFromGroupEntry(dialogEntry);
                //    if (entryGroup != null)
                //    {
                //        if (SelectedEntry == null || SelectedEntry != entryGroup)
                //            return selectGroupAndGroupEntry(entryGroup, dialogEntry);
                //        else return selectGroupEntry(dialogEntry);
                //    }
                //}
                //else 
                return selectEntry(dialogEntry);
            }
            return false;
        }


        #region Selection

        private bool selectEntry(DialogEntry entry)
        {
            if (entry != null && entryList.Contains(entry))
            {
                foreach (DialogEntry dEntry in entryList)
                {
                    if (dEntry.Status.HasFlag(DialogEntryStatus.Selected))
                    {
                        deselectEntry(dEntry);
                    }
                }
                SelectedEntry = entry;

                entry.Status |= DialogEntryStatus.Selected;

                fire_EntrySelected(entry);
                return true;
            }
            return false;
        }

        #endregion

        #region Deselection

        /// <summary>
        /// Deselects the selected entry.
        /// </summary>
        /// <returns></returns>
        public virtual bool DeselectSelectedEntry()
        {
            if (SelectedEntry != null)
            {
                return deselectEntry(SelectedEntry);
            }
            return false;
        }


        private bool deselectEntry(DialogEntry entry)
        {
            if (entry != null)
            {
                bool throwEvent = false;
                if (entryList.Contains(entry) && entry.Status.HasFlag(DialogEntryStatus.Selected))
                {
                    SelectedEntry = null;
                    throwEvent = true;
                }
        
                //if (entry.Type == DialogEntryType.Group) deselectGroupEntry(entry.GetSelectedGroupEntry());
                if (entry.Type == DialogEntryType.EditField && entry.Status.HasFlag(DialogEntryStatus.Editing))
                {
                    //Will be deactivated upon loosing focus (selected state)
                    entry.Status = DialogEntryStatus.Normal;
                    fire_EntryDeactivated(entry);
                }
                else entry.Status = entry.Status ^ DialogEntryStatus.Selected;

                if (throwEvent)
                {
                    fire_EntryDeselected(entry);
                }
                return true;
            }

            return false;
        }

        private void deselectGroupEntry(DialogEntry dialogEntry)
        {
            if (dialogEntry != null && dialogEntry.Status.HasFlag(DialogEntryStatus.Selected))
            {

                dialogEntry.Status = dialogEntry.Status ^ DialogEntryStatus.Selected;

                fire_EntryDeselected(dialogEntry);
            }
        }

        #endregion

        /// <summary>
        /// Deselects all selected entries.
        /// </summary>
        /// <param name="root">The root dialog to start deselcetion in all its subdialogs.</param>
        public void DeselectAllSelectedEntries(Dialog root = null)
        {
            if (root == null) root = this;
            DialogEntry rootSelected = root.GetSelectedEntry();
            if (rootSelected != null) root.deselectEntry(rootSelected);
            List<DialogEntry> rootEntryList = root.GetEntryList();

            foreach (DialogEntry entry in rootEntryList)
            {
                if (entry.Type == DialogEntryType.Submenu && entry.GetSubmenu() != null)
                {
                    DeselectAllSelectedEntries(entry.GetSubmenu());
                }
            }
        }

        #endregion

        #region Activation Handling

        #region Activation
        /// <summary>
        /// Activates the selected entry or if the selected entry is already activated deactivates it.
        /// </summary>
        /// <returns><c>true</c> or <c> false</c>.</returns>
        public virtual bool ActivateSelectedEntry()
        {
            bool success = false;
            DialogEntry selectedEntry = this.SelectedEntry;

            if (selectedEntry != null && !selectedEntry.Status.HasFlag(DialogEntryStatus.Disabled))
            {
                switch (selectedEntry.Type)
                {
                    case DialogEntryType.Submenu:
                        success = activateSubmenu(selectedEntry);
                        break;
                    case DialogEntryType.Checkbox:
                        success = activateCheckbox(selectedEntry);
                        break;
                    case DialogEntryType.RadioButton:
                        success = activateRadioButton(selectedEntry);
                        break;
                    case DialogEntryType.Group:
                        success = activateGroup(selectedEntry);
                        break;
                    case DialogEntryType.EditField:
                        success = activateEditField(selectedEntry);
                        break;
                    case DialogEntryType.Button:
                    case DialogEntryType.Label:
                    case DialogEntryType.EditField_Label:
                        break;
                    case DialogEntryType.Unknown:
                    case DialogEntryType.Activation:
                    //break;
                    default:
                        if (selectedEntry != ActivatedEntry)
                        {
                            //int selectedIndex = entryList.IndexOf(selected);
                            success = activateEntry(selectedEntry);
                        }
                        else
                        {
                            deactivateEntry(ActivatedEntry);
                            success = true;
                        }
                        break;
                }
            }

            return success;
        }

        private bool activateSubmenu(DialogEntry selectedEntry)
        {
            bool success = false;
            if (selectedEntry.Type == DialogEntryType.Submenu)
            {
                success = FocusOnChild();
            }
            return success;
        }

        private static bool activateGroup(DialogEntry selectedEntry)
        {
            bool success = false;
            //TODO: implement this
            if (selectedEntry != null && selectedEntry.Type == DialogEntryType.Group)
            {
                //if (!selectedEntry.Status.HasFlag(DialogEntryStatus.Activated)) activateEntry(selectedEntry);

                //DialogEntry selectedGroupEntry = selectedEntry.GetSelectedGroupEntry();
                //if (selectedGroupEntry != null)
                //{
                //    if (selectedGroupEntry.Status.HasFlag(DialogEntryStatus.Checked))
                //    {
                //        deactivateGroupEntry(selectedGroupEntry);
                //        selectedGroupEntry.Status |= DialogEntryStatus.Selected;
                //    }

                //    else
                //    {
                //        if (selectedGroupEntry.Type == DialogEntryType.Checkbox)
                //        {
                //            return activateGroupEntry(selectedGroupEntry);
                //        }
                //        else if (selectedGroupEntry.Type == DialogEntryType.RadioButton)
                //        {

                //            List<DialogEntry> groupEntryList = SelectedEntry.GetGroupList();
                //            if (groupEntryList != null && groupEntryList.Count > 0)
                //            {
                //                foreach (DialogEntry groupEntry in groupEntryList)
                //                {
                //                    if (groupEntry.Status.HasFlag(DialogEntryStatus.Activated)) deactivateGroupEntry(groupEntry);
                //                }
                //            }

                //            return activateGroupEntry(selectedGroupEntry);
                //        }
                //    }
                //}
            }
            return success;
        }

        private bool activateRadioButton(DialogEntry selectedEntry)
        {
            bool success = false;
            if (selectedEntry.Type == DialogEntryType.RadioButton)
            {
                if (!CheckedEntries.Contains(selectedEntry))
                {
                    selectedEntry.Status = DialogEntryStatus.Checked | DialogEntryStatus.Selected;
                    //selected.Status = selected.Status ^ DialogEntryStatus.Unchecked;
                    CheckedEntries.Add(selectedEntry);

                    if (selectedEntry.IsInGroup())
                    {
                        Group_DialogEntry parent = ((Group_DialogEntry)selectedEntry.ParentEntry);

                        if (parent != null && parent.HasChildren())
                            foreach (DialogEntry entry in parent.GetChildEntryList())
                            {
                                if (entry != selectedEntry && CheckedEntries.Contains(entry))
                                {
                                    CheckedEntries.Remove(entry); // other radiobuttons of the same group have to be deactivated
                                    fire_EntryUnchecked(entry);
                                }
                            }
                    }

                    fire_EntryChecked(selectedEntry);
                    success = true;
                }
                else
                {
                    success = false; // radiobutton can not be unchecked
                }
            }
            return success;
        }

        private bool activateCheckbox(DialogEntry _selectedEntry)
        {
            bool success = false;
            if (_selectedEntry != null && _selectedEntry.Type == DialogEntryType.Checkbox)
            {
                if (!CheckedEntries.Contains(_selectedEntry))
                {
                    _selectedEntry.Status = DialogEntryStatus.Checked | DialogEntryStatus.Selected;
                    //selected.Status = selected.Status ^ DialogEntryStatus.Unchecked;
                    CheckedEntries.Add(_selectedEntry);
                    fire_EntryChecked(_selectedEntry);
                    success = true;
                }
                else
                {
                    _selectedEntry.Status = DialogEntryStatus.Unchecked | DialogEntryStatus.Selected;
                    //selected.Status = selected.Status ^ DialogEntryStatus.Checked;
                    CheckedEntries.Remove(_selectedEntry);
                    fire_EntryUnchecked(_selectedEntry);
                    success = true;
                }
            }
            return success;
        }

        private bool activateEditField(DialogEntry selectedEntry)
        {
            bool success = true;
            if (_selectedEntry != null && _selectedEntry.Type == DialogEntryType.EditField)
            {
                EditField_DialogEntry editFieldEntry = (EditField_DialogEntry)selectedEntry;

                if (!selectedEntry.Status.HasFlag(DialogEntryStatus.Editing))
                {
                    editFieldEntry.Status = DialogEntryStatus.Selected | DialogEntryStatus.Editing;
                    success = true;
                    fire_EntryActivated(selectedEntry);
                }
                else
                {
                    editFieldEntry.Status = DialogEntryStatus.Normal | DialogEntryStatus.Selected;
                    success = true;
                    fire_EntryDeactivated(selectedEntry);
                }
            }

            return success;
        }

        private bool activateEntry(DialogEntry entry)
        {
            if (entry != null && entryList.Contains(entry) &&
                entry.Status.HasFlag(DialogEntryStatus.Selected) &&
                !entry.Status.HasFlag(DialogEntryStatus.Disabled) &&
                entry.Type != DialogEntryType.Submenu)
            {
                DeactivateAllEntries(getRootDialog());

                ActivatedEntry = SelectedEntry;
                entry.Status = DialogEntryStatus.Activated | DialogEntryStatus.Selected;
                fire_EntryActivated(ActivatedEntry);
                return true;
            }
            else
                return false;
        }

        #endregion

        #region Deactivation

        public virtual void DeactivateActiveEntry()
        {
            if (ActivatedEntry != null)
            {
                deactivateEntry(ActivatedEntry);
            }
            else DeactivateAllEntries(getRootDialog());
        }

        /// <summary>
        /// Deactivates all active entries.
        /// </summary>
        /// <param name="root">The root.</param>
        public virtual void DeactivateAllEntries(Dialog root)
        {
            DialogEntry rootActivated = root.ActivatedEntry;
            if (rootActivated != null) root.deactivateEntry(rootActivated);
            List<DialogEntry> rootEntryList = root.GetEntryList();

            foreach (DialogEntry entry in rootEntryList)
            {
                if (entry.Type == DialogEntryType.Submenu && entry.GetSubmenu() != null)
                {
                    DeactivateAllEntries(entry.GetSubmenu());
                }
            }
        }

        /// <summary>
        /// Deactivates the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns><c>true</c> if entry is not null, has Active Flag and is in the dialog. Else <c>false</c>.</returns>
        private bool deactivateEntry(DialogEntry entry)
        {
            if (entry != null && entryList.Contains(entry) && entry.Status.HasFlag(DialogEntryStatus.Activated))
            {
                ActivatedEntry = null;

                if (entry.Status.HasFlag(DialogEntryStatus.Selected)) entry.Status = DialogEntryStatus.Normal | DialogEntryStatus.Selected;
                else entry.Status = DialogEntryStatus.Normal;

                fire_EntryDeactivated(entry);
                return true;
            }
            else return false;
        }

        #endregion

        #endregion

        #region Dialog Tree Shifting

        /// <summary>
        /// Shift to the child dialog of the selected entry.
        /// </summary>
        /// <returns><c>true</c> if entry is not null, has the Submenu Type Flag and its Submenu is not null. Else <c>false</c>.</returns>
        public virtual bool FocusOnChild()
        {
            if (SelectedEntry != null)
            {
                if (SelectedEntry.Type == DialogEntryType.Submenu)
                {
                    if (SelectedEntry.GetSubmenu() != null)
                    {
                        this.IsActive = false;
                        DialogEntry entry = entryList.ElementAt(entryList.IndexOf(SelectedEntry));
                        entry.GetSubmenu().IsActive = true;

                        fire_SwitchedToChildDialog();
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual bool FocusOnFirstChild()
        {
            foreach (DialogEntry entry in entryList)
            {
                if (entry.Type == DialogEntryType.Submenu && entry.GetSubmenu() != null)
                {
                    this.IsActive = false;
                    entry.GetSubmenu().IsActive = true;
                    fire_SwitchedToChildDialog();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Shift to the parent dialog if it does not has Focus [IsActive = false].
        /// </summary>
        /// <returns>Returns <c>true</c> if Focus is on Submenu. Else <c>false</c>.</returns>
        public virtual bool FocusOnParent()
        {
            if (this.IsActive && this.Parent != null)
            {
                this.IsActive = false;
                this.Parent.IsActive = true;
                fire_SwitchedToParentDialog();
                return true;
            }
            else return false;
        }

        public virtual bool FocusOnRoot()
        {
            if (this.IsActive && getRootDialog() != null)
            {
                this.IsActive = false;
                Dialog root = getRootDialog();
                DeselectAllSelectedEntries(root);
                //resetAllScrollHights(root);
                root.IsActive = true;
                fire_SwitchedToParentDialog();
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Focuses on dialog.
        /// </summary>
        /// <param name="dlg">The dialog.</param>
        /// <returns></returns>
        public virtual bool FocusOn()
        {
            if (this.ID != null)
            {
                Dialog root = getRootDialog();

                Dialog activeDialog = root.GetActiveDialog();
                if (activeDialog != null) activeDialog.IsActive = false;

                DeselectAllSelectedEntries(root);

                this.IsActive = true;
                fire_SwitchedToDialog(this);
                return true;
            }
            else return false;
        }

        #endregion

        #endregion

        #region Entry List Handling

        /// <summary>
        /// Removes all entries from this dialog.
        /// </summary>
        /// <returns>if all entries could be removed.</returns>
        public virtual bool Clear()
        {
            var entries = GetEntryList();
            foreach (var item in entries)
            {
                RemoveEntry(item);
            }

            return EntryCount == 0;
        }

        /// <summary>
        /// Adds an entry to the list of dialog elements.
        /// </summary>
        /// <param name="entry">The entry to add.</param>
        public virtual bool AddEntry(DialogEntry entry)
        {

            bool success = false;

            if (entry != null && HasUniqueID(entry))
            {
                _semaphore.WaitOne();
                try
                {
                    if (entry.Type == DialogEntryType.Submenu && entry.GetSubmenu() != null)
                    {
                        entry.GetSubmenu().Parent = this;
                        if (entry.GetSubmenu().Parent != null)
                        {
                            entryList.Add(entry);
                            fire_EntryAdded(entry);
                        }
                    }
                    else
                    {
                        entryList.Add(entry);
                        if ((entry.Type == DialogEntryType.RadioButton || entry.Type == DialogEntryType.Checkbox) && entry.Status == DialogEntryStatus.Checked)
                        {
                            CheckedEntries.Add(entry);
                        }
                        entry.ParentDialog = this;
                        fire_EntryAdded(entry);
                    }

                    //if (entry.Status.HasFlag(DialogEntryStatus.Selected))
                    //{
                    //    deselectEntry(SelectedEntry);
                    //    SelectedEntry = entry;
                    //}
                    //if (entry.Status.HasFlag(DialogEntryStatus.Activated))
                    //{
                    //    DeactivateAllEntries(this.getRootDialog());
                    //    ActivatedEntry = entry;
                    //}

                    if (entryDict.ContainsKey(entry.ID)) entryDict[entry.ID] = entry;
                    else entryDict.Add(entry.ID, entry);

                    //_semaphore.Release();
                }
                finally { _semaphore.Release(); }
                // add all children
                if (entry.HasChildren())
                {
                    addAllChildrenOfEntry(entry);
                }

                success = true;
            }

            return success;

        }

        /// <summary>
        /// Adds an entry after a specific.
        /// </summary>
        /// <param name="entryToAdd">The entry to add.</param>
        /// <param name="entryID">The entry identifier of the previous entry.</param>
        /// <returns><c>true</c> if successfully added; otherwise <c>false</c>.</returns>
        public virtual bool AddAfter(DialogEntry entryToAdd, DialogEntry entry)
        {
            if (entryToAdd != null && entry != null && entryList.Contains(entry))
            {
                int entryIndex = entryList.IndexOf(entry);
                return AddAtPosition(entryToAdd, entryIndex + 1);
            }
            return false;
        }

        /// <summary>
        /// Adds an entry before a specific.
        /// </summary>
        /// <param name="entryToAdd">The entry to add.</param>
        /// <param name="entryID">The entry identifier of the following.</param>
        /// <returns><c>true</c> if successfully added; otherwise <c>false</c>.</returns>
        public virtual bool AddBefore(DialogEntry entryToAdd, DialogEntry entry)
        {
            if (entryToAdd != null && entry != null && entryList.Contains(entry))
            {
                int entryIndex = entryList.IndexOf(entry);
                return AddAtPosition(entryToAdd, entryIndex);
            }
            return false;
        }

        /// <summary>
        /// Adds an entry at a specific position.
        /// </summary>
        /// <param name="entryToAdd">The entry to add.</param>
        /// <param name="index">The index to add in.</param>
        /// <param name="replace">if set to <c>true</c> if you want to replace the element at the position.</param>
        /// <returns><c>true</c> if successfully added; otherwise <c>false</c>.</returns>
        public virtual bool AddAtPosition(DialogEntry entryToAdd, int index, bool replace = false)
        {
            bool entryAdded = false;

            if (entryToAdd != null && HasUniqueID(entryToAdd))
            {
                _semaphore.WaitOne();
                try
                {
                    index = Math.Max(0, index);

                    if (replace)
                    {
                        if (entryList.Count < index)
                        {
                            entryList[index] = entryToAdd;
                        }
                        else
                        {
                            entryList.Add(entryToAdd);
                        }
                        entryAdded = true;
                    }
                    else
                    {
                        if (entryList.Count > index)
                        {
                            entryList.Insert(index, entryToAdd);
                        }
                        else
                        {
                            entryList.Add(entryToAdd);
                        }

                        entryAdded = true;
                    }
                    //_semaphore.Release();
                }
                finally { _semaphore.Release(); }
            }

            if (entryAdded)
            {
                if ((entryToAdd.Type == DialogEntryType.RadioButton || entryToAdd.Type == DialogEntryType.Checkbox) && entryToAdd.Status == DialogEntryStatus.Checked)
                {
                    CheckedEntries.Add(entryToAdd);
                }

                if (entryDict.ContainsKey(entryToAdd.ID)) entryDict[entryToAdd.ID] = entryToAdd;
                else entryDict.Add(entryToAdd.ID, entryToAdd);

                fire_EntryAdded(entryToAdd);

                if (entryToAdd.Status.HasFlag(DialogEntryStatus.Selected))
                {
                    deselectEntry(SelectedEntry);
                    SelectedEntry = entryToAdd;
                }
                if (entryToAdd.Status.HasFlag(DialogEntryStatus.Activated))
                {
                    DeactivateAllEntries(this.getRootDialog());
                    ActivatedEntry = entryToAdd;
                }
            }
            return entryAdded;
        }

        /// <summary>
        /// Deletes an entry.
        /// </summary>
        /// <param name="entry">The entry to delete.</param>
        public virtual void RemoveEntry(DialogEntry entry)
        {
            _semaphore.WaitOne();
            try
            {
                if (entry != null && entryList.Contains(entry))
                {
                    if (SelectedEntry == entry) deselectEntry(entry);
                    if (ActivatedEntry == entry) deactivateEntry(entry);

                    entryList.Remove(entry);

                    if (entryDict.ContainsKey(entry.ID))
                        entryDict.Remove(entry.ID);

                    fire_EntryRemoved(entry);
                }
                //_semaphore.Release();
            }
            finally { _semaphore.Release(); }
        }

        /// <summary>
        /// Gets an registered entry by its identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="handleSubmenus">if set to <c>true</c> sub-menus will be searched as well..</param>
        /// <returns>
        /// The entry if it is part of this dialog or <c>null</c>
        /// </returns>
        public virtual DialogEntry GetEntryByID(String id, bool handleSubmenus = true)
        {
            if (entryDict.ContainsKey(id)) return entryDict[id];
            else if (handleSubmenus)
            {
                _semaphore.WaitOne();
                try
                {
                    foreach (var item in entryList)
                    {
                        if (item != null && item.HasSubmenu())
                        {
                            var subMen = item.GetSubmenu();
                            if (subMen != null && subMen.EntryCount > 0)
                            {
                                var entry = subMen.GetEntryByID(id, true);
                                if (entry != null) return entry;
                            }
                        }
                    }

                    //_semaphore.Release();
                }
                finally { _semaphore.Release(); }
            }
            return null;
        }


        protected virtual void addAllChildrenOfEntry(DialogEntry entry)
        {
            if (entry != null && entry.HasChildren())
            {
                foreach (var item in entry.GetChildEntryList())
                {
                    if (item is DialogEntry)
                    {
                        AddEntry(item as DialogEntry);
                    }
                }
            }
        }


        /// <summary>
        /// Gets the (child) dialog by identifier.
        /// </summary>
        /// <param name="id">The identifier of the dialog to search.</param>
        /// <returns>The dialog with the specified ID (can be the root) or <c>null</c>.</returns>
        public Dialog GetDialogByID(string id)
        {

            Dialog root = this.getRootDialog();
            return getDialogByID(root, id);
        }

        private Dialog getDialogByID(Dialog dlg, string id)
        {
            Dialog newDialog = null;

            //lock (SyncLock)
            //{
            try
            {
                if (dlg.ID.Equals(id)) newDialog = dlg;
                else
                {


                    foreach (DialogEntry entry in dlg.GetEntryList())
                    {
                        if (entry.HasSubmenu() && entry.GetSubmenu() != null && entry.GetSubmenu().ID != null && entry.GetSubmenu().ID.Equals(id)) { return entry.GetSubmenu(); }
                        else
                        {
                            if (entry.HasSubmenu() && entry.GetSubmenu() != null && newDialog == null) newDialog = getDialogByID(entry.GetSubmenu(), id);

                        }
                    }
                }
            }
            catch { }
            //}

            return newDialog;

        }


        #region Entry Listeners

        void registerToEvents(DialogEntry entry)
        {
            if (entry != null)
            {
                entry.DialogEntryChanged += entry_DialogEntryChanged;
            }
        }

        void unregisterFromEvent(DialogEntry entry)
        {
            if (entry != null)
            {
                try { entry.DialogEntryChanged -= entry_DialogEntryChanged; }
                catch { }
            }
        }

        void entry_DialogEntryChanged(object sender, EntryEventArgs e)
        {
            if (e != null && e.Entry != null)
            {
                System.Diagnostics.Debug.WriteLine("DIALOG ENTRY EVENT: " + e.Entry.ToString() + " STATUS: " + e.Entry.Status);

                // TODO: check for selection
                // SELECTION
                if (SelectedEntry == e.Entry)
                {
                    if (!e.Entry.Status.HasFlag(DialogEntryStatus.Selected))
                    {
                        SelectedEntry = null;
                    }
                }
                else if (e.Entry.Status.HasFlag(DialogEntryStatus.Selected))
                {
                    SelectedEntry = e.Entry;
                }

                // ACTIVATION
                // check for activation
                if (ActivatedEntry == e.Entry)
                {
                    if (!e.Entry.Status.HasFlag(DialogEntryStatus.Activated) || e.Entry.Status.HasFlag(DialogEntryStatus.Disabled))
                    {
                        ActivatedEntry = null;
                    }
                }
                else if (e.Entry.Status.HasFlag(DialogEntryStatus.Activated) && !e.Entry.Status.HasFlag(DialogEntryStatus.Disabled))
                {
                    ActivatedEntry = e.Entry;
                }


                if (e != null && e is DetailedEntryChangedEventArgs && e.Entry != null)
                {
                    if (((DetailedEntryChangedEventArgs)e).ChangedProperty.Equals("Status"))
                    {
                        //Firing just for when Entry was checked or unchecked, so now commented. Event is used in BrailleIO_D2F
                        //if (e.Entry.Status.HasFlag(DialogEntryStatus.Unchecked)) fire_EntryUnchecked(e.Entry);
                        //else if (e.Entry.Status.HasFlag(DialogEntryStatus.Checked)) fire_EntryChecked(e.Entry);
                    }
                }



                //// ID check;
                // FIXME: 
                //if (entryDict.ContainsKey(e.Entry.ID))
                //{
                //    if (entryDict[e.Entry.ID] != e.Entry) // FATAL ERROR: ID changed to a duplicate;
                //    { }
                //}
                //else
                //{
                //    // find the entry ....
                //}
            }
        }

        #endregion

        #endregion

        #region Check Functions

        private bool checkForDialogCyclus(Dialog dialogParent)
        {
            Dialog dialog = dialogParent;
            while (dialog != null)
            {
                if (dialog == this) return true;
                else dialog = dialog.Parent;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the ID is unique or already used.
        /// </summary>
        /// <param name="ID">The identifier.</param>
        /// <returns>
        ///   <c>true</c> if it is an unique identifier; otherwise, <c>false</c>.
        /// </returns>
        public bool IsUniqueID(String ID)
        {
            if (!String.IsNullOrWhiteSpace(ID))
            {
                if (!entryDict.ContainsKey(ID)) return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether this entry has an unique identifier.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        ///   <c>true</c> if it has an unique identifier; otherwise, <c>false</c>.
        /// </returns>
        public bool HasUniqueID(DialogEntry entry)
        {
            if (entry != null)
            {
                bool isUnique = IsUniqueID(entry.ID);
                if (!isUnique && entryDict.ContainsKey(entry.ID))
                    isUnique = entry == entryDict[entry.ID];
                return isUnique;
            }

            //foreach (DialogEntry dialogEntry in entryList)
            //{
            //    if (dialogEntry.ID.Equals(entry.ID))
            //    {
            //        System.Console.WriteLine("ID " + entry.ID + " is already used in DialogEntry " + entry.Text);
            //        return true;
            //    }
            //}
            return false;
        }

        #endregion

    }

    #region Enum

    /// <summary>
    /// Determine the type, style and behavior of a <see cref="Dialog"/>
    /// </summary>
    public enum DialogType
    {
        /// <summary>
        /// unknown or user defined type
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The default behavior
        /// </summary>
        Default = 1,
        /// <summary>
        /// Dialog is used like a menu
        /// </summary>
        Menu,
        /// <summary>
        /// Dialog is used like a message box
        /// </summary>
        Info,
        /// <summary>
        /// Dialog is used like a message box with a question.
        /// </summary>
        Question
    }

    #endregion

    #region Event Args
    /// <summary>
    /// Event arguments for dialog entry events.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class EntryEventArgs : EventArgs
    {

        /// <summary>
        /// The dialog entry causing this event.
        /// </summary>
        public readonly DialogEntry Entry;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryEventArgs"/> class.
        /// </summary>
        /// <param name="entry">The dialog entry causing this event.</param>
        public EntryEventArgs(DialogEntry entry)
        {
            Entry = entry;
        }

    }

    /// <summary>
    /// Event arguments for dialog events.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class DialogEventArgs : EventArgs
    {

        /// <summary>
        /// The dialog causing this event.
        /// </summary>
        public readonly Dialog Dialog;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogEventArgs"/> class.
        /// </summary>
        /// <param name="entry">The dialog causing this event.</param>
        public DialogEventArgs(Dialog dialog)
        {
            Dialog = dialog;
        }

    }
    #endregion
}
