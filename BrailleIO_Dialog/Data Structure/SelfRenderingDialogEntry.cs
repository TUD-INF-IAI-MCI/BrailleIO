using BrailleIO.Dialogs.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Dialogs
{
    /// <summary>
    /// <see cref="DialogEntry"/> that has its own related and specialized renderer.
    /// Increase the rendering performance.
    /// </summary>
    /// <seealso cref="BrailleIO.Dialogs.DialogEntry" />
    public class SelfRenderingDialogEntry : DialogEntry
    {

        DialogEntryRenderer _renderer = new DialogEntryRenderer();
        /// <summary>
        /// Gets or sets the renderer for this specific Entry.
        /// </summary>
        /// <value>
        /// The renderer.
        /// </value>
        public DialogEntryRenderer Renderer
        {
            get { return _renderer; }
            set { 
                if(value != null)
                _renderer = value; 
            }
        }

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
        public SelfRenderingDialogEntry(
            string ID,
            string text,
            string help = "...",
            DialogEntryType type = DialogEntryType.Activation,
            DialogEntryStatus status = DialogEntryStatus.Normal,
            DialogEntry parentEntry = null,
            Dialog parentDialog = null)
            : base(ID, text, help, type, status, parentEntry, parentDialog)
        {
            Renderer.Entry = this;

            if (type == DialogEntryType.EditField) Renderer = new EditFieldRenderer();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogEntry"/> class.
        /// </summary>
        /// <param name="dialogEntry">The dialog entry.</param>
        public SelfRenderingDialogEntry(DialogEntry dialogEntry) :base(dialogEntry) {
            Renderer.Entry = this;

            if (dialogEntry.Type == DialogEntryType.EditField) Renderer = new EditFieldRenderer();
        }
    }
}
