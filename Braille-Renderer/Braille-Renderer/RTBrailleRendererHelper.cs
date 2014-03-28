using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CsQuery;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace tud.mci.tangram.Braille_Renderer
{
    public static partial class RTBrailleRendererHelper
    {

        public static string cmdCall(string executable, string args, string STDIN)
        {

            ProcessStartInfo psi = new ProcessStartInfo(executable);
            psi.UseShellExecute = false;
            psi.LoadUserProfile = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.WindowStyle = ProcessWindowStyle.Minimized;
            psi.CreateNoWindow = true;
            psi.Arguments = args;

            Process process = new Process();
            process.StartInfo = psi;

            process.Start();

            process.StandardInput.Write(STDIN);
            process.StandardInput.Close();

            string stdOut = process.StandardOutput.ReadToEnd();

            process.Close();
            //substring raus
            stdOut = stdOut.Substring(0, Math.Max(stdOut.Length - 2, 0));

            return stdOut;
        }

        public static string getSelectorFromElement(IDomObject element)
        {
            if (element != null && ! (element is IDomText))
            {
                return element.NodeName;
                //string tag = element.OuterHTML.Substring(1, 3);
                //string[] temp = tag.Split('>');
                //string selector = temp[0];
                //return selector;
            }
            return String.Empty;
        }

        public static List<bool[]> putcharinmatrix(ref List<bool[]> M, bool[,] letter, ref uint x, ref uint y)  //ref x, y
        {
            int intx = Convert.ToInt32(x);
            int inty = Convert.ToInt32(y);

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 2; j++)
                {

                    bool temp = false;
                    temp = letter[i, j];
                    
                        var line = M[inty + i];
                        if (line != null) line[intx + j] = temp;
                   

                    
                }
            }
            return M;
        }

        public static bool[,] CreateRectangularArray(List<bool[]> arrays)
        {
            // TODO: Validation and special-casing for arrays.Count == 0
            int minorLength = arrays[0].Length;
            bool[,] ret = new bool[arrays.Count, minorLength];
            for (int i = 0; i < arrays.Count; i++)
            {
                var array = arrays[i];
                if (array.Length != minorLength)
                {
                    throw new ArgumentException
                        ("All arrays must be the same length");
                }
                for (int j = 0; j < minorLength; j++)
                {
                    ret[i, j] = array[j];
                }
            }
            return ret;
        }

        public static uint ConvertUnitsHor(string value)
        {
            uint pixel;
            if (value.EndsWith("px"))
            {
                value = value.TrimEnd('p', 'x');
                pixel = Convert.ToUInt32(value);
            }
            else if (value.EndsWith("em"))
            {
                value = value.TrimEnd('e', 'm');
                pixel = Convert.ToUInt32(value) * 3;
            }
            else
            {
                UInt32.TryParse((new Regex(@"[^\d]")).Replace(value, ""), out pixel);
            }
            return pixel;
        }

        public static uint ConvertUnitsVert(string value, uint lineheight)
        {
            uint pixel;
            if (value.EndsWith("px"))
            {
                value = value.TrimEnd('p', 'x');
                pixel = Convert.ToUInt32(value);
            }
            else if (value.EndsWith("em"))
            {
                value = value.TrimEnd('e', 'm');
                pixel = Convert.ToUInt32(value) * lineheight;
            }
            else
            {
                 UInt32.TryParse((new Regex(@"[^\d]")).Replace(value, ""),out pixel);
            }
            return pixel;
        }

        public static int ConvertUnitsInt(string value)
        {
            int pixel;
            if (value.EndsWith("px"))
            {
                value = value.TrimEnd('p', 'x');
                pixel = Convert.ToInt32(value);
            }
            else if (value.EndsWith("em"))
            {
                value = value.TrimEnd('e', 'm');
                pixel = Convert.ToInt32(value) * 3;
            }
            else
            {
                Int32.TryParse((new Regex(@"[^\d]")).Replace(value, ""), out pixel);
            }
            return pixel;
        }

        public static string getUnicodeFromNumber(int number)
        {
            switch (number)
            {
                case 1:
                    return "\\x2801";
                case 2:
                    return "\\x2803";
                case 3:
                    return "\\x2809";
                case 4:
                    return "\\x2819";
                case 5:
                    return "\\x2811";
                case 6:
                    return "\\x280b";
                case 7:
                    return "\\x281b";
                case 8:
                    return "\\x2813";
                case 9:
                    return "\\x280a";
                default:
                    return "\\x2800";
            }
        }




        private static Object gLock = new Object();
        private static Pen _p = new Pen(Brushes.LightGray, 0.4F);
        private static Object pLock = new Object();

        private static Pen Stroke
        {
            get
            {
                lock (gLock)
                {
                    return _p;
                }
            }
        }
        private static Object graphicsLock = new Object();
        const int pixel = 5;

        //paints display!
        public static void PaintBoolMatrixToImage(bool[][] m, string filePath)
        {
            if (m == null || m.Length < 1 || m[0].Length < 1) return;

            Bitmap bmp = new Bitmap(m[0].Length * (pixel + 1), m.Length * (pixel + 1));
            lock (graphicsLock)
            {
                using (Graphics PinGraphic = Graphics.FromImage(bmp))
                {
                    PinGraphic.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);

                    for (int i = 0; i < m.Length; i++)
                        for (int j = 0; j < m[0].Length; j++)
                        {
                            lock (graphicsLock)
                            {
                                try
{
	if (m[i][j])
	                                {
	                                    PinGraphic.FillRectangle(Brushes.Black, j * (pixel + 1), i * (pixel + 1), pixel, pixel);
	                                }
	                                else
	                                {
	                                    PinGraphic.DrawEllipse(Stroke, j * (pixel + 1), i * (pixel + 1), pixel - 1, pixel - 1);
	                                }
}
catch (System.Exception ex)
{
	
}
                            }
                        }

                    try
                    {
                        PinGraphic.Flush();
                        if (string.IsNullOrEmpty(filePath) || string.IsNullOrWhiteSpace(filePath)) return;
                        bmp.Save(filePath, ImageFormat.Bmp);
                    }
                    catch { }
                }
            }
        }

        public static bool[] newLine(uint lenght)
        {
            return new bool[lenght];
        }

        //public static void zoomout(string htmlfile, string cssfile)
        //{
        //    switch (cssfile)
        //    {
        //        case "Zoom1.css":
        //            break;
        //        case "Zoom2.css":
        //            start(htmlfile, "Zoom1,css");
        //            break;
        //        case "Zoome3.css":
        //            start(htmlfile, "Zoom2.css");
        //            break;
        //        case "Zoom4.css":
        //            start(htmlfile, "Zoom3,css");
        //            break;
        //    }
        //}

        //public static void zoomin(string htmlfile, string cssfile)
        //{
        //    switch (cssfile)
        //    {
        //        case "Zoom1.css":
        //            start(htmlfile, "Zoom2.css");
        //            break;
        //        case "Zoom2.css":
        //            start(htmlfile, "Zoom3,css");
        //            break;
        //        case "Zoome3.css":
        //            start(htmlfile, "Zoom4.css");
        //            break;
        //        case "Zoom4.css":
        //            break;
        //    }
        //}


    }
}
