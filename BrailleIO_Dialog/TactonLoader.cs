using BrailleIO.Interface;
using BrailleIO.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BrailleIO.Dialogs
{
    /// <summary>
    /// Static helper class for loading external/user defined tactons.
    /// </summary>
    public class TactonLoader
    {
        #region Members

        static IBrailleIOContentRenderer _brailleRenderer = new MatrixBrailleRenderer(RenderingProperties.IGNORE_LAST_LINESPACE | RenderingProperties.RETURN_REAL_WIDTH);
        /// <summary>
        /// Gets or sets the braille renderer for transforming strings to a tactile matrix.
        /// </summary>
        /// <value>
        /// The braille renderer.
        /// </value>
        public static IBrailleIOContentRenderer BrailleRenderer
        {
            get { return _brailleRenderer; }
            set { _brailleRenderer = value; }
        }

        static IBrailleIOContentRenderer _imgRenderer = new BrailleIO.Renderer.BrailleIOImageToMatrixRenderer();
        /// <summary>
        /// Gets or sets the image to tactile matrix renderer for transforming pictures to tactons.
        /// </summary>
        /// <value>
        /// The image renderer.
        /// </value>
        public static IBrailleIOContentRenderer ImageRenderer
        {
            get { return TactonLoader._imgRenderer; }
            set { TactonLoader._imgRenderer = value; }
        }

        private static IViewBoxModel dummyView = new BrailleIOViewRange(0, 0, int.MaxValue - 100, int.MaxValue);
        /// <summary>
        /// Gets or sets the dummy view for Braille text rendering.
        /// </summary>
        /// <value>
        /// The dummy view.
        /// </value>
        public static IViewBoxModel DummyView
        {
            get { return dummyView; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("The dummy view range for rendering cannot be NULL!");
                dummyView = value;
            }
        }

        #endregion

        #region TEXT

        /// <summary>
        /// Loads the tactons from a text. The several tacton definitions have to be separated by a line break.
        /// One tacton is defined as key:tacton.
        /// Attention: Do not use space characters after the divider ':' because the will be turned into the tacton representation as well!
        /// </summary>
        /// <param name="tactonDefs">The text containing the tacton definitions.</param>
        /// <returns>Dictionary of tactons with key and their tactile matrix representation.</returns>
        /// <remarks>For converting the text to a tactile representation the static available Braille renderer of this class is used.</remarks>
        public static Dictionary<String, bool[,]> LoadTactonsFromText(String tactonDefs)
        {
            Dictionary<String, bool[,]> dict = new Dictionary<string, bool[,]>();

            if (!String.IsNullOrWhiteSpace(tactonDefs) && BrailleRenderer != null)
            {
                // get all separated definitions
                var lines = tactonDefs.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
                if (lines != null && lines.Length > 0)
                {
                    foreach (var line in lines)
                    {
                        if (!String.IsNullOrWhiteSpace(line))
                        {
                            // find divider
                            int deviderPos = line.IndexOf(":");
                            if (deviderPos > 0)
                            {
                                String key = line.Substring(0, deviderPos).Trim();
                                bool[,] tacton = new bool[0, 0];
                                if (!String.IsNullOrWhiteSpace(key))
                                {
                                    String def = String.Empty;
                                    if (deviderPos < line.Length - 2)
                                    {
                                        def = line.Substring(deviderPos + 1);
                                    }

                                    // convert the definition into tactile matrix
                                    tacton = BrailleRenderer.RenderMatrix(DummyView, def);

                                    // add to the dictionary
                                    if (dict.ContainsKey(key)) { dict[key] = tacton; }
                                    else { dict.Add(key, tacton); }
                                }
                            }
                        }
                    }
                }
            }

            return dict;
        }

        /// <summary>
        /// Loads the tactons from a text file. The several tacton definitions have to be separated by a line break.
        /// One tacton is defined as key:tacton.
        /// Attention: Do not use space characters after the divider ':' because the will be turned into the tacton representation as well!
        /// </summary>
        /// <param name="filePath">The file path to the definition file.</param>
        /// <returns>
        /// Dictionary of tactons with key and their tactile matrix representation.
        /// </returns>
        /// <remarks>
        /// For converting the text to a tactile representation the static available Braille renderer of this class is used.
        /// </remarks>
        public static Dictionary<String, bool[,]> LoadTactonsFromTextFile(String filePath)
        {
            if (String.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return new Dictionary<string, bool[,]>();

            String defs = File.ReadAllText(filePath);
            return LoadTactonsFromText(defs);

        }

        #endregion

        #region IMAGE

        /// <summary>
        /// Loads a tacton from image.
        /// For transcoding an image to a tactile matrix, the static available ImageRender is used.
        /// </summary>
        /// <param name="img">The image to transcode into a tactile matrix.</param>
        /// <returns>the tactile matrix representation of the tacton.</returns>
        /// <remarks>By default, dark pixels of the given image will be turned into raised pins; light pixels 
        /// into lowered pins.</remarks>
        public static bool[,] LoadTactonFromImage(Image img)
        {
            bool[,] tacton = new bool[0, 0];

            if (img != null && ImageRenderer != null)
            {
                tacton = ImageRenderer.RenderMatrix(buildImageDummyViewRange(img), img);
            }

            return tacton;
        }

        /// <summary>
        /// Loads the tacton from file.
        /// For transcoding an image to a tactile matrix, the static available ImageRender is used.
        /// </summary>
        /// <param name="tactonPath">The file path to the tacton image file.</param>
        /// <param name="name">The name of the tacton (this is the file name without extension).</param>
        /// <param name="tacton">The tacton.</param>
        /// <returns>the tactile matrix representation of the tacton.</returns>
        /// <remarks>By default, dark pixels of the given image will be turned into raised pins; light pixels into 
        /// lowered pins.</remarks>
        public static bool LoadTactonFromImageFile(String tactonPath, out String name, out bool[,] tacton)
        {
            tacton = new bool[0, 0];
            name = String.Empty;
            bool success = false;

            if (!String.IsNullOrWhiteSpace(tactonPath) && File.Exists(tactonPath))
            {
                name = Path.GetFileNameWithoutExtension(tactonPath);
                try
                {
                    Image img = Image.FromFile(tactonPath);
                    tacton = LoadTactonFromImage(img);
                    success = true;
                }
                catch { }
            }

            return success;
        }

        /// <summary>
        /// Loads all available tacton file from one directory.
        /// For transcoding an image to a tactile matrix, the static available ImageRender is used.
        /// </summary>
        /// <param name="tactonDirPath">The path to the tacton directory.</param>
        /// <param name="loadSubDirectory">if set to <c>true</c> tactons from all subdirectories will be loaded as well.</param>
        /// <returns>
        /// Dictionary of tactons with key and their tactile matrix representation.
        /// </returns>
        /// <remarks>By default, dark pixels of the given image will be turned into raised pins; light pixels into 
        /// lowered pins.</remarks>
        public static Dictionary<String, bool[,]> LoadTactonsFromDirectory(
            String tactonDirPath,
            bool loadSubDirectory = false)
        {
            Dictionary<String, bool[,]> dict = new Dictionary<string, bool[,]>();

            if (!String.IsNullOrWhiteSpace(tactonDirPath) && Directory.Exists(tactonDirPath))
            {
                var files = Directory.GetFiles(tactonDirPath);
                if (files != null && files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        String key;
                        bool[,] tacton;

                        if (IsImageExtension(Path.GetExtension(file)) && 
                            LoadTactonFromImageFile(file, out key, out tacton)) // images
                        {
                            if (dict.ContainsKey(key)) { dict[key] = tacton; }
                            else { dict.Add(key, tacton); }
                        }
                        else // maybe text files
                        {
                            var textTactons = LoadTactonsFromTextFile(file);
                            // combine both dictionaries
                            combine2Dicts(ref dict, textTactons);
                        }
                    }
                }

                if (loadSubDirectory)
                {
                    var subDirs = Directory.GetDirectories(tactonDirPath);
                    if (subDirs != null && subDirs.Length > 0)
                    {
                        foreach (var subDir in subDirs)
                        {
                            var subTactons = LoadTactonsFromDirectory(subDir, true);
                            // combine both dictionaries
                            combine2Dicts(ref dict, subTactons);
                        }
                    }
                }
            }


            return dict;
        }

        #endregion

        #region Utils



        private static List<String> _validExtensions = new List<string>() { ".jpg", ".bmp", ".gif", ".png", ".tiff" };
        private static List<String> ValidImageFileExtensions
        {
            get
            {
                if (_validExtensions == null)
                {
                    // load from app.config, text file, DB, wherever
                }
                return _validExtensions;
            }
            set { _validExtensions = value; }
        }

        /// <summary>
        /// Determines whether the given extension string defines an accepted image file or not.
        /// </summary>
        /// <param name="ext">The file extension strin (incl. dot e.g. '.bmp').</param>
        /// <returns>
        ///   <c>true</c> if this is an accepted image extension; otherwise, <c>false</c>.
        /// </returns>
        protected static bool IsImageExtension(string ext)
        {
            return ValidImageFileExtensions.Contains(ext);
        }

        /// <summary>
        /// Adds one dictionary to another. Existing keys will be overwritten.
        /// </summary>
        /// <param name="dict">The dictionary to add to.</param>
        /// <param name="dict2add">The dictionary to add.</param>
        private static void combine2Dicts(ref Dictionary<String, bool[,]> dict, Dictionary<string, bool[,]> dict2add)
        {
            if (dict != null && dict2add != null && dict2add.Count > 0)
            {
                foreach (var subTacton in dict2add)
                {
                    if (dict.ContainsKey(subTacton.Key)) { dict[subTacton.Key] = subTacton.Value; }
                    else { dict.Add(subTacton.Key, subTacton.Value); }
                }
            }
        }

        /// <summary>
        /// Builds a dummy view range with the same size as the image.
        /// </summary>
        /// <param name="img">The image.</param>
        /// <returns>A dummy view range with the size of the image.</returns>
        private static IViewBoxModel buildImageDummyViewRange(Image img)
        {
            BrailleIOViewRange view = new BrailleIOViewRange(0, 0, 0, 0);
            if (img != null)
            {
                view.SetWidth(img.Width);
                view.SetHeight(img.Height);
            }
            return view;
        }

        #endregion
    }
}
