using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace BrailleIO
{
    public enum BrailleIO_Languages
    {
        //only german supported yet. 
        German = 1,
        English = 2,
        Russian = 3,
        Norsk = 4,
        Swedish = 5,
        Danish = 6
    }

    public class BrailleIO_Lang
    {
        private OrderedDictionary braille_matrix = new OrderedDictionary();
        public readonly OrderedDictionary langs = new OrderedDictionary();
        public BrailleIO_Lang()
        {
            // utf-8 braille charset
            load();
        }

        private void parseFile(string file, ref OrderedDictionary r)
        {
            //attempt to use liblouis failed. Liblouis delivered false translations.
            string[] lines = System.IO.File.ReadAllLines(@file, new UTF8Encoding());
            for (int lnr = 0; lnr < lines.Length; lnr++)
            {
                string line = lines[lnr];
                Match isComment = Regex.Match(line, @"^//", RegexOptions.IgnoreCase);
                Match isTranslation = Regex.Match(line, @"\s[\d]*", RegexOptions.IgnoreCase);
                // Here we check the Match instance.

                //line...
                if (!isComment.Success && isTranslation.Success) // is a translation
                {
                    string[] trans = line.Split(' ');
                    r.Add(trans[0], dotsToUTF8(trans[1]));
                }
                else // is a comment line do nothing
                { }

            }
        }


        private string dotsToUTF8(string sequence)
        {

            string r = "";

            string[] subsequences = sequence.Split('-');

            for (int s = 0; s < subsequences.Length; s++)
            {
                int hexnum = 10240; // braille charset offset in UTF
                for (int i = 0; i < subsequences[s].Length; i++)
                {
                    hexnum += (int)Math.Pow(2, (double.Parse(subsequences[s][i].ToString()) - 1));
                }
                r += ((char)hexnum).ToString();
            }
            return r;
        }

        private void load()
        {
            string path = "";
            string[] path_segments = System.Reflection.Assembly.GetEntryAssembly().Location.Split('\\');
            for (int i = 0; i < path_segments.GetLength(0) - 1; i++)
            {
                path += path_segments[i] + "\\";
            }
            string langs_init = path + "Dictionaries\\langs.txt";
            if (System.IO.File.Exists(langs_init))
            {
                string[] lines = System.IO.File.ReadAllLines(@langs_init, new UTF8Encoding());
                for (int lnr = 0; lnr < lines.Length; lnr++)
                {
                    OrderedDictionary r = new OrderedDictionary();
                    string dict = lines[lnr];
                    parseFile("Dictionaries\\" + dict + ".txt", ref r);
                    this.langs.Add(dict, r);
                }
            }
        }


    }
}
