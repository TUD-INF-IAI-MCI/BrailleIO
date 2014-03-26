using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Web;
using System.IO;
using CsQuery;
using System.Diagnostics;

namespace tud.mci.tangram.Braille_Renderer
{
    //public static class Globals
    //{
    //    public static int max_X = 120;
    //    static int _currentX;
    //    object lickObj = new Object();
    //    public static int CurrentX
    //    {
    //        get
    //        {
    //            return _currentX;
    //        }
    //        set
    //        {
    //            _currentX = value;
    //        }
    //    }

    //    static int _currentY;
    //    public static int CurrentY
    //    {
    //        get
    //        {
    //            return _currentY;
    //        }
    //        set
    //        {
    //            _currentY = value;
    //        }
    //    }
    //}

    class Program
    {


        static void Main(string[] args)
        {
            
            string tables;

                Console.WriteLine("Type name of translation table: ");
                tables = Console.ReadLine();

        
        string htmlfile = "Testdokument.html";
        string cssfile = "Zoom1.css";
        string pathToLiblouis = @"C:\Users\Alexander\Documents\Bachelorarbeit\liblouis-mingw32msvc\bin\";
        string pathToTables = @"C:/Users/Alexander/Documents/Bachelorarbeit/liblouis-mingw32msvc/bin/tables/";
        uint maxwidth = 120;

        RTBrailleRenderer rtb = new RTBrailleRenderer(pathToLiblouis, pathToTables);
            
            //start(htmlfile, cssfile);

            //zoomout(htmlfile, cssfile);
            //zoomin(htmlfile, cssfile);


        }
    }

    class HandleExecutable
    {
        private DataReceivedEventHandler outputHandler;

        public DataReceivedEventHandler OutputHandler
        {
            set { outputHandler = value; }
        }
        private DataReceivedEventHandler errorHandler;

        public DataReceivedEventHandler ErrorHandler
        {
            set { errorHandler = value; }
        }

        public void callExecutable(string executable, string args) { callExecutable(executable, args, null); }
        public void callExecutable(string executable, string args, string STDIN)
        {
            string commandLine = executable;
            ProcessStartInfo psi = new ProcessStartInfo(commandLine);
            psi.UseShellExecute = false;
            psi.LoadUserProfile = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.WindowStyle = ProcessWindowStyle.Minimized;
            psi.CreateNoWindow = true;
            psi.Arguments = args;
            var p = new Process();
            p.StartInfo = psi;
            try
            {
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                if (outputHandler != null) p.OutputDataReceived += outputHandler;
                if (errorHandler != null) p.ErrorDataReceived += errorHandler;

                if (STDIN != null)
                {
                    p.StandardInput.Write(STDIN);
                }
                p.StandardInput.Close();

                p.WaitForExit();
                p.Close();
                p.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Console.WriteLine(ex.Message);
            }
        }
    }


}
