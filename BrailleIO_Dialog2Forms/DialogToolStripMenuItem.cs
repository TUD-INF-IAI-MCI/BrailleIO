//Autor:    Stephanie Schöne
// TU Dresden, Germany

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BrailleIO.Dialogs
{
    /*Base class for Dialog_ToolStripMenuItem and DialogEntry_ToolStripMenuItem.*/
    public class DialogToolStripMenuItem : ToolStripMenuItem
    {
        #region Members

        #region Public

        /// <summary>
        /// If [AutoFollow == false] the interaction with the D2F Menu will not influence the user position inside the Dialog tree. This is its default behaviour.
        /// If [AutoFollow == true] the interaction with the D2F Menu will force the Dialog to follow its current position inside the tree.
        /// </summary>
        public Boolean AutoFollow;

        public DialogToolStripHierarchy Hierarchy;


        private string _id;

        /// <summary>
        /// Gets or sets the dialog item identifier.
        /// </summary>
        /// <value>
        /// The dialog item identifier.
        /// </value>
        public string ID
        {
            get
            {
                return _id;

            }
            set
            {
                if (value.Equals(_id)) return;
                _id = value;
            }
        }
        

        private DialogToolStripitemStatus _status;

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public DialogToolStripitemStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (value.Equals(_status)) return;
                _status = value;
            }
        }

        private DialogClassType _classType;

        /// <summary>
        /// Gets or sets the type of the class.
        /// </summary>
        /// <value>
        /// The type of the class.
        /// </value>
        public DialogClassType ClassType
        {
            get
            {
                return _classType;
            }
            set
            {
                if (value.Equals(_classType)) return;
                _classType = value;
            }
        }

        /// <summary>
        /// The parent
        /// </summary>
        public Dialog_ToolStripMenuItem Parent;

        #endregion

        #region Private 

        /*Incoming Item, if it is a Dialog*/
        public Dialog dialogItem;

        /*Incoming Item, if it is a DialogEntry*/
        public DialogEntry dialogEntryItem;

        #endregion

        #region Event Members
        /// <summary>
        /// Occurs when OnClick event triggers.
        /// </summary>
        public event EventHandler<ToolStripItemClickedEventArgs> MenuClicked;

        /// <summary>
        /// Occurs when OnDropDownClosed event triggers.
        /// </summary>
        public event EventHandler<ToolStripDropDownClosedEventArgs> MenuClosed;

        /// <summary>
        /// Occurs when OnDropDownOpened triggers.
        /// </summary>
        public event EventHandler<ToolStripItemClickedEventArgs> MenuOpened;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogToolStripMenuItem"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        public DialogToolStripMenuItem(Dialog dlg) : base(dlg.Title)
        {
            dialogItem = dlg;
            ID = dlg.ID;
            ClassType = DialogClassType.Dialog;
            Status = DialogToolStripitemStatus.Closed;
            this.ToolTipText = dlg.Help;
            AutoFollow = false;
            ToolTipText = dlg.Help;

            if (dlg.Parent == null)
            {
                Hierarchy = DialogToolStripHierarchy.Root;
            }
            else
            {
                Hierarchy = DialogToolStripHierarchy.Node;
            }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogToolStripMenuItem"/> class.
        /// </summary>
        /// <param name="dlgEntry">The dialog entry.</param>
        public DialogToolStripMenuItem(DialogEntry dlgEntry)
            : base(dlgEntry.Title)
        {
            dialogEntryItem = dlgEntry;
            ID = dlgEntry.ID;
            ClassType = DialogClassType.DialogEntry;
            this.ToolTipText = dlgEntry.Help;
            AutoFollow = false;
            ToolTipText = dlgEntry.Help;

            if (dlgEntry.HasSubmenu())
            {
                Hierarchy = DialogToolStripHierarchy.Node;
            }
            else
            {
                Hierarchy = DialogToolStripHierarchy.Leaf;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogToolStripMenuItem"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="classType">Type of the class.</param>
        public DialogToolStripMenuItem(string id, string title, DialogClassType classType)
            : base(title)
        {
            ID = id;
            ClassType = classType;
            AutoFollow = false;
            ToolTipText = "";

            Hierarchy = DialogToolStripHierarchy.Unknown;
        }

        #endregion

        #region Event Listeners


        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.ToolStripItem.Click" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.EventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnClick(EventArgs e)
        {
            if (e != null)
            {
                base.OnClick(e);
                if (this.ClassType == DialogClassType.DialogEntry)
                {
                    if (dialogEntryItem != null && dialogEntryItem.ParentDialog != null)
                    {
                        Dialog parent = dialogEntryItem.ParentDialog;

                        if (parent.GetActiveDialog() != null)
                        {
                            Dialog ActiveDialog = parent.GetActiveDialog();
                            DialogEntry SelectedEntry = ActiveDialog.SelectedEntry;

                            parent.FocusOn();
                            parent.SelectEntry(dialogEntryItem);
                            parent.ActivateSelectedEntry();

                            /*If the Dialog is not following, the prior selected entry and active dialog will be restored*/
                            if (!AutoFollow)
                            {
                                //causes activateActivation to get wrong ActiveDialog && wrong switch case is chosen
                               // ActiveDialog.FocusOn();
                               // if(SelectedEntry != null)
                                 //   ActiveDialog.SelectEntry(SelectedEntry);
                            }
                        }
                    }

                }
                else if (this.ClassType == DialogClassType.Dialog)
                { 
                    if (dialogItem != null && AutoFollow)
                    {                    
                        if (this.DropDownItems != null && this.DropDownItems.Count != null && this.DropDownItems.Count > 0 )
                        {
                            /*Dialog position should follow the navigation through the D2F structure*/
                            dialogItem.FocusOn();
                        }
                    }
                }
                fire_OnClickEvent();
            }
        }

        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.ToolStripDropDownItem.DropDownClosed" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.EventArgs" />, der die Ereignisdaten enthält.</param>
        protected override void OnDropDownClosed(EventArgs e)
        {
 	         base.OnDropDownClosed(e);
             Status = DialogToolStripitemStatus.Closed;
        
            /*If AutoFollow is true and the root Dialog_ToolStripMenuItem is closed,
             * the position in the dialog tree should return to the dialog root*/
             if (AutoFollow)  ResetDialogPosition();

             fire_OnDropDownClosed();
        }


        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.ToolStripDropDownItem.DropDownOpened" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.EventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
            Status = DialogToolStripitemStatus.Opened;

            /*If AutoFollow is true and the root Dialog_ToolStripMenuItem is opened,
             * the position in the dialog tree should return to the dialog root*/

            if (AutoFollow) ResetDialogPosition();

            fire_OnDropDownOpened();
        }


        private void fire_OnClickEvent()
        {

            if (MenuClicked != null)
            {
                try
                {
                    MenuClicked.Invoke(this, new ToolStripItemClickedEventArgs(this));
                }
                catch { }
            }
        }

       private void fire_OnDropDownClosed()
        {
            if (MenuClosed != null)
            {
                try
                {
                    MenuClosed.Invoke(this, new ToolStripDropDownClosedEventArgs(new ToolStripDropDownCloseReason()));
                }
                catch { }
            }
        }

       private void fire_OnDropDownOpened()
       {
           if (MenuOpened != null)
           {
               try
               {
                   MenuOpened.Invoke(this, new ToolStripItemClickedEventArgs(this));
               }
               catch { }
           }
       }

        #endregion


        #region Public Getter

        /// <summary>
        /// Gets the dialog tool strip item with identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public DialogToolStripMenuItem GetDialogToolStripItemWithID(string id)
        {
            if (id != null)
            {
                if (ID.Equals(id)) return this;
         
                    DialogToolStripMenuItem dsti = null;

                    if (this.DropDownItems != null && this.DropDownItems.Count > 0)
                    {
                        for (int i = 0; i < this.DropDownItems.Count; i++)
                        {
                            if (!(this.DropDownItems[i] is ToolStripSeparator))
                            {
                                DialogToolStripMenuItem item = (DialogToolStripMenuItem) this.DropDownItems[i];
                                if (item.ID.Equals(id)) return item;
                                else
                                {
                                    if (dsti == null) dsti = item.GetDialogToolStripItemWithID(id);
                                    else break;
                                } 
                            }
                        }
                    }
                    
                    return dsti;
                }
            
            else return null; 
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Resets the position in the dialog to root.
        /// </summary>
        private void ResetDialogPosition()
        {
            if (this.ClassType == DialogClassType.Dialog && AutoFollow)
            {
               // if (dialogItem.Parent == null && dialogItem != null  !dialogItem.IsActive)
                if (Hierarchy == DialogToolStripHierarchy.Root && dialogItem != null && !dialogItem.IsActive)
                {
                    dialogItem.FocusOn();
                    dialogItem.FocusOnRoot();
                }
            }
        }

        /// <summary>
        /// Activates the dialog tool strip item.
        /// </summary>
        public void ActivateDialogToolStripItem()
        {
            if (this.CheckOnClick) this.PerformClick();

        }

        /*All navigations through the DialogToolStripMenu influence the position inside of the Dialog.*/
        public virtual void ActivateAutoFollow()
        {
            AutoFollow = true;

        }

        /*Navigations through the DialogToolStripMenu do not influence the position inside of the Dialog.
         When activating entries though DialogToolStripMenu, the current Dialog position will always be restored.*/
        public virtual void DeactivateAutoFollow()
        {
            AutoFollow = false;
        }

        #endregion
    }

    #region Enum
    public enum DialogToolStripitemStatus
    {
        Unknown = 0,
        Opened = 1, /*DialogToolStripItem is currently open*/
        Closed = 2, /*DialogToolStripItem is currently closed*/
        Disabled = 3,
    }

    public enum DialogToolStripHierarchy
    {
        Unknown = 0,
        Root = 1, /*DialogToolStripItem is the root of the whole tree*/
        Node = 2, /*DialogToolStripItem is a node in the whole tree*/
        Leaf = 3, /*DialogToolStripItem is a leaf in the whole tree*/
    }

    public enum DialogClassType
    {
        Unknown = 0,
        Dialog = 1,
        DialogEntry = 2,
    }

    #endregion
}
