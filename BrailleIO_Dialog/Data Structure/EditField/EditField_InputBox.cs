//Autor:    Stephanie Schöne
// TU Dresden, Germany

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Dialogs
{
    public class EditField_InputBox
    {
        #region Members

        private Boolean _isGraphical;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is graphical (with border box) or textual (ef::).
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is graphical; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsGraphical
        {
            get
            {
                return _isGraphical;
            }
            set
            {
                _isGraphical = value;
            }
        }

        private Boolean _isMinimized;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is minimized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is minimized; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsMinimized
        {
            get
            {
                return _isMinimized;
            }
            set
            {
                switch (MinimizeType)
                {
                    case MinimizeTypes.AlwaysMinimize:
                        if (value) _isMinimized = value;
                        break;
                    case MinimizeTypes.AutoActivationMinimize:
                    case MinimizeTypes.AutoSelectionMinimize:
                        _isMinimized = value;
                        break;
                    case MinimizeTypes.NeverMinimize:
                        if (!value) _isMinimized = value;
                        break;
                    case MinimizeTypes.Unknown:
                    default:
                        break;
                }

            }
        }

        private MinimizeTypes _minimizeType;


        /// <summary>
        /// Gets or sets how the edit field should be minimized.
        /// </summary>
        /// <value>
        /// The type of the minimize.
        /// </value>
        public MinimizeTypes MinimizeType
        {
            get
            {
                return _minimizeType;
            }
            set
            {
                if (_minimizeType == null|| _minimizeType == MinimizeTypes.Unknown)
                {
                    _minimizeType = value;

                    if (MinimizeType == MinimizeTypes.AlwaysMinimize)
                    {
                        IsMinimized = true;
                    }
                    else if (MinimizeType == MinimizeTypes.NeverMinimize)
                    {
                        IsMinimized = false;
                    }
                    else
                    {
                        IsMinimized = true;
                    }
                }            
            }
        }


        private BoxHeightTypes _boxHeightType;

        /// <summary>
        /// Gets or sets the type of the input box (how many lines it will have).
        /// </summary>
        /// <value>
        /// The type of the input box.
        /// </value>
        public BoxHeightTypes BoxHeightType
        {
            get
            {
                return _boxHeightType;
            }
            set
            {
                if(_boxHeightType == null || _boxHeightType == BoxHeightTypes.Unknown)
                 _boxHeightType = value;
            }
        }
        #endregion

        #region Construtor

        /// <summary>
        /// Initializes a new instance of the <see cref="EditField_InputBox"/> class.
        /// </summary>
        /// <param name="minimizeType">Type of the minimize.</param>
        /// <param name="boxHeightType">Type of the box height.</param>
        public EditField_InputBox(MinimizeTypes minimizeType, BoxHeightTypes boxHeightType, Boolean isGraphical = true)
        {
            BoxHeightType = boxHeightType;
            MinimizeType = minimizeType;
            IsGraphical = isGraphical;
        }
        #endregion


        /// <summary>
        /// Handles the minimization. Depending on new and current status.
        /// </summary>
        /// <param name="oldStatus">The old status.</param>
        /// <param name="newStatus">The new status.</param>
        internal void handleMinimization(DialogEntryStatus oldStatus, DialogEntryStatus newStatus)
        {
            if (oldStatus != null && newStatus != null && MinimizeType != null && IsMinimized != null)
            {
                if (MinimizeType == MinimizeTypes.AutoSelectionMinimize)
                {
                    //Not selected, will be selected
                    if (!oldStatus.HasFlag(DialogEntryStatus.Selected) && newStatus.HasFlag(DialogEntryStatus.Selected))
                    {
                        if (MinimizeType == MinimizeTypes.AutoSelectionMinimize && IsMinimized)
                        {
                            IsMinimized = false;
                        }
                    }
                    //Selected, will be unselected
                    else if (oldStatus.HasFlag(DialogEntryStatus.Selected) && !newStatus.HasFlag(DialogEntryStatus.Selected))
                    {
                        if (MinimizeType != MinimizeTypes.NeverMinimize && !IsMinimized)
                        {
                            IsMinimized = true;
                        }
                    }
                }
                else if (MinimizeType == MinimizeTypes.AutoActivationMinimize)
                {
                    //Not editing, will be editing
                    if (!oldStatus.HasFlag(DialogEntryStatus.Editing) && newStatus.HasFlag(DialogEntryStatus.Editing))
                    {
                        if (MinimizeType == MinimizeTypes.AutoActivationMinimize && IsMinimized)
                        {
                            IsMinimized = false;
                        }
                    }
                    //Editing, will be not editing
                    else if (oldStatus.HasFlag(DialogEntryStatus.Editing) && !newStatus.HasFlag(DialogEntryStatus.Editing))
                    {
                        if (MinimizeType == MinimizeTypes.AutoActivationMinimize && !IsMinimized)
                        {
                            IsMinimized = true;
                        }
                    }
                }
            } 
        }
    }


    #region Enums

    public enum MinimizeTypes
    {
        Unknown,
        /*Never minimize the edit field*/
        NeverMinimize,
        /*automatically minimize and maximize the edit field upon (de-)selection*/
        AutoSelectionMinimize,
        /*automatically minimize and maximize the edit field upon (de-)activation*/
        AutoActivationMinimize,
        /*never maximize edit field*/
        AlwaysMinimize
    }

    public enum BoxHeightTypes
    {
        Unknown,
        /*The EditField consists of one single Line*/
        SingleLine,
        /*The EditField consists of multiple Lines, but only as much as needed to show all allowed characters*/
        TextLength,
        /*The EditField adapts its amount of lines to the text length.*/
        MaximumTextLength
    }

    #endregion
}
