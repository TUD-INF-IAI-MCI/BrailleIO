//Autor:    Stephanie Schöne
// TU Dresden, Germany

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tud.mci.LanguageLocalization;

namespace BrailleIO.Dialogs
{
    public class EditField_DialogEntry : SelfRenderingDialogEntry
    {

        #region Members

        private IEditField_InputManager _inputManager;

        /// <summary>
        /// Gets or sets the input manager.
        /// </summary>
        /// <value>
        /// The input manager.
        /// </value>
        public IEditField_InputManager InputManager
        {
            get {
                if (_inputManager == null) 
                    _inputManager = new EditField_InputManager(this.Title);
                
                return _inputManager; }
            set { _inputManager = value; }
        }

        private EditField_InputBox _inputBox;

        /// <summary>
        /// The input box
        /// </summary>
        public EditField_InputBox InputBox
        {
            get
            {
                if(_inputBox == null) 
                    _inputBox = new EditField_InputBox(MinimizeTypes.Unknown, BoxHeightTypes.Unknown);
                return _inputBox;
            }
            set { _inputBox = value; }
        }

        private IEditField_EventManager _eventManager;

        /// <summary>
        /// Gets or sets the event manager.
        /// </summary>
        /// <value>
        /// The event manager.
        /// </value>
        public IEditField_EventManager EventManager
        {
            get
            {
                if (_eventManager == null)
                    _eventManager = new EditField_EventManager();

                return _eventManager;
            }
            set
            {
                if (value != null && value is IEditField_EventManager)
                {
                    _eventManager = value;
                }
            }
        }

        private IEditField_Validator _validator;

        /// <summary>
        /// Gets or sets the validator.
        /// </summary>
        /// <value>
        /// The validator.
        /// </value>
        public IEditField_Validator Validator
        {
            get
            {
                if (_validator == null)
                {
                    _validator = new EditField_Validator();
                    if (!validate(Title))
                    {
                        EventManager.fire_ValidationError(Validator.LastError, this);
                        Title = "";
                    }
                }
                return _validator;
            }
            set
            {
                if (value != null && value is IEditField_Validator)
                {
                    _validator = value;
                    if (!validate(Title))
                    {

                        Title = "";
                    }
                }
            }
        }


        #region Label Members

        private Boolean _hasLabel;


        /// <summary>
        /// Gets or sets a value indicating whether this instance has a topic label.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has a topic label; otherwise, <c>false</c>.
        /// </value>
        public Boolean HasLabel
        {
            get
            {
                return _hasLabel;
            }
            set
            {
                if (value && Label != null)
                {
                    _hasLabel = value;
                }
                else _hasLabel = false;
            }
        }

        private String _label;

        /// <summary>
        /// Gets or sets the edit field topic label.
        /// </summary>
        /// <value>
        /// The edit field topic label.
        /// </value>
        public String Label
        {
            get
            {
                if (InputBox.IsGraphical)
                    if (_label == null)
                    {
                        return "";
                    }
                    else
                    {
                        return _label;
                    }
                else
                    if (_label == null)
                    {
                        return "ef:";
                    }

                    else
                    {
                        return "ef " + _label + ":";
                    }
            }
            set
            {
                if (value != null)
                {
                    _label = value;
                    HasLabel = true;
                }
                else
                {
                    HasLabel = false;
                }
            }
        }

        #endregion 

        #region Member Overrides

        /// <summary>
        /// Gets or sets the (control)type of this entry.
        /// </summary>
        /// <value>
        /// The (control)type.
        /// Can only be <see cref="DialogEntryType.EditField"/>
        /// </value>
        override public DialogEntryType Type
        {
            get { return DialogEntryType.EditField; }
            set { }
        }

