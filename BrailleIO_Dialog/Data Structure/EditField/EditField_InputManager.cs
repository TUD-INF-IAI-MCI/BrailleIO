//Autor:    Stephanie Schöne
// TU Dresden, Germany

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tud.mci.LanguageLocalization;

namespace BrailleIO.Dialogs
{
    public class EditField_InputManager : IEditField_InputManager
    {

        #region Members

        #region Constants
        private const int UNDO_STORAGE_MAX = 10;
        private const int REDO_STORAGE_MAX = 10;
        private const int CURSOR_JUMP_DISTANCE = 5;

        private const int ERRORCODE_NOINPUTACCEPTED = -2;
        private const int ERRORCODE_NULLERROR = -1;
        private const int ERRORCODE_NOERROR = 0;

        private const int ERRORCODE_UNDOLIMITREACHED = 1;
        private const int ERRORCODE_REDOLIMITREACHED = 2;

        private const int ERRORCODE_NOCHARLEFT = 3;
        private const int ERRORCODE_NOCHARRIGHT = 4;
        private const int ERRORCODE_ALREADYEMPTY = 5;

        private const int ERRORCODE_REACHEDENDLEFT = 6;
        private const int ERRORCODE_REACHEDENDRIGHT = 7;
        #endregion

        private Boolean acceptsNewInput;

        private static LL ll = new LL(BrailleIO.Dialogs.Properties.Resources.Language);

        private Boolean _errorOccured;

        /// <summary>
        /// Gets or sets a value indicating whether [error occured].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [error occured]; otherwise, <c>false</c>.
        /// </value>
        public Boolean ErrorOccured
        {
            get
            {
                return _errorOccured;
            }
            set
            {
                _errorOccured = value;
            }
        }

        private InputError _lastError;

        /// <summary>
        /// Gets or sets the last error.
        /// </summary>
        /// <value>
        /// The last error.
        /// <value>-1</value> - no input accepted
        /// <value>0</value> - no error
        /// <value>1</value> - null error
        /// <value>2</value> - undo limit reached
        /// <value>3</value> - redo limit reached
        /// <value>4</value> - remove error: No characters left to remove on left side
        /// <value>5</value> - remove error: No characters left to remove on right side
        /// <value>6</value> - cursor can not move further to right
        /// <value>7</value> - cursor can not move further to left
        /// <value>8</value> - content already empty
        /// </value>
        public InputError LastError
        {
            get
            {
                return _lastError;
            }
            set
            {
                _lastError = value;
            }
        }

        #region Backup Storages

        private int _undoStorageMax;

        /// <summary>
        /// Gets or sets the undo storage maximum capacity.
        /// </summary>
        /// <value>
        /// The undo storage maximum.
        /// </value>
        public int UndoStorageMax
        {
            get
            {
                if (_undoStorageMax != null)
                    return _undoStorageMax;
                else return UNDO_STORAGE_MAX;
            }
            set
            {
                if (value > 0)
                    _undoStorageMax = value;
            }
        }

        private int _redoStorageMax;

        /// <summary>
        /// Gets or sets the redo storage maximum capacity.
        /// </summary>
        /// <value>
        /// The redo storage maximum.
        /// </value>
        public int RedoStorageMax
        {
            get
            {
                if (_redoStorageMax != null)
                    return _redoStorageMax;
                else return REDO_STORAGE_MAX;
            }
            set
            {
                if (value > 0)
                    _redoStorageMax = value;
            }
        }

        /// <summary>
        /// Stores Title states prior to accepted changes for Undo.
        /// </summary>
        private List<string> undoStorage;

        /// <summary>
        /// Stores undone Title versions to for Redo.
        /// </summary>
        private List<string> redoStorage;

        /// <summary>
        /// Makes a Backup of the last valid title.
        /// </summary>
        private string titleBackup;

        #endregion

        #region CursorPosition
        private int _cursorPosition;

