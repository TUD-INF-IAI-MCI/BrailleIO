//Autor:    Stephanie Schöne
// TU Dresden, Germany

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tud.mci.LanguageLocalization;

namespace BrailleIO.Dialogs
{
    public class EditField_Validator: IEditField_Validator
    {

        #region Members

        #region Constants
        private const int ERRORCODE_NULLERROR = -1;
        private const int ERRORCODE_NOERROR = 0;
        private const int ERRORCODE_INALIDLENGTH = 1;
        private const int ERRORCODE_INALIDCHARACTER = 2;
        private const int ERRORCODE_INALIDPATTERN = 3;
        private const int ERRORCODE_NOINT = 4;
        private const int ERRORCODE_NODOUBLE = 5;
        #endregion

        private const int MAX_TEXT_LENGTH = 20;

        private ValidationError _lastError;

        /// <summary>
        /// Gets or sets the last error.
        /// </summary>
        /// <value>
        /// The last error.
        /// </value>
        public ValidationError LastError
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


        private readonly Boolean _directValidation;

        /// <summary>
        /// Gets or sets a value indicating if field will be validated directly on input.
        /// Recommended for more complex input fields like e-mail address.
        /// </summary>
        /// <value>
        ///   <c>true</c> if field will be validated directly.
        ///   If set to <c>false</c>: Will only be validated on save. Before finish, needs to be manually validated!
        /// </value>
        public Boolean DirectValidation
        {
            get
            {
                return _directValidation;
            }
            set
            {
            }
        }


        private System.Text.RegularExpressions.Regex regEx;

        internal string regExPattern;


        private int _maxTextLength;

        /// <summary>
        /// Gets or sets the maximum length of the text.
        /// </summary>
        /// <value>
        /// The maximum length of the text.
        /// </value>
        public int MaxTextLength
        {
            get
            {
                return _maxTextLength;
            }
            set
            {
                if (value != null)
                    _maxTextLength = value;
            }
        }

        private InputRestrictionTypes _inputRestrictionType;

