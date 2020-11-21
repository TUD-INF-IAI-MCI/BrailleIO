
namespace BrailleIO.Dialogs
{
    /// <summary>
    /// Special DialogEntry for radio button handling. 
    /// This class contains special handling for radio-button-control like features.
    /// You can also use the standard <see cref="DialogEntry"/> and handle the behavior 
    /// by your own.
    /// </summary>
    /// <seealso cref="BrailleIO.Dialogs.SelfRenderingDialogEntry" />
    public class RadioButton_DialogEntry : SelfRenderingDialogEntry
    {
        #region Member Overrides

        /// <summary>
        /// Gets or sets the (control)type of this entry.
        /// </summary>
        /// <value>
        /// The (control)type.
        /// Can only be <see cref="DialogEntryType.RadioButton"/>
        /// </value>
        override public DialogEntryType Type
        {
            get { return DialogEntryType.RadioButton; }
            set { }
        }

        private DialogEntryStatus _status = DialogEntryStatus.Normal;
        /// <summary>
        /// Gets or sets the status of this entry.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        override public DialogEntryStatus Status
        {
            get { return _status; }
            set
            {
                var _oldStatus = _status;
                if (value.HasFlag(DialogEntryStatus.Disabled)) _status = DialogEntryStatus.Disabled;
                else
                {
                    if (value.HasFlag(DialogEntryStatus.Unchecked))
                    {
                        if (_status.HasFlag(DialogEntryStatus.Checked))
                            uncheckThis();
                        else _status = DialogEntryStatus.Unchecked;
                    }
                    else if (value.HasFlag(DialogEntryStatus.Checked))
                    {
                        if (_status.HasFlag(DialogEntryStatus.Unchecked) || _status.HasFlag(DialogEntryStatus.Normal)) 
                            checkThis();
                    }
                    else { } // ignore
                }

                if (value.HasFlag(DialogEntryStatus.Selected))
                    _status |= DialogEntryStatus.Selected;
                else 
                    _status &= ~DialogEntryStatus.Selected;

                if (_status != _oldStatus) 
                    fire_DialogEntryChanged("Status");

            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButton_DialogEntry" /> class.
        /// </summary>
        /// <param name="ID">The unique identifier.</param>
        /// <param name="text">The text that is displayed.</param>
        /// <param name="help">The help text explaining the functionality behind this entry.</param>
        /// <param name="status">The status of the Radiobutton (should be DialogEntryStatus.Checked or DialogEntryStatus.Unchecked).</param>
        /// <param name="parentDialog">The parent dialog this entry is related to. Can be <c>null</c> - when added to a Dialog this Field will be set automatically.</param>
        /// <param name="parentEntry">The parent entry (must be some kind of group type).</param>
        public RadioButton_DialogEntry(
            string ID,
            string text,
            string help = "...",
            DialogEntryStatus status = DialogEntryStatus.Unchecked,
            DialogEntry parentEntry = null,
            Dialog parentDialog = null)
            : base(ID, text, help, DialogEntryType.RadioButton, status, parentEntry, parentDialog)
        {
            Status = status;
        }


        #endregion

        #region Helper Functions

        private void checkThis()
        {
            _status &= ~DialogEntryStatus.Unchecked;
            _status |= DialogEntryStatus.Checked;

            // TODO: 
            //  -   disable the others in this group [✓]
            //  -   or dialog [?] --> RadieoButtons must be within a group!
            if (ParentEntry != null)
            {
                var siblings = ParentEntry.GetChildEntryList();
                if (siblings != null)
                {
                    foreach (var sibling in siblings)
                    {
                        if (sibling != null && 
                            sibling != this && 
                            sibling is DialogEntry && 
                            ((DialogEntry)sibling).Type == DialogEntryType.RadioButton)
                        {
                            ((DialogEntry)sibling).Status |= DialogEntryStatus.Unchecked;
                        }
                    }
                }
            }
        }

        private bool uncheckThis()
        {
            // check if any other entry is checked instead?
            if (ParentEntry != null)
            {
                var siblings = ParentEntry.GetChildEntryList();
                if (siblings != null)
                {
                    foreach (var sibling in siblings)
                    {
                        if (
                            sibling != null && 
                            sibling != this && 
                            sibling is DialogEntry &&
                            ((DialogEntry)sibling).Type == DialogEntryType.RadioButton && 
                            ((DialogEntry)sibling).Status.HasFlag(DialogEntryStatus.Checked)
                            )
                        { // one other sibling was found that is checked, .. so we can uncheck this
                            _status &= ~DialogEntryStatus.Checked;
                            _status |= DialogEntryStatus.Unchecked;
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }

        #endregion
    }
}
