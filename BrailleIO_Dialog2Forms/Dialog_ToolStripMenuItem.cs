//Autor:    Stephanie Schöne
// TU Dresden, Germany

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BrailleIO.Dialogs
{
    public class Dialog_ToolStripMenuItem : DialogToolStripMenuItem
    {
        #region Members

        /// <summary>
        /// The last clicked DropDownItem (never ToolStripSeperator).
        /// </summary>
        private DialogToolStripMenuItem ClickedItem;

        private DialogType _type;

        /// <summary>
        /// Gets or sets the type of the dialog item.
        /// </summary>
        /// <value>
        /// The type of the dialog item.
        /// </value>
        public DialogType Type
        {
            get
            {
                return _type;

            }
            set
            {
                if (value.Equals(_type)) return;
                _type = value;

            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Dialog_ToolStripMenuItem"/> class.
        /// </summary>
        /// <param name="dlg"></param>
        public Dialog_ToolStripMenuItem(Dialog dlg): base(dlg)
        {
            Type = dlg.Type;

            registerForDialogEvents(dlg);

            DropDown.Closing += OnDropDownClosing;
            DropDown.ItemClicked += OnDropDownItemClicked;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Dialog_ToolStripMenuItem"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="id">The identifier.</param>
        public Dialog_ToolStripMenuItem(string title, string id)
            : base(title, id, DialogClassType.Dialog)
        {
            Dialog item = new Dialog(title, id);
            registerForDialogEvents(item);

            DropDown.Closing += OnDropDownClosing;
            DropDown.ItemClicked += OnDropDownItemClicked;
        }

        #endregion

        #region Events
        /// <summary>
        /// Registers for dialog events.
        /// </summary>
        /// <param name="dlg">The dialog.</param>
        private void registerForDialogEvents(Dialog dlg)
        {
            dlg.EntryActivated += d_EntryActivated;
            dlg.EntryDeactivated += d_EntryDeactivated;
            dlg.EntryChecked += d_EntryChecked;
            dlg.EntryUnchecked += d_EntryUnchecked;
            dlg.DialogChanged += d_DialogChanged;
        }

        private void d_DialogChanged(object sender, DialogEventArgs e)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Called when DropDown is Closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ToolStripDropDownClosingEventArgs"/> instance containing the event data.</param>
        void OnDropDownClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e != null && e.CloseReason != null && e.CloseReason == ToolStripDropDownCloseReason.ItemClicked && ClassType == DialogClassType.Dialog)
            {
                if (ClickedItem != null && ClickedItem.ClassType == DialogClassType.DialogEntry)
                {
                    DialogEntry_ToolStripMenuItem entry = (DialogEntry_ToolStripMenuItem) ClickedItem;

                    if(entry != null && entry.Type != DialogEntryType.Activation){
                        e.Cancel = true;
                    } 
                }
            }
        }

        /// <summary>
        /// Called when DropDownItem is clicked. Needed to get ClickedItem before deciding if Menu should be closed.
        /// Event OnClick is after DropDown.Closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ToolStripItemClickedEventArgs"/> instance containing the event data.</param>
        void OnDropDownItemClicked(Object sender, ToolStripItemClickedEventArgs e)
        {
            if (e != null && e.ClickedItem != null && !(e.ClickedItem is ToolStripSeparator))
            {
                ClickedItem = (DialogToolStripMenuItem)e.ClickedItem;
            }
        }

        /// <summary>
        /// Handles the EntryActivated event when a dialog entry is activated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EntryEventArgs"/> instance containing the event data.</param>
        void d_EntryActivated(object sender, EntryEventArgs e)
        {
            if (e != null && e.Entry != null)
            {
                DialogToolStripMenuItem entry = this.GetDialogToolStripItemWithID(e.Entry.ID);

                if (entry != null) entry.Checked = true;
            }

        }

        /// <summary>
        /// Handles the EntryDeactivated event when a dialog entry is deactivated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EntryEventArgs"/> instance containing the event data.</param>
        void d_EntryDeactivated(object sender, EntryEventArgs e)
        {
            if (e != null && e.Entry != null)
            {
                DialogToolStripMenuItem entry = this.GetDialogToolStripItemWithID(e.Entry.ID);
                if (entry != null) entry.Checked = false;
            }
        }

        /// <summary>
        /// Handles the EntryChecked event  when a dialog entry is checked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EntryEventArgs"/> instance containing the event data.</param>
        void d_EntryChecked(object sender, EntryEventArgs e)
        {
            if (e != null && e.Entry != null) 
            { 
                System.Diagnostics.Debug.WriteLine("???? Dialog Entry CH " + e.Entry.Title);
                DialogToolStripMenuItem item = this.GetDialogToolStripItemWithID(e.Entry.ID);
                if (item != null && !item.Checked) item.Checked = true;
            }
        }

        /// <summary>
        /// Handles the EntryUnchecked event when a dialog entry is unchecked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EntryEventArgs"/> instance containing the event data.</param>
        void d_EntryUnchecked(object sender, EntryEventArgs e)
        {
            if (e != null && e.Entry != null)
            {
                System.Diagnostics.Debug.WriteLine("???? Dialog Entry UCH " + e.Entry.Title);
                DialogToolStripMenuItem item = this.GetDialogToolStripItemWithID(e.Entry.ID);
                if (item != null && item.Checked) item.Checked = false;
            }
        }

        /*Should be called on root item to activate autofollow on all contained items*/
        public override void ActivateAutoFollow()
        {
            base.ActivateAutoFollow();

            if (this.DropDownItems.Count > 0)
            {
                for (int i = 0; i < DropDownItems.Count; i++)
                {

                    if (!(DropDownItems[i] is ToolStripSeparator))
                    {
                        ((DialogToolStripMenuItem)DropDownItems[i]).ActivateAutoFollow();

                    }
                }          
            }

        }

        /*Should be called on root item to deactivate autofollow on all contained items*/
        public override void DeactivateAutoFollow()
        {
            base.DeactivateAutoFollow();

            if (this.DropDownItems.Count > 0)
            {
                for (int i = 0; i < DropDownItems.Count; i++)
                {

                    if (!(DropDownItems[i] is ToolStripSeparator))
                    {
                        ((DialogToolStripMenuItem)DropDownItems[i]).DeactivateAutoFollow();

                    }
                }
            }
        }
    }

        #endregion
}
