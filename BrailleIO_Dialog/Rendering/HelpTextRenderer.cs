using BrailleIO.Interface;
using BrailleIO.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using tud.mci.LanguageLocalization;

namespace BrailleIO.Dialogs.Rendering
{
    /// <summary>
    /// Renderer for transforming the help text of <see cref="DialogEntry"/> into a bool matrix. 
    /// </summary>
    /// <seealso cref="BrailleIO.Interface.IBrailleIOContentRenderer" />
    /// <seealso cref="System.IDisposable" />
    public class HelpTextRenderer : IBrailleIOContentRenderer
    {

        #region Member

        #region private

        static LL ll = new LL(BrailleIO.Dialogs.Properties.Resources.Language);

        static readonly bool[,] emptyMatrix = new bool[0, 0] { };

        bool[,] _lastRendereinresult = emptyMatrix;
        Size lastViewSize = new Size();
        String lastHelp = String.Empty;
        String lastTitle = String.Empty;

        #endregion

        #region public

        private HelpRenderingProperties _properties;
        /// <summary>
        /// Gets or sets the properties controlling the rendering.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public HelpRenderingProperties Properties
        {
            get { return _properties; }
            set
            {
                if (_properties != value)
                    lastHelp = String.Empty;
                _properties = value;
            }
        }

        static MatrixBrailleRenderer _brailleRenderer;
        /// <summary>
        /// Gets or sets the braille renderer.
        /// </summary>
        /// <value>
        /// The braille renderer.
        /// </value>
        public static MatrixBrailleRenderer BrailleRenderer
        {
            get
            {
                if (_brailleRenderer == null) _brailleRenderer = new MatrixBrailleRenderer(RenderingProperties.IGNORE_LAST_LINESPACE | RenderingProperties.RETURN_REAL_WIDTH);
                return _brailleRenderer;
            }
            set { _brailleRenderer = value; }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpTextRenderer" /> class.
        /// </summary>
        /// <param name="properties">The properties to control the rendering.</param>
        /// <param name="brailleRenderer">The braille renderer to use.</param>
        public HelpTextRenderer(HelpRenderingProperties properties = HelpRenderingProperties.ShowHaeder,
            MatrixBrailleRenderer brailleRenderer = null)
        {
            _properties = properties;
            _brailleRenderer = brailleRenderer;
        }

        #endregion


        #region IBrailleIOContentRenderer

        /// <summary>
        /// Renders a content object into an boolean matrix;
        /// while <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// </summary>
        /// <param name="view">The frame to render in. This gives access to the space to render and other parameters. Normally this is a IBrailleIOViewRange.</param>
        /// <param name="content">The content to render.</param>
        /// <returns>
        /// A two dimensional boolean M x N matrix (bool[M,N]) where M is the count of rows (this is height)
        /// and N is the count of columns (which is the width).
        /// Positions in the Matrix are of type [i,j]
        /// while i is the index of the row (is the y position)
        /// and j is the index of the column (is the x position).
        /// In the matrix <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// </returns>
        public bool[,] RenderMatrix(IViewBoxModel view, object content)
        {

            if (view != null && content != null && content is DialogEntry && BrailleRenderer != null)
            {
                if (((DialogEntry)content).Help.Equals(lastHelp)
                    && ((DialogEntry)content).Title.Equals(lastTitle))
                {
                    Size currentSize = view.ContentBox.Size;
                    if (currentSize.Equals(lastViewSize))
                    {
                        if (!_lastRendereinresult.Equals(emptyMatrix))
                        {
                            view.ContentHeight = _lastRendereinresult.GetLength(0);
                            view.ContentWidth = _lastRendereinresult.GetLength(1);

                            return _lastRendereinresult;
                        }
                    }
                    else
                    {
                        lastViewSize = currentSize;
                    }
                }
                else
                {
                    lastHelp = ((DialogEntry)content).Help;
                    lastTitle = ((DialogEntry)content).Title;
                }

                string text = lastHelp;
                // Add header
                if (Properties.HasFlag(HelpRenderingProperties.ShowHaeder))
                    text = ll.GetTrans("help.header", lastTitle) + "\r\n" + text;

                _lastRendereinresult = BrailleRenderer.RenderMatrix(view, text);
            }

            view.ContentHeight = _lastRendereinresult.GetLength(0);
            view.ContentWidth = _lastRendereinresult.GetLength(1);
            return _lastRendereinresult;
        }

        #endregion

    }

    /// <summary>
    /// Properties defining 
    /// </summary>
    [Flags]
    public enum HelpRenderingProperties : uint
    {
        /// <summary>
        /// Normal rendering: purely display the help text.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Displays an additional header line
        /// </summary>
        ShowHaeder = 1
    }

}