        /// <summary>
        /// Gets or sets the cursor position.
        /// </summary>
        /// <value>
        /// The cursor position. Can be from 0 (before first letter) to Title.length (after last letter)
        /// </value>
        public int CursorPosition
        {
            get
            {
                return _cursorPosition;
            }
            set
            {
                if (value != null && value >= 0)
                {
                    _cursorPosition = value;
                }
            }
        }


        private int _cursorJumpDistance;

        /// <summary>
        /// Gets or sets the cursor jump distance.
        /// </summary>
        /// <value>
        /// The cursor jump distance.
        /// </value>
        public int CursorJumpDistance
        {
            get
            {
                if (_cursorJumpDistance == null) _cursorJumpDistance = CursorJumpDistance;
                return _cursorJumpDistance;
            }
            set
            {
                if (value != null && value > 0)
                {
                    _cursorJumpDistance = value;
                }
            }
        }
        #endregion


        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EditField_InputManager"/> class.
        /// </summary>
        /// <param name="maxTextLength">Maximum length of the text.</param>
        /// <param name="type">The type.</param>
        /// <param name="Title">The title.</param>
        public EditField_InputManager(String Title)
        {
            undoStorage = new List<string>();
            redoStorage = new List<string>();

            UndoStorageMax = UNDO_STORAGE_MAX;
            RedoStorageMax = REDO_STORAGE_MAX;

            titleBackup = "";
            CursorPosition = 0;
            CursorJumpDistance = CURSOR_JUMP_DISTANCE;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EditField_InputManager"/> class.
        /// </summary>
        /// <param name="Title">The title.</param>
        /// <param name="undoStorageMax">The undo storage maximum.</param>
        /// <param name="redoStorageMax">The redo storage maximum.</param>
        /// <param name="cursorJumpDistance">The cursor jump distance.</param>
        public EditField_InputManager(String Title, int undoStorageMax, int redoStorageMax, int cursorJumpDistance)
        {
            undoStorage = new List<string>();
            redoStorage = new List<string>();

            UndoStorageMax = undoStorageMax;
            RedoStorageMax = redoStorageMax;

            titleBackup = "";
            CursorPosition = 0;
            CursorJumpDistance = cursorJumpDistance;
        }

        #endregion

        #region Mode Handling
        /// <summary>
        /// Handles the mode change.
        /// </summary>
        /// <param name="modeType">Type of the mode.</param>
        /// <param name="Title">The title.</param>
        /// <returns></returns>
        public string HandleModeChange(ModeTypes modeType, string Title)
        {
            if (modeType != null && Title != null)
            {
                switch (modeType)
                {
                    case ModeTypes.Start:
                        startInput(Title);
                        break;
                    case ModeTypes.Abort:
                        Title = abortInput(Title);
                        break;
                    case ModeTypes.Finish:
                        Title = finishInput(Title);
                        break;
                    default:
                        break;
                }
            }
            return Title;
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Handles the input.
        /// </summary>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="Title">The title.</param>
        /// <param name="AddString">The add string.</param>
        /// <returns></returns>
        public string HandleInput(InputTypes inputType, string Title, string AddString = null)
        {
            if (inputType != null && Title != null)
            {
                if (acceptsNewInput)
                {
                    switch (inputType)
                    {
                        case InputTypes.Unknown:
                            break;
                        case InputTypes.AddContent:
                            Title = addContent(Title, AddString);
                            break;
                        case InputTypes.RemoveContent:
                            Title = removeContent(Title);
                            break;
                        case InputTypes.RemoveContentAfter:
                            Title = removeContentAfter(Title);
                            break;
                        case InputTypes.ClearContent:
                            Title = clearContent(Title);
                            break;
                        case InputTypes.MoveCursorRight:
                            Title = moveCursorRight(Title);
                            break;
                        case InputTypes.MoveCursorLeft:
                            Title = moveCursorLeft(Title);
                            break;
                        case InputTypes.MoveCursorToBeginning:
                            moveCursorToBeginning(Title);
                            break;
                        case InputTypes.MoveCursorToEnd:
                            moveCursorToEnd(Title);
                            break;
                        case InputTypes.CursorJumpLeft:
                            jumpCursorLeft(Title);
                            break;
                        case InputTypes.CursorJumpRight:
                            jumpCursorRight(Title);
                            break;
                        case InputTypes.UndoContent:
                            Title = undoInput(Title);
                            break;
                        case InputTypes.RedoContent:
                            Title = redoInput(Title);
                            break;
                        case InputTypes.Save:
                            Title = saveInput(Title);
                            break;
                        default:
                            break;
                    }
                }
                else setError(Title, ERRORCODE_NOINPUTACCEPTED, "No Input accepted");   
            }
            return Title;
        }

        #region Start Save Abort Finish
        /// <summary>
        /// Starts the input.
        /// </summary>
        /// <param name="Title">The title.</param>
        private void startInput(string Title)
        {
            if (Title != null)
            {
                acceptsNewInput = true;
                titleBackup = Title;
                CursorPosition = Title.Length;
                setNoError(Title);
            }
            else setNullError();
        }


        /// <summary>
        /// Aborts the input.
        /// </summary>
        /// <returns></returns>
        private string abortInput(string Title)
        {
            if (Title != null)
            {
                acceptsNewInput = false;
                CursorPosition = 0;
                setNoError(Title);
                return titleBackup;
            }
            else setNullError();

            return "";
        }

        /// <summary>
        /// Saves the input value without finishing the input.
        /// </summary>
        /// <param name="Title">The title.</param>
        /// <returns></returns>
        private string saveInput(string Title)
        {
            if (Title != null)
            {
              if (!Title.Equals(titleBackup))
                    {
                        if (undoStorage.Count >= UndoStorageMax)
                        {
                            undoStorage.RemoveAt(0);
                        }
                        undoStorage.Add(titleBackup);
                        titleBackup = Title;
              }
              setNoError(Title);
            }
            else setNullError();

            return Title;
        }

        /// <summary>
        /// Finishes the input.
        /// </summary>
        /// <param name="Title">The title.</param>
        /// <returns></returns>
        private string finishInput(string Title)
        {
            if (Title != null)
            {

                    if (!Title.Equals(titleBackup))
                    {
                        if (undoStorage.Count >= UndoStorageMax)
                        {
                            undoStorage.RemoveAt(0);
                        }
                        undoStorage.Add(titleBackup);

                        acceptsNewInput = false;
                    }
                    else
                    {
                        acceptsNewInput = false;
                    }

                    CursorPosition = 0;
                    setNoError(Title);
            }
            else setNullError();


            return Title;
        }



        #endregion

        #region UNDO&REDO

        /// <summary>
        /// Undoes the input.
        /// </summary>
        private string undoInput(string Title)
        {
            if (Title != null)
            {
                string newTitle = titleBackup;

                // If current state is new and not saved yet, no need to really undo. New State just needs to be thrown away
                if (titleBackup.Equals(Title))
                {
                    if (undoStorage.Count > 0)
                    {
                        if (redoStorage.Count >= RedoStorageMax)
                        {
                            redoStorage.RemoveAt(0);
                        }

                        redoStorage.Add(Title);


                        newTitle = undoStorage.ElementAt(undoStorage.Count - 1);

                        titleBackup = newTitle;

                        undoStorage.RemoveAt(undoStorage.Count - 1);
                        setNoError(Title);
                    }
                    else
                    {
                        setError(Title, ERRORCODE_UNDOLIMITREACHED, "Undo Limit is reached. No undo left.");
                    }
                }
                else setNoError(Title);

                if(CursorPosition > newTitle.Length) moveCursorToEnd(newTitle);
                return newTitle;
            }
            else setNullError(); 
                
            return Title;

        }

        /// <summary>
        /// Redoes the input.
        /// </summary>
        private string redoInput(string Title)
        {
            if (Title != null)
            {
                String newTitle = Title;
                if (redoStorage.Count > 0)
                {
                    if (undoStorage.Count >= UndoStorageMax)
                    {
                        undoStorage.RemoveAt(0);
                    }
                    undoStorage.Add(Title);
                    newTitle = redoStorage.ElementAt(redoStorage.Count - 1);
                    titleBackup = newTitle;
                    redoStorage.RemoveAt(redoStorage.Count - 1);
                    setNoError(Title);
                }
                else
                {
                    setError(Title, ERRORCODE_REDOLIMITREACHED, "Redo Limit is reached. No redo left.");
                }

                if (CursorPosition > newTitle.Length) moveCursorToEnd(newTitle);
                return newTitle;
            }
            else setNullError();
                
            return Title;
        }
        #endregion

        #region Add Remove

        private string addContent(string Title, string input)
        {
            if (Title != null && input != null)
            {

                    char[] letters = Title.ToArray();
                    string tempTitle = "";

                    if (CursorPosition == 0) tempTitle = input + Title;
                    else if (CursorPosition == Title.Length) tempTitle = Title + input;
                    else
                    {
                        for (int i = 0; i < letters.Length; i++)
                        {
                            if (i == CursorPosition) tempTitle = tempTitle + input;
                            tempTitle = tempTitle + letters[i].ToString();
                        }
                    }

                    Title = tempTitle;
                    moveCursorRight(Title);
                
                setNoError(Title);
            }
            else setNullError();

            return Title;
        }

        private string removeContent(string Title)
        {
            if (Title != null)
            {
                string newTitle = Title;

       
                    if (Title.Length > 0)
                    {
                        if (CursorPosition > 0)
                        {
                            string toRemove = Title[CursorPosition - 1].ToString();

                            newTitle = Title.Remove(CursorPosition - 1, 1);
                            moveCursorLeft(Title);
                            setNoError(Title);
                        }
                        else
                        {
                            setError(Title, ERRORCODE_NOCHARLEFT, "On the left side are no characters left to remove.");
                        }

                    }
                    else
                    {
                        setError(Title, ERRORCODE_NOCHARLEFT, "On the left side are no characters left to remove.");
                    }
                
 
                return newTitle;
            }
            else setNullError();

            return Title;
        }

        private string removeContentAfter(string Title)
        {
            if (Title != null)
            {
                string newTitle = Title;

        
                    if (Title.Length > 0)
                    {
                        if (CursorPosition < Title.Length)
                        {
                            string toRemove = Title[CursorPosition].ToString();

                            newTitle = Title.Remove(CursorPosition, 1);
                            setNoError(Title);
                        }
                        else
                        {
                            setError(Title, ERRORCODE_NOCHARRIGHT, "On the right side are no characters left to remove.");
                        }
                    }
                    else
                    {
                        setError(Title, ERRORCODE_NOCHARRIGHT, "On the right side are no characters left to remove.");
                    }
                
                return newTitle;
            }
            else setNullError(); 
            return Title;
        }

        private string clearContent(string Title)
        {
            if (Title != null)
            {
                string newTitle = Title;

                    if (Title.Length > 0)
                    {
                        Title = "";
                        moveCursorToBeginning(Title);
                        setNoError(Title);
                    }
                    else
                    {
                        setError(Title, ERRORCODE_ALREADYEMPTY, "Content is already empty!");
                    }
                
            }
            else setNullError();
            
               return Title;
        }

        #endregion

        #endregion

        #region Cursor Handling

        /// <summary>
        /// Moves the cursor right.
        /// </summary>
        /// <returns></returns>
        private string moveCursorRight(string Title)
        {
            Boolean moved = false;
            if (Title != null)
            {
                if (CursorPosition == Title.Length)
                {
                    setError(Title, ERRORCODE_REACHEDENDRIGHT, "Cursor can not move further to the right.");
                }
                else
                {
                    CursorPosition++;
                    moved = true;
                    setNoError(Title);
                }
            }
            else setNullError();

            return Title;
        }

        /// <summary>
        /// Moves the cursor left.
        /// </summary>
        /// <returns></returns>
        private string moveCursorLeft(string Title)
        {
            Boolean moved = false;
            if (Title != null)
            {
                if (CursorPosition == 0)
                {
                    setError(Title, ERRORCODE_REACHEDENDLEFT, "Cursor can not move further to the left.");
                }
                else
                {
                    CursorPosition--;
                    moved = true;
                    setNoError(Title);
                }
            }
            else setNullError();

            return Title;
        }

        /// <summary>
        /// Moves the cursor to beginning.
        /// </summary>
        /// <returns></returns>
        private Boolean moveCursorToBeginning(string Title)
        {
            Boolean moved = false;
            if (Title != null)
            {
                CursorPosition = 0;
                moved = true;
                setNoError(Title);
            }
            else setNullError();

            return moved;
        }

        /// <summary>
        /// Moves the cursor to end.
        /// </summary>
        /// <returns></returns>
        private Boolean moveCursorToEnd(string Title)
        {
            Boolean moved = false;
            if (Title != null)
            {
                CursorPosition = Title.Length;
                moved = true;
                setNoError(Title);
            }
            else setNullError();

            return moved;
        }


        private Boolean jumpCursorRight(string Title)
        {
            Boolean moved = false;
            if (Title != null)
            {
                if (CursorPosition == Title.Length)
                {
                    setError(Title, ERRORCODE_REACHEDENDRIGHT, "Cursor can not move further to the right.");
                }
                else if (CursorPosition + CursorJumpDistance > Title.Length)
                {
                    CursorPosition = Title.Length;
                    moved = true;
                    setNoError(Title);
                }
                else
                {
                    CursorPosition = CursorPosition + CursorJumpDistance;
                    moved = true;
                    setNoError(Title);
                }
            }
            else setNullError();

            return moved;
        }

        private Boolean jumpCursorLeft(string Title)
        {
            Boolean moved = false;
            if (Title != null)
            {
                if (CursorPosition == 0)
                {
                    setError(Title, ERRORCODE_REACHEDENDLEFT, "Cursor can not move further to the left.");
                }
                else if (CursorPosition - CursorJumpDistance < 0)
                {
                    CursorPosition = 0;
                    moved = true;
                    setNoError(Title);
                }
                else
                {
                    CursorPosition = CursorPosition - CursorJumpDistance;
                    moved = true;
                    setNoError(Title);
                }
            }
            else setNullError();

            return moved;
        }
        #endregion

        #region Setter functions

        /// <summary>
        /// Sets the undo storage maximum capacity.
        /// </summary>
        /// <param name="max">The maximum.</param>
        public void SetUndoStorageMax(int max)
        {
            if (max != null && max > 0)
            {
                UndoStorageMax = max;
            }
        }

        /// <summary>
        /// Sets the redo storage maximum capacity.
        /// </summary>
        /// <param name="max">The maximum.</param>
        public void SetRedoStorageMax(int max)
        {
            if (max != null && max > 0)
            {
                RedoStorageMax = max;
            }
        }

        /// <summary>
        /// Sets the cursor jump distance.
        /// </summary>
        /// <param name="distance">The distance.</param>
        public void SetCursorJumpDistance(int distance)
        {
            if (distance != null && distance > 0)
            {
                CursorJumpDistance = distance;
            }
        }
        #endregion

        #region lastError

        private void setNoError(String title)
        {
            LastError = new InputError
            {
                Title = title,
                ErrorCode = ERRORCODE_NOERROR,
                Description = "No Error occured"
            };
            ErrorOccured = false;
        }

        private void setNullError()
        {
            LastError = new InputError
            {
                Title = "",
                ErrorCode = ERRORCODE_NULLERROR,
                Description = "NullError"
            };
            ErrorOccured = true;
        }

        private void setError(String title, int errorCode, String description)
        {
            LastError = new InputError
            {
                Title = title,
                ErrorCode = errorCode,
                Description = description
            };
            ErrorOccured = true;
        }

        #endregion

    }
}