        /// <summary>
        /// Gets or sets the type of the edit field (which character restrictions).
        /// </summary>
        /// <value>
        /// The type of the edit field.
        /// </value>
        public InputRestrictionTypes InputRestrictionType
        {
            get
            {
                if (_inputRestrictionType == null) return InputRestrictionTypes.Unknown;
                else return _inputRestrictionType;
            }
            set
            {
                if(value != null)
                    _inputRestrictionType = value;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EditField_Validator" /> class.
        /// </summary>
        /// <param name="directValidation">if set to <c>true</c> field will be validated directly. if <c>false</c> only when saved. Manual validation is needed!.</param>
        public EditField_Validator(Boolean directValidation = true)
        {
            MaxTextLength = MAX_TEXT_LENGTH;
            InputRestrictionType = InputRestrictionTypes.Unrestricted;
            _directValidation = directValidation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditField_Validator" /> class.
        /// </summary>
        /// <param name="maxTextLength">Maximum length of the text.</param>
        /// <param name="type">The type.</param>
        /// <param name="directValidation">if set to <c>true</c> field will be validated directly. if <c>false</c> only when saved. Manual validation is needed!.</param>
        public EditField_Validator(int maxTextLength, InputRestrictionTypes type, Boolean directValidation = true)
        {
            MaxTextLength = maxTextLength;
            this.InputRestrictionType = type;

            regExPattern = getRegExPattern(InputRestrictionType);
            regEx = new System.Text.RegularExpressions.Regex(regExPattern);
            _directValidation = directValidation;
        }
        #endregion

        #region Validations
        /// <summary>
        /// Validates the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool ValidateText(String text)
        {
            if (text != null)
                return validateTextRestrictions(text);
            else
            {
                setNullError();
                return false;
            }
        }

        /// <summary>
        /// Validates the length of the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool ValidateTextLength(String text)
        {
            if (text != null)
            {
                if (text.Length <= _maxTextLength)
                {
                    setNoError(text);
                    return true;
                }
                else
                {

                    setError(text, ERRORCODE_INALIDLENGTH, text.Substring(_maxTextLength - 1, text.Length - _maxTextLength), "Text is too long!");

                    return false;
                }
            }
            else
            {
                setNullError();
                return false;
            }
        }

        /// <summary>
        /// Checks the text based on set restrictions. If restrictions are not met, text will be empty.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        internal bool validateTextRestrictions(String text)
        {
            bool validText = false;

            if (text != null && regEx != null && !text.Equals(String.Empty))
            {
                    if (InputRestrictionType == InputRestrictionTypes.IntegerType && !text.Equals(String.Empty))
                    {
                        int value;
                        validText = Int32.TryParse(text, out value);

                        if (!validText)
                        {
                            setError(text, ERRORCODE_NOINT, text, "Value is not a valid integer.");
                        }
                    }
                    else if(InputRestrictionType == InputRestrictionTypes.DoubleType && !text.Equals(String.Empty)){
                        double value;
                        validText = Double.TryParse(text, out value);

                        if (!validText)
                        {
                            setError(text, ERRORCODE_NODOUBLE, text, "Value is not a valid double.");
                        }
                    }
                    else
                    {
                        validText = regEx.IsMatch(text);

                        if (!validText)
                        {
                            if (DirectValidation && InputRestrictionType != InputRestrictionTypes.Custom)
                            {
                                setError(text, ERRORCODE_INALIDCHARACTER, text[text.Length-1].ToString(), "Invalid character.");
                            }
                            else
                            {
                                setError(text, ERRORCODE_INALIDPATTERN, text, "Invalid value pattern.");
                            }
                        }
                    }
            }
            else if(text.Equals(String.Empty)) validText = true;

            return validText;
        }

        #endregion

        #region Helper Functions

        private static string getRegExPattern(InputRestrictionTypes EditFieldType)
        {
            String pattern = "";


            switch (EditFieldType)
            {
                case InputRestrictionTypes.Unknown:
                    break;
                case InputRestrictionTypes.Alphabet:
                    pattern = "^[A-Za-z]*$";
                    break;
                case InputRestrictionTypes.AlphabetWithWhitespace:
                    pattern = "^[ A-Za-z]*$";
                    break;
                case InputRestrictionTypes.AlphaNumeric:
                    pattern = "^[A-Za-z0-9]*$";
                    break;
                case InputRestrictionTypes.AlphaNumericWithWhitespace:
                    pattern = "^[ A-Za-z0-9]*$";
                    break;
                case InputRestrictionTypes.NumericSingle:
                    pattern = "^[0-9]?$";
                    break;
                case InputRestrictionTypes.Numeric:
                    pattern = "^[0-9]*$";
                    break;
                case InputRestrictionTypes.IntegerType:
                    pattern = "Int32Type";
                    break;
                case InputRestrictionTypes.DoubleType:
                    pattern = "DoubleType";
                    break;
                case InputRestrictionTypes.Custom:
                case InputRestrictionTypes.Unrestricted:
                default:
                    break;
            }

            return pattern;
        }

        /// <summary>
        /// Sets the reg ex pattern. For Custom InputRestrictionType only
        /// If content is preset, you should check that it is valid.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public Boolean SetRegExPattern(String pattern)
        {
            if (InputRestrictionType == InputRestrictionTypes.Custom && pattern != null)
            {
                try
                {
                    regExPattern = pattern;
                    regEx = new System.Text.RegularExpressions.Regex(pattern);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Pattern is invalid!");
                }
            }
            return false;
        }

        #endregion

        #region lastError

        private void setNoError(String title)
        {
            LastError = new ValidationError
            {
                Title = title,
                ErrorCode = ERRORCODE_NULLERROR,
                ErrorPosition = "",
                Description = "No Error occured"
            };
        }

        private void setNullError()
        {
            LastError = new ValidationError
            {
                Title = "",
                ErrorCode = ERRORCODE_NULLERROR,
                ErrorPosition = "",
                Description = "NullError"
            };
        }

        private void setError(String title, int errorCode, String errorPosition, String description)
        {
            LastError = new ValidationError
            {
                Title = title,
                ErrorCode = errorCode,
                ErrorPosition = errorPosition,
                Description = description
            };
        }
        #endregion
    }

    #region Enum
    public enum InputRestrictionTypes
    {
        /// <summary>
        /// Unrestricted input.
        /// </summary>
        Unknown,
        /// <summary>
        /// Input can be alphabet characters or empty.
        /// </summary>
        Alphabet,
        /// <summary>
        /// Input can:alphabet characters, whitespaces, empty.
        /// </summary>
        AlphabetWithWhitespace,
        /// <summary>
        /// Input can be alphabet character and/or numbers (0-9) or empty.
        /// </summary>
        AlphaNumeric,
        /// <summary>
        /// Input can:alphabet characters, numbers (0-9), whitespaces, empty.
        /// </summary>
        AlphaNumericWithWhitespace,
        /// <summary>
        /// Input can only be a single number (0-9).
        /// </summary>
        NumericSingle,
        /// <summary>
        /// Input can only be numbers (0-9) or empty. 
        /// </summary>
        Numeric,
        /// <summary>
        /// Unrestricted Input. 
        /// </summary>
        Unrestricted,
        /// <summary>
        /// Can be Int32 value or empty. Value can be signed. Ex.: +5, -100
        /// </summary>
        IntegerType,
        /// <summary>
        /// Can be Double value or empty. Value can be signed, can have exponential value.
        /// To indicate decimal place '.' is used. Ex.: -1.643e6, 1643.57
        /// </summary>
        DoubleType,
        /// <summary>
        /// Input will be matched to custom pattern. Should be a Regular Expression.
        /// </summary>
        Custom
    }

    #endregion
}
