using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BrailleRenderer.BrailleInterpreter
{
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
        /// Converts the dot string (e.g. 1278) to an list of integer.
        /// </summary>
        /// <param name="dots">The dot pattern as String.</param>
        /// <returns>a List of integer indicating the raised dots as a Position in a Braille cell</returns>
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

            dots.Sort();
            //TODO: sort?
            return dots;
        }


        #endregion

        #region IBraileInterpreter

        public List<int> GetDotsFromChar(char c)
        {
            if (charToDotsList.ContainsKey(c))
            {
                return charToDotsList[c];
            }
            return new List<int>();
        }

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
