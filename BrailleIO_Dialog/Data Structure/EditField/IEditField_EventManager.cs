//Autor:    Stephanie Schöne
// TU Dresden, Germany

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Dialogs
{
    public interface IEditField_EventManager
    {
        event EventHandler<ModeEventArgs> ModeChanges;

        event EventHandler<InputEventArgs> Interactions;

        event EventHandler<InputErrorEventArgs> InputErrors;

        event EventHandler<ValidationErrorEventArgs> ValidationErrors;

        void fire_ModeChange(ModeTypes modeType, string Title);

        void fire_Interaction(InputTypes type, string title, string optional = "");

        void fire_ValidationError(ValidationError error, EditField_DialogEntry entry);

        void fire_InputError(InputError error, EditField_DialogEntry entry);
    }

    #region Event Args
    public class ModeEventArgs : EventArgs
    {
        public readonly ModeTypes Type;
        public readonly string Title;

        public ModeEventArgs(ModeTypes type, string title = "")
        {
            this.Type = type;
            this.Title = title;
        }

    }

    public class InputEventArgs : EventArgs
    {
        public readonly InputTypes Type;
        public readonly string Title;
        public readonly string Optional;

        public InputEventArgs(InputTypes type, string title = "", string optional = "")
        {
            this.Type = type;
            this.Title = title;
            this.Optional = optional;
        }

    }

    public class InputErrorEventArgs : EventArgs
    {
        public readonly EditField_DialogEntry Entry;
        public readonly string Title;
        public readonly int ErrorCode;
        public readonly string Description;

        public InputErrorEventArgs(InputError error, EditField_DialogEntry entry)
        {
            this.Entry = entry;
            this.Title = error.Title;
            this.ErrorCode = error.ErrorCode;
            this.Description = error.Description;
        }

    }

    public class ValidationErrorEventArgs : EventArgs
    {
        public readonly EditField_DialogEntry Entry;
        /*Content after Input (with newest input in last place for direct validation)*/
        public readonly string Title;
        public readonly int ErrorCode;
        /*Position that causes error.*/
        public readonly string ErrorPosition;
        public readonly string Description;

        public ValidationErrorEventArgs(ValidationError error, EditField_DialogEntry entry)
        {
            this.Entry = entry;
            this.Title = error.Title;
            this.ErrorCode = error.ErrorCode;
            this.ErrorPosition = error.ErrorPosition;
            this.Description = error.Description;
        }
    }

    #endregion
}