        /// <summary>
        /// Gets or sets the status of this entry.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public override DialogEntryStatus Status
        {
            get
            {
                return base.Status;
            }
            set
            {
                /*Update Minimization status and possible Mode Changes.*/
                if (Status != null && InputBox != null) InputBox.handleMinimization(Status, value);
                if (Status != null) handleModeChanges(value);

                if (value.HasFlag(DialogEntryStatus.Aborting))
                {
                    if (value.HasFlag(DialogEntryStatus.Selected))
                        base.Status = DialogEntryStatus.Normal | DialogEntryStatus.Selected;
                    else 
                        base.Status = DialogEntryStatus.Normal;

                }
                else if (value.HasFlag(DialogEntryStatus.Editing)){
                    if(value.HasFlag(DialogEntryStatus.Selected)){
                        base.Status = value;
                    }
                    else {
                        base.Status = DialogEntryStatus.Normal;
                    }
                }
                else base.Status = value;
            }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EditField_DialogEntry" /> class.
        /// </summary>
        /// <param name="ID">The identifier.</param>
        /// <param name="text">The text.</param>
        /// <param name="help">The help.</param>
        /// <param name="parentEntry">The parent entry.</param>
        /// <param name="parentDialog">The parent dialog.</param>
        /// <param name="status">The status.</param>
        /// <param name="minimizeType">Type of the minimize.</param>
        /// <param name="boxHeightType">Type of the box height.</param>
        /// <param name="maxTextLength">Maximum length of the text.</param>
        /// <param name="characterRestriction">The character restriction.</param>
        /// <param name="directValidation">if set to <c>true</c> field will be validated directly. if <c>false</c> only when saved. Manual validation is needed!.</param>
        public EditField_DialogEntry(
            string ID,
            string text,
            string help = "...",
            DialogEntry parentEntry = null,
            Dialog parentDialog = null,
            DialogEntryStatus status = DialogEntryStatus.Unknown,
            MinimizeTypes minimizeType = MinimizeTypes.Unknown,
            BoxHeightTypes boxHeightType = BoxHeightTypes.Unknown,
            Boolean isGraphical = true,
            int maxTextLength = 20,
            InputRestrictionTypes characterRestriction = InputRestrictionTypes.Unrestricted,
            Boolean directValidation = true
            )
            : base(ID, text, help, DialogEntryType.EditField, status, parentEntry, parentDialog)
        {

            InputBox = new EditField_InputBox(minimizeType, boxHeightType, isGraphical);

            EventManager = new EditField_EventManager();

            if (!validateForSpaces(Title))
            {
                Title = "";
            }
            InputManager = new EditField_InputManager(Title);

            Validator = new EditField_Validator(maxTextLength, characterRestriction, directValidation);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditField_DialogEntry"/> class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="minimizeType">Type of the minimize.</param>
        /// <param name="boxHeightType">Type of the box height.</param>
        /// <param name="maxTextLength">Maximum length of the text.</param>
        /// <param name="characterRestriction">The character restriction.</param>
        /// <param name="directValidation">if set to <c>true</c> field will be validated directly. if <c>false</c> only when saved. Manual validation is needed!.</param>
        public EditField_DialogEntry(
            DialogEntry entry,
            MinimizeTypes minimizeType = MinimizeTypes.Unknown,
            BoxHeightTypes boxHeightType = BoxHeightTypes.Unknown,
            Boolean isGraphical = true,
            int maxTextLength = 20,
            InputRestrictionTypes characterRestriction = InputRestrictionTypes.Unrestricted,
            Boolean directValidation = true)
            : base(entry.ID, entry.Title, entry.Help, DialogEntryType.EditField, entry.Status, entry.ParentEntry, entry.ParentDialog)
        {
             InputBox = new EditField_InputBox(minimizeType, boxHeightType, isGraphical);

            EventManager = new EditField_EventManager();

            if (!validateForSpaces(Title))
            {
                Title = "";
            }
            InputManager = new EditField_InputManager(Title);

            Validator = new EditField_Validator(maxTextLength, characterRestriction, directValidation);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditField_DialogEntry"/> class.
        /// </summary>
        /// <param name="ID">The identifier.</param>
        /// <param name="text">The text.</param>
        /// <param name="inputM">The input m.</param>
        /// <param name="eventM">The event m.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="help">The help.</param>
        /// <param name="parentEntry">The parent entry.</param>
        /// <param name="parentDialog">The parent dialog.</param>
        /// <param name="status">The status.</param>
        /// <param name="minimizeType">Type of the minimize.</param>
        /// <param name="boxHeightType">Type of the box height.</param>
        public EditField_DialogEntry(
            string ID,
            string text,
            IEditField_InputManager inputM,
            IEditField_EventManager eventM,
            IEditField_Validator validator,
            string help = "...",
            DialogEntry parentEntry = null,
            Dialog parentDialog = null,
            DialogEntryStatus status = DialogEntryStatus.Unknown,
            MinimizeTypes minimizeType = MinimizeTypes.Unknown,
            BoxHeightTypes boxHeightType = BoxHeightTypes.Unknown,
            Boolean isGraphical = true
            )
            : base(ID, text, help, DialogEntryType.EditField, status, parentEntry, parentDialog)
        {
            InputManager = inputM;

            InputBox = new EditField_InputBox(minimizeType, boxHeightType, isGraphical);

            EventManager = eventM;

            if (!validateForSpaces(Title))
            {
                Title = "";
            }

            Validator = validator;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EditField_DialogEntry" /> class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="inputM">The input m.</param>
        /// <param name="eventM">The event m.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="minimizeType">Type of the minimize.</param>
        /// <param name="boxHeightType">Type of the box height.</param>
        public EditField_DialogEntry(
            DialogEntry entry,
            IEditField_InputManager inputM,
            IEditField_EventManager eventM,
            IEditField_Validator validator,
            MinimizeTypes minimizeType = MinimizeTypes.Unknown,
            BoxHeightTypes boxHeightType = BoxHeightTypes.Unknown,
            Boolean isGraphical = true)
            : base(entry.ID, entry.Title, entry.Help, DialogEntryType.EditField, entry.Status, entry.ParentEntry, entry.ParentDialog)
        {
            InputManager = inputM;

            InputBox = new EditField_InputBox(minimizeType, boxHeightType, isGraphical);

            EventManager = eventM;

            if (!validateForSpaces(Title))
            {
                Title = "";
            }

            Validator = validator;
        }

        #endregion

        #region Public Setter

        /// <summary>
        /// Sets the edit field label. Needs to be called after this entry was added to its parent dialog.
        /// </summary>
        /// <param name="label">The label.</param>
        public void SetEditFieldLabel(String label)
        {
            if (label != null && !label.Equals(""))
            {
                Label = label;
            }
        }

        ///// <summary>
        ///// Sets the input manager.
        ///// </summary>
        ///// <param name="inputManager">The input manager.</param>
        //public void SetInputManager(IEditField_InputManager inputM)
        //{
        //    if (inputM != null)
        //    {
        //        inputManager = inputM;
        //    }
        //}

        /// <summary>
        /// Sets the validator.
        /// </summary>
        /// <param name="validator">The validator.</param>
        public void SetValidator(IEditField_Validator validator)
        {
            if (validator != null)
            {
                this.Validator = validator;
            }
        }

        /// <summary>
        /// Sets the event manager.
        /// </summary>
        /// <param name="eventM">The event m.</param>
        public void SetEventManager(IEditField_EventManager eventM)
        {
            if (eventM != null)
            {
                EventManager = eventM;
            }
        }
        #endregion

        #region Public Getter
        /// <summary>
        /// Gets the cursor position.
        /// </summary>
        /// <returns></returns>
        internal int GetCursorPosition()
        {
            if (InputManager != null)
                return InputManager.CursorPosition;
            else return 0;
        }
        #endregion

        #region Event Registration

        /// <summary>
        /// Registers for input events.
        /// </summary>
        /// <param name="d_errorEvent">The d_ edit field input error.</param>
        /// <param name="d_warningEvent">The d_ edit field input warning.</param>
        /// <param name="d_interactionEvent">The d_ edit field input interactions.</param>
        public void RegisterForInputEvents(EventHandler<InputErrorEventArgs> d_errorEvent, EventHandler<ValidationErrorEventArgs> d_warningEvent, EventHandler<InputEventArgs> d_interactionEvent, EventHandler<ModeEventArgs> d_EditFieldModeChanges)
        {
            if (d_errorEvent != null && d_warningEvent != null && d_interactionEvent != null && d_EditFieldModeChanges != null)
            {
                RegisterForInputErrorEvent(d_errorEvent);
                RegisterForValidationEvent(d_warningEvent);
                RegisterForInteractionEvent(d_interactionEvent);
                RegisterForModeChangeEvent(d_EditFieldModeChanges);
            }
        }

        private void RegisterForModeChangeEvent(EventHandler<ModeEventArgs> d_event)
        {
            if (EventManager != null && d_event != null)
                EventManager.ModeChanges += d_event;
        }

        /// <summary>
        /// Registers for error event.
        /// </summary>
        /// <param name="d_event">The d_event.</param>
        public void RegisterForInputErrorEvent(EventHandler<InputErrorEventArgs> d_event)
        {
            if (EventManager != null && d_event != null)
                EventManager.InputErrors += d_event;
        }

        /// <summary>
        /// Registers for warning event.
        /// </summary>
        /// <param name="d_event">The d_event.</param>
        public void RegisterForValidationEvent(EventHandler<ValidationErrorEventArgs> d_event)
        {
            if (EventManager != null && d_event != null)
                EventManager.ValidationErrors += d_event;
        }

        /// <summary>
        /// Registers for interaction event. (Contains Start, Abort, Save, Finish, Add, Remove, Undo, Redo, CursorMove)
        /// </summary>t.</param>
        public void RegisterForInteractionEvent(EventHandler<InputEventArgs> d_event)
        {
            if (EventManager != null && d_event != null)
                EventManager.Interactions += d_event;

        }
        #endregion

        #region Input Functions
        /// <summary>
        /// Inputs the specified input type. Input will be executed, if valid.
        /// </summary>
        /// <param name="inputType">Type of the input.</param>
        public Boolean Input(InputTypes inputType, string input = null)
        {
            Boolean inputSucceeded = true;

            if(Validator.DirectValidation){
               inputSucceeded = inputWithValidation(inputType, input);
            }
            else {
                inputSucceeded = inputWithLaterValidation(inputType, input);
            }
            return inputSucceeded;
        }


        /// <summary>
        /// Inputs the with later validation. Will be validated for TextLength and Remove Action on Position 0.
        /// </summary>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="input">The input.</param>
        private Boolean inputWithLaterValidation(InputTypes inputType, string input)
        {
            Boolean validInput = true;

            if (inputType != null && Title != null && InputBox != null && InputBox.BoxHeightType != null && InputBox.BoxHeightType != BoxHeightTypes.Unknown)
            {
                if (inputType == InputTypes.AddContent && input != null && !Validator.ValidateTextLength(Title + input))
                {
                    EventManager.fire_ValidationError(Validator.LastError, this);
                    validInput = false;
                }
                else
                {
                    //Validate for pattern and spaces upon saving, else just execute
                    if (inputType == InputTypes.Save)
                    {
                        if (validateForSpaces(Title) && validate(Title, input)) executeInput(inputType, input);
                        else
                        {
                            validInput = false;
                        }
                    }
                    else executeInput(inputType, input);
                }
            }
            return validInput;
        }

        /// <summary>
        /// Input with validation
        /// </summary>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private Boolean inputWithValidation(InputTypes inputType, string input)
        {
            Boolean validInput = true;

            if (inputType != null && Title != null && InputBox != null && InputBox.BoxHeightType != null && InputBox.BoxHeightType != BoxHeightTypes.Unknown)
            {

             if(validateWithSpaceValidation(inputType, Title, input)) validInput = executeInput(inputType, input);
                
            }

            return validInput;
        }

        //execute the input with the input manager
        private Boolean executeInput(InputTypes inputType, string input)
        {
            Boolean executed = true;

            if (inputType != null && Title != null)
            {
                string titleBackup = Title;

                string tempTitle = InputManager.HandleInput(inputType, Title, input);

                if (InputManager.ErrorOccured)
                {
                    EventManager.fire_InputError(InputManager.LastError, this);
                    executed = false;
                }
                else
                {
                    if (inputType == InputTypes.RemoveContent)
                    {
                        if (GetCursorPosition() > 0)
                        {
                            input = Title[GetCursorPosition() - 1].ToString();
                        }
                        else executed = false;
                    }
                    else if (inputType == InputTypes.RemoveContentAfter)
                    {
                        if (Title.Length > (GetCursorPosition() + 1))
                        {
                            input = Title[GetCursorPosition()].ToString();
                        }
                        else
                        {
                            executed = false;
                        }
                    }
                      
                    Title = tempTitle;
                    EventManager.fire_Interaction(inputType, Title, input);
                    if (!Title.Equals(titleBackup)) fire_DialogEntryChanged();
                }     
            }

            return executed;
        }

        #endregion

        #region Mode Handling
        /*Seperate function to handle mode changes depending on prior and current status: Abort, Start, Finish*/
        private void handleModeChanges(DialogEntryStatus status)
        {
            if (Status != null && status != null && InputBox != null && InputBox.BoxHeightType != null && InputBox.BoxHeightType != BoxHeightTypes.Unknown)
            {
                if (!Status.HasFlag(DialogEntryStatus.Editing) && status.HasFlag(DialogEntryStatus.Selected) && status.HasFlag(DialogEntryStatus.Editing)) modeInput(ModeTypes.Start);
                else if (status.HasFlag(DialogEntryStatus.Aborting)) { modeInput(ModeTypes.Abort); }
                else if (Status.HasFlag(DialogEntryStatus.Editing) && Status.HasFlag(DialogEntryStatus.Selected) && !status.HasFlag(DialogEntryStatus.Selected) && !status.HasFlag(DialogEntryStatus.Editing)) modeInput(ModeTypes.Abort);
                else if (Status.HasFlag(DialogEntryStatus.Editing) && Status.HasFlag(DialogEntryStatus.Selected) && status.HasFlag(DialogEntryStatus.Selected) && !status.HasFlag(DialogEntryStatus.Editing))
                {
                    modeInput(ModeTypes.Finish);
                }
            }
        }

        /*handles internal mode changes and throws event*/
        private void modeInput(ModeTypes modeType)
        {
            if (modeType != null && Title != null && InputBox.BoxHeightType != null && InputBox.BoxHeightType != BoxHeightTypes.Unknown)
            {
                Title = InputManager.HandleModeChange(modeType, Title);
                EventManager.fire_ModeChange(modeType, Title);
            }
        }
        #endregion

        #region Validation

        private bool validateWithSpaceValidation(InputTypes inputType, string Title, string input)
        {
            Boolean valid = true;

            switch (inputType)
            {
                case InputTypes.AddContent:
                    valid = validateForSpaces_AddContent(Title, input) && validate(Title, input); 
                    break;
                case InputTypes.RemoveContent:
                    valid = validateForSpaces_RemoveContent(Title);
                    break;
                case InputTypes.RemoveContentAfter:
                    valid = validateForSpaces_RemoveContentAfter(Title);
                    break;
                case InputTypes.Save:
                    break;
                default:
                    break;
            }
            return valid;
        }


        /// <summary>
        /// Validates the specified title.
        /// </summary>
        /// <param name="Title">The title.</param>
        /// <returns></returns>
        private bool validate(string Title, String input = "")
        {
            Boolean valid = true;
            if (Title != null)
            {
                if (!Validator.ValidateText(Title + input) || !Validator.ValidateTextLength(Title))
                {
                    EventManager.fire_ValidationError(Validator.LastError, this);
                    valid = false;
                }

            }
            else valid = false;

            return valid;
        }


        /*checks if too many spaces were used. Currently not allowed due to rendering problems: Space in beginning, more than one following spaces.*/
        #region Space Validation

        //checks whole Title for double spaces and leading space
        private Boolean validateForSpaces(string Title)
        {
            if (Title != null && Title.Length > 0)
            {
                //Check for leading space
                if (Title.ElementAt(0).ToString().Equals(" "))
                {
                    EventManager.fire_ValidationError(new ValidationError { Title = Title, ErrorCode = 11, Description = "Leading white space ignored" }, this);
                    return false;
                }
                    //Check for double spaces
                else
                {
                    Boolean foundSpace = false;

                    for (int i = 0; i < Title.Length; i++)
                    {
                        String part = Title.ElementAt(i).ToString();

                        if (!foundSpace && part.Equals(" "))
                        {
                            foundSpace = true;
                        }
                        else if (foundSpace && part.Equals(" "))
                        {
                            EventManager.fire_ValidationError(new ValidationError { Title = Title, ErrorCode = 10, Description = "Double Space error" }, this);
                            return false;
                        }
                        else foundSpace = false;
                    }
                }
            }

            return true;
        }

        //Check for Spaces in case of adding new content
        private bool validateForSpaces_AddContent(string Title, string input)
        {
            Boolean valid = true;
            if (Title != null && input != null)
            {
                //Space on pos 0 not allowed, since it will not be rendered anyway
                if ((Title.Length == 0 || GetCursorPosition() == 0) && (input.Equals(String.Empty) || input.Equals(" ")))
                {
                    EventManager.fire_ValidationError(new ValidationError { Title = Title, ErrorCode = 10, Description = "Double Space error" }, this);
                    valid = false;
                }
                else if (Title.Length > 0 && (input.Equals(String.Empty) || input.Equals(" ")))
                {
                    //Check if space is after cursor
                    if (Title.Length > GetCursorPosition() && Title.ElementAt(GetCursorPosition()) != null)
                    {
                        String part = Title.ElementAt(GetCursorPosition()).ToString();
                        if (part.Equals(" "))
                        {
                            EventManager.fire_ValidationError(new ValidationError { Title = Title, ErrorCode = 10, Description = "Double Space error" }, this);
                            return false;
                        }
                    }
                    //Check if space is before cursor
                    if (GetCursorPosition() - 1 > 0 && Title.Length >= GetCursorPosition() - 1 && Title.ElementAt(GetCursorPosition() - 1) != null)
                    {
                        String part = Title.ElementAt(GetCursorPosition() - 1).ToString();
                        if (part.Equals(" "))
                        {
                            EventManager.fire_ValidationError(new ValidationError { Title = Title, ErrorCode = 10, Description = "Double Space error" }, this);
                            return false;
                        }
                    }

                    return true;
                }
            }
            else valid = false;

            return valid;
        }


        //Check if removing content will cause two spaces to move together
        private bool validateForSpaces_RemoveContent(string Title)
        {
            if (GetCursorPosition() > 1 && GetCursorPosition() < Title.Length)
            {
                if (Title.ElementAt(GetCursorPosition() ) != null && Title.ElementAt(GetCursorPosition() - 2) != null)
                {
                    String part = Title.ElementAt(GetCursorPosition() - 2).ToString();
                    String part2 = Title.ElementAt(GetCursorPosition()).ToString();

                    if (part.Equals(" ") && part2.Equals(" "))
                    {
                        EventManager.fire_ValidationError(new ValidationError { Title = Title, ErrorCode = 10, Description = "Double Space error" }, this);
                        return false;
                    }
                }
            }
            return true;
        }

        //Check if removing content will cause two spaces to move together
        private bool validateForSpaces_RemoveContentAfter(string Title)
        {
            if (GetCursorPosition() > 0 && GetCursorPosition() < Title.Length)
            {
                if (Title.ElementAt(GetCursorPosition() + 1) != null && Title.ElementAt(GetCursorPosition() -1) != null)
                {
                    String part = Title.ElementAt(GetCursorPosition() - 1).ToString();
                    String part2 = Title.ElementAt(GetCursorPosition() + 1).ToString();

                    if (part.Equals(" ") && part2.Equals(" "))
                    {
                        EventManager.fire_ValidationError(new ValidationError { Title = Title, ErrorCode = 10, Description = "Double Space error" }, this);
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        /// <summary>
        /// Validates the specified title. For Spaces, Maximum Text Length and Pattern.
        /// </summary>
        /// <param name="Title">The title.</param>
        /// <returns></returns>
        public Boolean Validate()
        {
            return validate(Title, "") && validateForSpaces(Title);
        }
        #endregion
    }

    #region Enum

    public enum InputTypes
    {
        Unknown,

        AddContent,
        RemoveContent,
        RemoveContentAfter,
        ClearContent,
        UndoContent,
        RedoContent,

        MoveCursorRight,
        MoveCursorLeft,
        MoveCursorToBeginning,
        MoveCursorToEnd,
        CursorJumpRight,
        CursorJumpLeft,
        Save
    }

    public enum ModeTypes
    {
        /*Field needs to have Status: Editing, Selected. Activate Field.*/
        Start,
        /*Field needs to have Status: Normal, Selected (optional). Leave the field OR State Change (Aborting)*/
        Abort,
        /*Field needs to have Status: Normal, Selected. Deactivate Field.*/
        Finish
    }
    /*
    public enum InputErrorTypes
    {
        Unknown,
        OnInit_InvalidTextLength,
        OnInit_InvalidTextCharacter,
        InvalidTextLength,
        InvalidTextCharacter
    }


    public enum InputWarningTypes
    {
        Unknown,
        TextShortened,
        TextDeleted,
        TextRedone,
        NoCharacterLeft,
        NoUndoLeft,
        NoRedoLeft,
        Undo_LimitReached,
        Redo_LimitReached,
        UnsavedChanges_Lost,
        NoChangesDetected,
        CursorCanNotMoveFurtherBack,
        CursorCanNotMoveFurther,
        CouldNotBeSaved,
        DoubleWhiteSpaceError,
        LeadingWhiteSpaceIgnored,
        CursorCanNotJumpFurther,
        CursorCanNotJumpFurtherBack
    }*/

    #endregion

}
