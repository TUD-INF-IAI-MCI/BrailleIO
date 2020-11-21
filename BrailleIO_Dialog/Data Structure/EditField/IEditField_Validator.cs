//Autor:    Stephanie Schöne
// TU Dresden, Germany

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Dialogs
{
    public interface IEditField_Validator
    {

        /// <summary>
        /// Gets or sets the last error.
        /// </summary>
        /// <value>
        /// The last error.
        /// </value>
        ValidationError LastError { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if field will be validated directly on input.
        /// Recommended for more complex input fields like e-mail address.
        /// </summary>
        /// <value>
        ///   <c>true</c> if field will be validated directly.
        ///   If set to <c>false</c>: Will only be validated on save. Before finish, needs to be manually validated!
        /// </value>
        Boolean DirectValidation { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of the text.
        /// </summary>
        /// <value>
        /// The maximum length of the text.
        /// </value>
        int MaxTextLength { get; set; }
        
        /// <summary>
        /// Validates the length of the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>bool</returns>
        bool ValidateTextLength(String text);

        /// <summary>
        /// Validates the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>bool</returns>
        bool ValidateText(String text);

    }

    public struct ValidationError
    {
        /*Content after Input (with newest input in last place for direct validation)*/
        public string Title;

        public int ErrorCode;

        /*Character or string that causes error*/
        public string ErrorPosition;

        public string Description;
    }
}
