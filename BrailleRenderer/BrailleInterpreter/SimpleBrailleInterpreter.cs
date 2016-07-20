using System;
using System.Collections.Generic;

namespace BrailleIO.Renderer.BrailleInterpreter
{
    /// <summary>
    /// This is a basic Braille renderer. It converts a given Unicode sign into an dot pattern.
    /// The translation is defined in simple translation table files. default the Eurobraille table with
    /// German letters is loaded.
    /// </summary>
    /// <seealso cref="BrailleIO.Renderer.BrailleInterpreter.IBraileInterpreter" />
    public class SimpleBrailleInterpreter : IBraileInterpreter
    {
        #region Members

        readonly CtiFileLoader fl = new CtiFileLoader();
        private Dictionary<char, List<int>> charToDotsList = new Dictionary<char, List<int>>();
        private Dictionary<List<int>, char> dotsToCharList = new Dictionary<List<int>, char>();

        #endregion

        #region Constructor


        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleBrailleInterpreter"/> class.
        /// The BrailleInteroreter can translate strings into Braille dot pattern.
        /// </summary>
        /// <param name="tablePath">The table path, that should be used to translate.</param>
        public SimpleBrailleInterpreter(String tablePath = "")
        {
            loadBasicUnicodeMap();

            if (String.IsNullOrWhiteSpace(tablePath))
            {
                loadBasicGermanComputerBrailleMap();
            }
            else
            {
                try
                {
                    fl.LoadFile(tablePath);
                }
                catch { }
            }
            mapCtiDictionariesToLocalDictionaries();
        }

        #endregion

        #region Initalization

        /// <summary>
        /// Loads the basic unicode braille map.
        /// </summary>
        /// <returns><c>true</c> if the unicode Braille mapping  table could be loadead 
        /// successfully from the Resources. Otherwise <c>false</c>.</returns>
        private bool loadBasicUnicodeMap()
        {
            try
            {
                if (fl != null)
                {
                    // load the basic unicode braille chars
                    bool baseSuccess = fl.LoadFile(Properties.Resources.unicode, true);
                    if (baseSuccess)
                    {
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }


        private bool loadBasicGermanComputerBrailleMap()
        {
            try
            {
                if (fl != null)
                {
                    // load the basic German braille chars
                    bool baseSuccess = fl.LoadFile(Properties.Resources.de_chardefs8, true);
                    // load the basic number braille chars
                    bool digitSuccess = fl.LoadFile(Properties.Resources.digits6DotsPlusDot6, true);
                }
            }
            catch { }
            return false;
        }


        private void mapCtiDictionariesToLocalDictionaries()
        {
            if (fl != null)
            {
                Dictionary<String, String> cdList = fl.CharToDotsList;
                Dictionary<String, String> dcList = fl.DotsToCharList;

                // do the charToDotList
                foreach (var maping in cdList)
                {
                    if (maping.Key.Length > 0)
                    {
                        List<int> dots = ConvertDotPatternStringToIntList(maping.Value);
                        char kc = maping.Key[0];

                        if (charToDotsList.ContainsKey(kc))
                        {
                            charToDotsList[kc] = dots;
                        }
                        else
                        {
                            charToDotsList.Add(kc, dots);
                        }
                    }
                }


                // do the DotsToCharList
                foreach (var maping in dcList)
                {
                    if (maping.Value.Length > 0)
                    {
                        List<int> dots = ConvertDotPatternStringToIntList(maping.Key);
                        char kc = maping.Value[0];

                        if (dotsToCharList.ContainsKey(dots))
                        {
                            dotsToCharList[dots] = kc;
                        }
                        else
                        {
                            dotsToCharList.Add(dots, kc);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts the dot string (e.g. 1278) to a list of integer.
        /// </summary>
        /// <param name="dotPattern">The dot pattern as String.</param>
        /// <returns>a List of integer indicating the raised dots as a position in a Braille cell.
        ///     1 4
        ///     2 5
        ///     3 6
        ///     7 8
        /// </returns>
        public static List<int> ConvertDotPatternStringToIntList(String dotPattern)
        {
            List<int> dots = new List<int>();

            if (!String.IsNullOrEmpty(dotPattern))
            {
                for (int i = 0; i < dotPattern.Length; i++)
                {
                    char c = dotPattern[i];
                    if (c != ' ')
                    {
                        int d = -1;
                        bool success = Int32.TryParse(c.ToString(), out d);
                        if (d > 0 && d < 9)
                        {
                            dots.Add(d);
                        }
                    }
                }
            }

            //dots.Sort();
            //TODO: sort?

            return dots;
        }


        #endregion

        #region IBraileInterpreter

        /// <summary>
        /// Converts a character (e.g. T) to a list of integer (e.g. 2,3,4,5,7) that
        /// indicates the positions of raised pins in a Braille cell.
        /// </summary>
        /// <param name="c">The character to interpret.</param>
        /// <returns>
        /// a List of integer indicating the raised dots as a position in a Braille cell.
        /// 1 4
        /// 2 5
        /// 3 6
        /// 7 8
        /// </returns>
        public List<int> GetDotsFromChar(char c)
        {
            if (charToDotsList.ContainsKey(c))
            {
                return charToDotsList[c];
            }
            return new List<int>();
        }

        /// <summary>
        /// Gets the dot pattern lists from string.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>
        /// A list of interpreted characters. Each child list of this list
        /// stands for one Braille cell.
        /// The Braille cell is given as a sublist, containing a list of
        /// raised pin positions inside a Braille cell.
        /// </returns>
        public List<List<int>> GetDotsFromString(string text)
        {
            List<List<int>> result = new List<List<int>>();

            if (text.Length > 0)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    result.Add(GetDotsFromChar(text[i]));
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the char from a dot pattern. Only one-cell patterns can be interpreted.
        /// </summary>
        /// <param name="dots">The dot pattern to interpret as a list of raised pin-positions
        /// inside a Braille cell . E.g. 2,3,4,5,7 will become a 'T'</param>
        /// <returns>
        /// The correlated character to the requested dot pattern for one Braille cell.
        /// </returns>
        public char GetCharFromDots(List<int> dots)
        {
            if (dots != null)
            {
                dots.Sort();
                if (dotsToCharList.ContainsKey(dots))
                {
                    return dotsToCharList[dots];
                }
            }
            return ' ';
        }

        /// <summary>
        /// Gets the string form a list of dot patterns.
        /// Each sublist stands for one Braille cell.
        /// </summary>
        /// <param name="dots">The dot patterns to interpret.
        /// Each sublist is one Braille cell. The Sublist is a list of raised
        /// pin positions inside one Braille cell.</param>
        /// <returns>
        /// A string of interpreted Braille dot patterns.
        /// </returns>
        public string GetStringFormDots(List<List<int>> dots)
        {
            String result = String.Empty;
            if (dots != null)
            {
                foreach (var dot in dots)
                {
                    result += GetCharFromDots(dot).ToString();
                }
            }
            return result;
        }

        #endregion
    }
}
