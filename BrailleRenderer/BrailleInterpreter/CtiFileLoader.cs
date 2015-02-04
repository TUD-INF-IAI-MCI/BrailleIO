using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace BrailleRenderer.BrailleInterpreter
{
    /// <summary>
    /// Class for loading an interpreting braille translation tables 
    /// based on the definitions of the 'liblouis' project [https://github.com/liblouis]. 
    /// </summary>
    class CtiFileLoader
    {
        private Dictionary<String, String> charToDotsList = new Dictionary<String, String>();
        private Dictionary<String, String> dotsToCharList = new Dictionary<String, String>();

        /// <summary>
        /// Gets the char to dots list. A dictionary which contains a mapping from chars to a 
        /// dot pattern as a sorted string of raised Braille dots e.g. '1245'.
        /// The key is the char to translate e.g. 'g', the value is the corresponding Braille dot pattern e.g. '1245'.
        /// </summary>
        /// <value>The char to dots list.</value>
        public Dictionary<String, String> CharToDotsList { get { return charToDotsList; } }
        /// <summary>
        /// Gets the dots to char list. A dictionary which contains a mapping from dot pattern 
        /// as a sorted string of raised Braille dots e.g. '1245' to a character
        /// The key is the  Braille dot pattern e.g. '1245' and the value is the corresponding character e.g. 'g'.
        /// </summary>
        /// <value>The dots to char list.</value>
        public Dictionary<String, String> DotsToCharList { get { return dotsToCharList; } }

        private String parentPath = "";

        /// <summary>
        /// Loads a Braille translation table file. 
        /// Based on the translation definitions of the 'liblouis' project [https://github.com/liblouis]
        /// You can load as much files as you want. 
        /// Double mappings of dot pattern will be overwritten by the last loaded definition.
        /// </summary>
        /// <param name="path">The path to the translation table file to load.</param>
        /// <returns><c>true</c> if the file could be loaded and translated into mapping dictonaries.</returns>
        public bool LoadFile(String path, bool suppressWarnings = false)
        {
            if (File.Exists(path))
            {
                FileInfo fi = new FileInfo(path);
                parentPath = fi.Directory.FullName;

                string line;

                // Read the file and display it line by line.
                using (System.IO.StreamReader file = new System.IO.StreamReader(path))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        if (String.IsNullOrWhiteSpace(line)) continue;
                        line = line.TrimStart();
                        if (line.StartsWith("#")) continue;

                        splitLine(line, suppressWarnings);
                    }
                    file.Close();
                }
            }
            else
            {
                if (!suppressWarnings)
                    throw new ArgumentException("Table file '" + path + "' does not exist!");
            }
            return true;
        }


        /// <summary>
        /// Loads a Braille translation table file.
        /// Based on the translation definitions of the 'liblouis' project [https://github.com/liblouis]
        /// You can load as much files as you want.
        /// Double mappings of dot pattern will be overwritten by the last loaded definition.
        /// </summary>
        /// <param name="table">The translation table file as byte array e.g. when the file is loades from the Recources.</param>
        /// <returns>
        /// 	<c>true</c> if the file could be loaded and translated into mapping dictonaries.
        /// </returns>
        public bool LoadFile(byte[] table, bool suppressWarnings = false)
        {
            if (table != null && table.Length > 0)
            {
                string line;

                // Read the file and display it line by line.
                using (System.IO.StreamReader file = new System.IO.StreamReader(new MemoryStream(table)))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        if (String.IsNullOrWhiteSpace(line)) continue;
                        line = line.TrimStart();
                        if (line.StartsWith("#")) continue;

                        splitLine(line, suppressWarnings);
                    }
                    file.Close();
                }
            }
            else
            {
                if (!suppressWarnings)
                    throw new ArgumentException("Table file Stream is not valid");
            }
            return true;
        }



        private void splitLine(String line, bool suppressWarning)
        {
            if (!String.IsNullOrWhiteSpace(line))
            {
                string pattern = @"\s+";            // Split on hyphens
                string[] parts = Regex.Split(line, pattern);
                //System.Diagnostics.Debug.WriteLine("\tparts: " + string.Join(" | ", parts));
                if (parts.Length > 1)
                {
                    SignType sType;
                    try
                    {
                        sType = (SignType)Enum.Parse(typeof(SignType), parts[0]);
                        if (!(Enum.IsDefined(typeof(SignType), sType) | sType.ToString().Contains(",")))
                            sType = SignType.none;
                    }
                    catch (ArgumentException)
                    {
                        sType = SignType.none;
                    }

                    if (sType == SignType.none || sType == SignType.space) return;
                    if (sType == SignType.include)
                    {
                        try
                        {
                            LoadFile(parentPath + "\\" + parts[1], suppressWarning);
                        }
                        catch (Exception ex)
                        {
                            if (!suppressWarning) throw ex;

                        }
                    }
                    else
                    {

                        if (parts.Length > 2)
                        {
                            if (parts[1].StartsWith(@"\") && parts[1].Length > 2) // get unicode Hex definitions but leave the backslash as a char
                            {
                                parts[1] = GetCharFromUnicodeHex(parts[1]);
                            }


                            try
                            {
                                if (CharToDotsList.ContainsKey(parts[1])) { CharToDotsList[parts[1]] = parts[2]; }
                                else { charToDotsList.Add(parts[1], parts[2]); }

                                if (DotsToCharList.ContainsKey(parts[2])) { dotsToCharList[parts[2]] = parts[1]; }
                                else { dotsToCharList.Add(parts[2], parts[1]); }
                            }
                            catch { }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Gets the char from unicode hexadecimal string.
        /// </summary>
        /// <param name="characterCode">The character code e.g. '\x2800'.</param>
        /// <returns>the current available unicode character if available e.g. ' '</returns>
        public static string GetCharFromUnicodeHex(String characterCode)
        {

            if (!String.IsNullOrEmpty(characterCode))
            {
                if (characterCode.StartsWith(@"\"))
                {
                    characterCode = characterCode.Substring(1);
                }
                if (characterCode.StartsWith("x"))
                {
                    characterCode = characterCode.Substring(1);
                }

                int number;
                bool success = Int32.TryParse(characterCode, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out number);

                if (success)
                {
                    return GetCharFromUnicodeInt(number);
                }
            }
            return String.Empty;
        }


        /// <summary>
        /// try to parse a char from unicode int.
        /// </summary>
        /// <param name="number">The number code e.g. 10241.</param>
        /// <returns>the char of the given value e.g. ' '</returns>
        public static string GetCharFromUnicodeInt(int number)
        {
            try
            {
                char c2 = (char)number;
                return c2.ToString();
            }
            catch { }
            return String.Empty;
        }

        /// <summary>
        /// Resets this instance and clears the internal lists.
        /// </summary>
        public void Reset()
        {
            charToDotsList.Clear();
            dotsToCharList.Clear();
            parentPath = "";
        }


        enum SignType
        {
            none,
            space,
            punctuation,
            sign,
            math,
            include,
            uppercase,
            lowercase,
            digit,
            display
        }

    }
}
