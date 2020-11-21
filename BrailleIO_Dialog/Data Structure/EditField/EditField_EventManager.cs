//Autor:    Stephanie Schöne
// TU Dresden, Germany

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Dialogs
{
    public class EditField_EventManager : IEditField_EventManager
    {

        #region Event Members

        public event EventHandler<ModeEventArgs> ModeChanges;

        public event EventHandler<ModeEventArgs> ModeStarted;

        public event EventHandler<ModeEventArgs> ModeAborted;

        public event EventHandler<ModeEventArgs> ModeFinished;


        public event EventHandler<InputErrorEventArgs> InputErrors;

        public event EventHandler<ValidationErrorEventArgs> ValidationErrors;


        public event EventHandler<InputEventArgs> Interactions;

        public event EventHandler<InputEventArgs> InputSaved;


        public event EventHandler<InputEventArgs> ContentAdded;

        public event EventHandler<InputEventArgs> ContentRemoved;

        public event EventHandler<InputEventArgs> ContentCleared;

        public event EventHandler<InputEventArgs> ContentUndone;

        public event EventHandler<InputEventArgs> ContentRedone;

        public event EventHandler<InputEventArgs> CursorMoved;

        #endregion

        #region Event Throwing

        #region Modes

        public void fire_ModeChange(ModeTypes type, string title)
        {
            if (ModeChanges != null && type != null && title != null)
            {
                logModeChange(type, title);

                try
                {
                    ModeChanges.Invoke(this, new ModeEventArgs(type, title));
                }
                catch { }

                switch (type)
                {
                    case ModeTypes.Start:
                        fire_ModeStarted(type, title);
                        break;
                    case ModeTypes.Abort:
                        fire_ModeAborted(type, title);
                        break;
                    case ModeTypes.Finish:
                        fire_ModeFinished(type, title);
                        break;
                    default:
                        break;
                }
            }

        }


        private void fire_ModeStarted(ModeTypes type, string title)
        {
            if (ModeStarted != null)
            {
                try
                {
                    ModeStarted.Invoke(this, new ModeEventArgs(type, title));
                }
                catch { }
            }
        }


        private void fire_ModeAborted(ModeTypes type, string title)
        {
            if (ModeAborted != null)
            {
                try
                {
                    ModeAborted.Invoke(this, new ModeEventArgs(type, title));
                }
                catch { }
            }
        }

        private void fire_ModeFinished(ModeTypes type, string title)
        {
            if (ModeFinished != null)
            {
                try
                {
                    ModeFinished.Invoke(this, new ModeEventArgs(type, title));
                }
                catch { }
            }
        }

        #endregion

        #region Error & Warning

        public void fire_InputError(InputError error, EditField_DialogEntry entry)
        {
   
                logError(error);

                if (InputErrors != null)
                {
                    try
                    {
                        InputErrors.Invoke(this, new InputErrorEventArgs(error, entry));
                    }
                    catch { }
                }
            
        }

        public void fire_ValidationError(ValidationError error, EditField_DialogEntry entry)
        {

                logWarning(error);

                if (ValidationErrors != null)
                {
                    try
                    {
                        ValidationErrors.Invoke(this, new ValidationErrorEventArgs(error, entry));
                    }
                    catch { }
                }
            
         
        }

        #endregion

        #region Interactions

        public void fire_Interaction(InputTypes type, string title = "", string optional = "")
        {
            if (type != null && title != null)
            {
                logInteraction(type, title, optional);
                if (Interactions != null)
                {
                    try
                    {
                        Interactions.Invoke(this, new InputEventArgs(type, title, optional));
                    }
                    catch { }

                    switch (type)
                    {
                        case InputTypes.Unknown:
                            break;
                        case InputTypes.Save:
                            fire_InputSaved(type, title);
                            break;
                        case InputTypes.AddContent:
                            fire_ContentAdded(type, title, optional);
                            break;
                        case InputTypes.RemoveContent:
                            fire_ContentRemoved(type, title, optional);
                            break;
                        case InputTypes.ClearContent:
                            fire_ContentCleared(type, title, optional);
                            break;
                        case InputTypes.UndoContent:
                            fire_ContentUndone(type, title, optional);
                            break;
                        case InputTypes.RedoContent:
                            fire_ContentRedone(type, title, optional);
                            break;
                        case InputTypes.MoveCursorRight:
                            fire_CursorMoved(type, title);
                            break;
                        case InputTypes.MoveCursorLeft:
                            fire_CursorMoved(type, title);
                            break;
                        case InputTypes.MoveCursorToBeginning:
                            fire_CursorMoved(type, title);
                            break;
                        case InputTypes.MoveCursorToEnd:
                            fire_CursorMoved(type, title);
                            break;
                        default:
                            break;
                    }
                }
            }       
        }

        private void fire_InputSaved(InputTypes type, string title)
        {
            if (InputSaved != null)
            {
                try
                {
                    InputSaved.Invoke(this, new InputEventArgs(type, title));
                }
                catch { }
            }
        }

        private void fire_ContentAdded(InputTypes type, string title, string input)
        {
            if (ContentAdded != null)
            {
                try
                {
                    ContentAdded.Invoke(this, new InputEventArgs(type, title, input));
                }
                catch { }
            }
        }

        private void fire_ContentRemoved(InputTypes type, string title, string input)
        {
            if (ContentRemoved != null)
            {
                try
                {
                    ContentRemoved.Invoke(this, new InputEventArgs(type, title, input));
                }
                catch { }
            }
        }

        private void fire_ContentCleared(InputTypes type, string title, string input)
        {
            if (ContentCleared != null)
            {
                try
                {
                    ContentCleared.Invoke(this, new InputEventArgs(type, title, input));
                }
                catch { }
            }
        }


        private void fire_ContentUndone(InputTypes type, string title, string optional)
        {
            if (ContentUndone != null)
            {
                try
                {
                    ContentUndone.Invoke(this, new InputEventArgs(type, title, optional));
                }
                catch { }
            }
        }


        private void fire_ContentRedone(InputTypes type, string title, string optional)
        {
            if (ContentRedone != null)
            {
                try
                {
                    ContentRedone.Invoke(this, new InputEventArgs(type, title, optional));
                }
                catch { }
            }
        }

        private void fire_CursorMoved(InputTypes type, String title)
        {
            if (CursorMoved != null)
            {
                try
                {
                    CursorMoved.Invoke(this, new InputEventArgs(type));
                }
                catch { }
            }
        }

        #endregion

        #endregion

        #region Event Registering
        /// <summary>
        /// Registers for mode type event.
        /// </summary>
        /// <param name="d_event">The d_event.</param>
        /// <param name="type">The type.</param>
        public void RegisterForModeTypeEvent(EventHandler<ModeEventArgs> d_event, ModeTypes type)
        {
            if (d_event != null && type != null)
            {
                switch (type)
                {
                    case ModeTypes.Start:
                        ModeStarted += d_event;
                        break;
                    case ModeTypes.Abort:
                        ModeAborted += d_event;
                        break;
                    case ModeTypes.Finish:
                        ModeFinished += d_event;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Registers for input type event.
        /// </summary>
        /// <param name="d_event">The d_event.</param>
        /// <param name="type">The type.</param>
        public void RegisterForInputTypeEvent(EventHandler<InputEventArgs> d_event, InputTypes type)
        {
            //BETTER WITH SOME SORT OF LIST
            if (d_event != null && type != null)
            {
                switch (type)
                {
                    case InputTypes.Unknown:
                        break;
                    case InputTypes.AddContent:
                        ContentAdded += d_event;
                        break;
                    case InputTypes.RemoveContent:
                        ContentRemoved += d_event;
                        break;
                    case InputTypes.UndoContent:
                        ContentUndone += d_event;
                        break;
                    case InputTypes.RedoContent:
                        ContentRedone += d_event;
                        break;
                    case InputTypes.MoveCursorRight:
                    case InputTypes.MoveCursorLeft:
                    case InputTypes.MoveCursorToBeginning:
                    case InputTypes.MoveCursorToEnd:
                        CursorMoved += d_event;
                        break;
                    case InputTypes.Save:
                        InputSaved += d_event;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region LOGGER

        private void logModeChange(ModeTypes type, string Title)
        {
            if (type != null && Title != null)
            {
                Console.WriteLine("EditField_EventManager InputTypes:" + type.ToString() + " Text is: " + Title);
            }
        }

        internal void logInteraction(InputTypes type, string Title, string optional = "")
        {
            if (type != null && Title != null)
            {
                if (optional == null || optional.Equals(String.Empty))
                    Console.WriteLine("EditField_EventManager InputTypes:" + type.ToString() + " Current: " + Title);
                else
                    Console.WriteLine("EditField_EventManager InputTypes:" + type.ToString() + " Current: " + Title + " Additional Info: " + optional);
            }
        }

        internal void logError(InputError error)
        {
             Console.WriteLine("EditField_EventManager InputError:" + error.ToString());
                /*
                if (shouldBe == null || shouldBe.Equals(String.Empty))
                    Console.WriteLine("EditField_EventManager InputError:" + error.ToString());
                else if (!shouldBe.Equals(String.Empty) && (inputIs == null || inputIs.Equals(String.Empty)))
                    Console.WriteLine("EditField_EventManager InputError:" + error.ToString() + " Should be: " + shouldBe);
                else
                    Console.WriteLine("EditField_EventManager InputError:" + error.ToString() + " Should be: " + shouldBe + " Is: " + inputIs);*/
            
        }

        internal void logWarning(ValidationError error)
        {
            Console.WriteLine("EditField_EventManager ValidationError:" + error.ToString());
        }
        #endregion

    }


}
