using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrailleIO.Interface;
using tud.mci.tangram.Braille_Renderer;
using System.IO;

namespace BrailleIO.Renderer
{
    public class BrailleIOLiblouisBrailleRenderer : IBrailleIOContentRenderer
    {
        private String cssPath, tables;
        private readonly RTBrailleRenderer renderer;
        private volatile bool render = true;

        bool[,] _m;
        readonly object mLock = new object();
        bool[,] m
        {
            get { lock (mLock) { return _m; } }
            set { lock (mLock) { _m = value; } }
        }
        volatile String lastString;
        volatile IViewBoxModel lastView;

        public BrailleIOLiblouisBrailleRenderer(String pathToLiblouis, String pathToTables, String cssPath, String tables)
        {
            this.cssPath = cssPath;
            this.tables = tables;
            renderer = (String.IsNullOrWhiteSpace(pathToLiblouis) || String.IsNullOrWhiteSpace(pathToTables)) ? new RTBrailleRenderer() : new RTBrailleRenderer(pathToLiblouis, pathToTables);
        }

        public BrailleIOLiblouisBrailleRenderer(String cssPath, String tables) : this(null, null, cssPath, tables) { }

        /// <summary>
        /// Sets the used CSS file.
        /// </summary>
        /// <param name="css">The path to the CSS file to use.</param>
        /// <returns><code>true</code> if the css path exist and the css file was changed</returns>
        public bool SetCss(String css)
        {
            if (File.Exists(css) && !this.cssPath.Equals(css))
            {
                this.cssPath = css;
                return render = true;
            }
            return false;
        }


        /// <summary>
        /// Sets the transcription tables to use.
        /// </summary>
        /// <param name="tables">The tables to use for transcription. 
        /// Comma separated string of table names only, without any 
        /// space characters between them and without the file path. 
        /// The path to the tables was set in the constructor of this renderer object</param>
        public void SetTables(String tables) { if (!this.tables.Equals(tables)) { this.tables = tables; render = true; } }

        public bool[,] renderMatrix(IViewBoxModel view, object content)
        {
            System.Diagnostics.Debug.WriteLine("---------- Braille Renderer Call");

            bool[,]_m = new bool[0, 0];

            if (view != null && renderer != null)
            {
                if (content is String)
                {
                    if (!view.Equals(lastView) || !lastString.Equals(content as String) || render)
                    {
                        System.Diagnostics.Debug.WriteLine("---------- ---- Real Renderer Call -" + cssPath + " - " + tables);

                        lastString = content as String;
                        lastView = view;
                        render = false;

                        List<bool[]> brailleLines = renderer.RenderHTMLDoc(content as String, cssPath, (uint)(Math.Max(view.ContentBox.Width - 3, 0)), tables);

                        //FIXME: only for fixing
                        //RTBrailleRendererHelper.PaintBoolMatrixToImage(brailleLines.ToArray(), @"C:\Users\Admin\Desktop\tmp\br_" + cssPath.Substring(Math.Max(cssPath.Length -5, 0)) + "_" + tables + ".bmp");


                        if (brailleLines.Count > 0 && brailleLines[0] != null)
                        {
                            m = new bool[brailleLines.Count, brailleLines[0].Length];

                            for (int y = 0; y < brailleLines.Count; y++)
                            {
                                for (int x = 0; x < brailleLines[0].Length; x++)
                                {
                                    if(m.GetLength(0) > y && m.GetLength(1) > x && brailleLines.Count > y && brailleLines[y].Length > x) 
                                        m[y, x] = brailleLines[y][x];
                                }
                            }
                        }
                    }
                    if(m != null) _m = m;
                    System.Diagnostics.Debug.WriteLine("---------------- Return new Matrix of " + _m.GetLength(0) + " lines");
                }
            }
            view.ContentHeight = _m.GetLength(0);
            view.ContentWidth = _m.GetLength(1);
            return _m;
        }
    }
}
