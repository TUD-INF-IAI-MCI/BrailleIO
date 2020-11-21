//Autor:    Stephanie Schöne
// TU Dresden, Germany

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BrailleIO.Dialogs
{
    public class DialogEntry_ToolStripMenuItem : DialogToolStripMenuItem
    {

        #region Members

        private DialogEntryType _type;

        /// <summary>
        /// Gets or sets the type of the dialog item.
        /// </summary>
        /// <value>
        /// The type of the dialog item.
        /// </value>
        public DialogEntryType Type
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
        /// Initializes a new instance of the <see cref="DialogEntry_ToolStripMenuItem"/> class.
        /// </summary>
        /// <param name="dlgEntry">The dialog entry.</param>
        public DialogEntry_ToolStripMenuItem(DialogEntry dlgEntry): base(dlgEntry)
        {
            Type = dlgEntry.Type;
            this.CheckOnClick = true;
            
            //Check for Types & Status, some coloring to differentiate between types for now

            switch (Type)
            {
                case DialogEntryType.Activation:
                    break;
                case DialogEntryType.Button:
                    this.Text = "btn:: " + this.Text;
                    this.CheckOnClick = false;
                    break;
                case DialogEntryType.Checkbox:
                    this.Text = "[] " + this.Text;
                    break;
                case DialogEntryType.EditField:

                    EditField_DialogEntry efield = (EditField_DialogEntry)dlgEntry;
                    if (efield.HasLabel)
                    {
                        if (efield.InputBox != null && !efield.InputBox.IsGraphical)
                        this.Text = efield.Label + this.Text;
                        else this.Text = "ef:: " + efield.Label + this.Text;
                    }
                    else
                    {
                        this.Text = "ef:: " + this.Text;
                    }
                    this.CheckOnClick = false;
                    this.Enabled = false;
                    
                    break;
                case DialogEntryType.Group:
                    this.CheckOnClick = false;
                    this.Font = new System.Drawing.Font(this.Font, System.Drawing.FontStyle.Bold);       
                    break;
                case DialogEntryType.EditField_Label:
                    this.CheckOnClick = false;
                    this.Enabled = false;
                    this.Font = new System.Drawing.Font(this.Font, System.Drawing.FontStyle.Italic);  
                    break;
                case DialogEntryType.Label:
                    this.CheckOnClick = false;
                    this.Enabled = false;
                    break;
                case DialogEntryType.RadioButton:
                    this.Text = "() " + this.Text;
                    break;
                case DialogEntryType.Unknown:
                    break;
                default: break;

            }

            DialogEntryStatus itemStatus = dlgEntry.Status;

            if (itemStatus.HasFlag(DialogEntryStatus.Checked))
            {
                this.Checked = true;
            }

            if (itemStatus.HasFlag(DialogEntryStatus.Disabled))
            {
                this.Enabled = false;
                this.Text = "xx" + this.Text;
            }

            dlgEntry.DialogEntryChanged += d_EntryChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogEntry_ToolStripMenuItem"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="id">The identifier.</param>
        public DialogEntry_ToolStripMenuItem(string title, string id)
            : base(title, id, DialogClassType.DialogEntry)
        {
        }

        #endregion

        void d_EntryChanged(object sender, EntryEventArgs e)
        {
            // Updates Editfield contents in D2F menu
            if (e != null && e.Entry != null && e.Entry.Type!= null && 
                e.Entry.Type == DialogEntryType.EditField && e.Entry.Title != null && !e.Entry.Title.Equals(Text))
            {
                this.Text = "ef:: " + e.Entry.Title;
            }
        }
    }
}
