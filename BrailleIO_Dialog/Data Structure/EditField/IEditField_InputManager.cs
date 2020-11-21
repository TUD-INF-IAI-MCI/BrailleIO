//Autor:    Stephanie Schöne
// TU Dresden, Germany

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Dialogs
{
    public interface IEditField_InputManager
    {

        /// <summary>
        /// Gets or sets a value indicating whether [error occured].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [error occured]; otherwise, <c>false</c>.
        /// </value>
        Boolean ErrorOccured { get; set; }

        /// <summary>
        /// Gets or sets the last error.
        /// </summary>
        /// <value>
        /// The last error.
        /// </value>
        InputError LastError { get; set; }

        /// <summary>
        /// Gets or sets the cursor position.
        /// </summary>
        /// <value>
        /// The cursor position.
        /// </value>
        int CursorPosition { get; set; }

        /// <summary>
        /// Handles the input.
        /// </summary>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="Title">The title.</param>
        /// <param name="AddString">The add string.</param>
        /// <returns></returns>
        string HandleInput(InputTypes inputType, string Title, string AddString = null);

        /// <summary>
        /// Handles the mode change.
        /// </summary>
        /// <param name="modeType">Type of the mode.</param>
        /// <param name="Title">The title.</param>
        /// <returns></returns>
        string HandleModeChange(ModeTypes modeType, string Title);
    }

    public struct InputError
    {
        public string Title;

        public int ErrorCode;

        public string Description;
    }
}
