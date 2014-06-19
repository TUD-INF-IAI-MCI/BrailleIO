using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Web;
using System.IO;
using CsQuery;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace tud.mci.tangram.Braille_Renderer
{
    public partial class RTBrailleRenderer
    { //todo: rest einfügen, Pfad zu Liblouis, Pfad zu Tabellen, BReite

        public const uint DEFAULT_LINE_HEIGHT = 5;

        string htmlfile = "";
        string cssfile = "";
        static string pathToLiblouis = @"liblouis/";
        static string pathToTables = @"tables/";
        string command;
        string param;
        string tables;
        Globals coords;
        uint maxwidth;

        //Test Threadsafe
        List<bool[]> _m = new List<bool[]>(); //Niemals auf diese Liste direkt zugreifen
        readonly object _mLock = new object(); //das lock object

        /// <summary>
        /// Gets or sets the m.
        /// </summary>
        /// <value>
        /// The matrix of the display.
        /// </value>
        List<bool[]> M
        {
            get
            {
                lock (_mLock)
                {
                    if (_m == null)
                    {
                        _m = new List<bool[]>();
                    }
                    return _m;
                }
            }

            set
            {
                lock (_mLock)
                {
                    _m = value;
                }
            }
        }


        private void Underline(ref List<bool[]> M, string underlineStyle, ref uint x, ref uint y, ref bool dotBefore)
        {
            switch (underlineStyle)
            {
                case "solid":
                    for (int i = 0; i < 3; i++)
                    {
                        M[Convert.ToInt32(y) + 4][x + i] = true;
                    }
                    break;
                case "dotted":
                    if (!dotBefore)
                    {
                        M[Convert.ToInt32(y) + 4][x] = true;
                        M[Convert.ToInt32(y) + 4][x + 1] = false;
                        M[Convert.ToInt32(y) + 4][x + 2] = true;
                        dotBefore = true;
                    }
                    else
                    {
                        M[Convert.ToInt32(y) + 4][x] = false;
                        M[Convert.ToInt32(y) + 4][x + 1] = true;
                        M[Convert.ToInt32(y) + 4][x + 2] = false;
                        dotBefore = false;
                    }

                    break;
                case "dashed":
                    M[Convert.ToInt32(y) + 4][x] = true;
                    M[Convert.ToInt32(y) + 4][x + 1] = true;
                    M[Convert.ToInt32(y) + 4][x + 2] = false;
                    break;
                case "double":
                    for (int i = 0; i < 3; i++)
                    {
                        M[Convert.ToInt32(y) + 4][x + i] = true;
                        M[Convert.ToInt32(y) + 5][x + i] = true;
                    }
                    break;
            }
        }



        /// <summary>
        /// Draws the border.
        /// </summary>
        /// <param name="M">The m.</param>
        /// <param name="border">The border.</param>
        /// <param name="startx">The startx.</param>
        /// <param name="endx">The endx.</param>
        /// <param name="starty">The starty.</param>
        /// <param name="endy">The endy.</param>
        /// <returns></returns>
        List<bool[]> drawBorder(ref List<bool[]> M, BoxModel border, uint startx, uint endx, uint starty, uint endy)
        {

            for (int i = Convert.ToInt32(starty); i <= Convert.ToInt32(endy); i++)
            {
                //draw Top Line as thick as specified
                if (i < Convert.ToInt32(starty) + Convert.ToInt32(border.Top))
                {
                    for (int j = Convert.ToInt32(startx); j <= Convert.ToInt32(endx); j++)
                    {
                        M[i][j] = true;
                    }
                }

                //draw Bot Line as thick as specified
                else if (i > Convert.ToInt32(endy) - Convert.ToInt32(border.Bottom))
                {

                    for (int j = Convert.ToInt32(startx); j <= Convert.ToInt32(endx); j++)
                    {
                        M[i][j] = true;
                    }
                }

                    // Draw side lines as thick as specified
                else
                {
                    //Draw left line
                    for (int k = Convert.ToInt32(startx); k < startx + border.Left; k++)
                    {
                        M[i][k] = true;
                    }
                    //Draw right line
                    for (int l = Convert.ToInt32(endx); l > endx - border.Right; l--)
                    {
                        M[i][l] = true;
                    }
                }
            }

            return M;
        }



        //////////////////////////////////////////////////////////////////////////////////////////////
        //                                              AK                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Creates new struct 'HtmlBrailleElement' for the first time (parent node).
        /// </summary>
        /// <param name="M">The matrix of the display.</param>
        /// <param name="htmlelement">The htmlelement from the DOM.</param>
        /// <param name="rules">The css rules.</param>
        public void NewElement(ref List<bool[]> M, IDomObject htmlelement, Dictionary<String, List<String>> rules)
        {



            string selector = RTBrailleRendererHelper.getSelectorFromElement(htmlelement);

            if (!rules.ContainsKey(selector))
            {
                return;
            }
            else
            {
                List<string> declarations = rules[selector];
                INodeList nodes = (((CsQuery.Implementation.DomContainer<CsQuery.Implementation.DomElement>)(htmlelement))).ChildNodes;
                HtmlBrailleElement element = new HtmlBrailleElement();

                element.nodelist = nodes;
                element.selector = selector;
                element.before = null;
                element.after = null;
                element.lineheight = 5;
                element.width = maxwidth;


                foreach (string declaration in declarations)
                {
                    if (declaration.StartsWith("line-height"))
                    {
                        string[] feature_value = declaration.Split(':');
                        string feature = feature_value[0];
                        string value = feature_value[1].TrimStart();
                        element.lineheight = RTBrailleRendererHelper.ConvertUnitsVert(value, 1);
                    }
                }

                foreach (string declaration in declarations)
                {
                    #region before
                    if (declaration.StartsWith("before|"))
                    {
                        string[] partpseudo = declaration.Split('|');
                        string nopseudo = partpseudo[1];
                        string[] feature_value = nopseudo.Split(':');
                        string feature = feature_value[0];
                        string value = feature_value[1].TrimStart();

                        if (feature.Equals("content"))
                        {
                            value = value.Substring(1, value.Length - 2);
                            if (element.selector == "li" && RTBrailleRendererHelper.getSelectorFromElement(htmlelement.ParentNode) == "ol")
                            {
                                element.before = value + RTBrailleRendererHelper.getUnicodeFromNumber(htmlelement.Index + 1);
                            }
                            else
                            {
                                element.before = value;
                            }
                        }
                    }
                    #endregion before

                    #region afer
                    else if (declaration.StartsWith("after|"))
                    {
                        string[] partpseudo = declaration.Split('|');
                        string nopseudo = partpseudo[1];
                        string[] feature_value = nopseudo.Split(':');
                        string feature = feature_value[0];
                        string value = feature_value[1].TrimStart();

                        if (feature.Equals("content"))
                        {
                            value = value.Substring(1, value.Length - 2);
                            element.after = value;
                        }
                    }
                    #endregion after

                    else
                    {
                        if (!(declaration == null))
                        {
                            string[] feature_value = declaration.Split(':');
                            string feature = feature_value[0];
                            string value = feature_value[1].TrimStart();
                            switch (feature)
                            {
                                #region margin
                                case "margin":
                                    if (value.Contains(' '))
                                    {
                                        string[] multivalues = value.Split(' ');
                                        int size = multivalues.Length;


                                        switch (size)
                                        {
                                            case 2:
                                                BoxModel margin2 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]));
                                                element.margin = margin2;
                                                break;
                                            case 3:
                                                BoxModel margin3 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], element.lineheight));
                                                element.margin = margin3;
                                                break;
                                            case 4:
                                                BoxModel margin4 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[3]));
                                                element.margin = margin4;
                                                break;

                                        }

                                    }
                                    else
                                    {

                                        BoxModel margin1 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value), RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value));
                                        element.margin = margin1;
                                    }

                                    break;
                                #endregion margin

                                #region border-width
                                case "border-width":
                                    if (value.Contains(' ')) //Angaben müssen der Form "margin: 4px 2px 3px" sein
                                    {
                                        string[] multivalues = value.Split(' ');
                                        int size = multivalues.Length;
                                        uint[] borderpixel = new uint[size];

                                        switch (size)
                                        {
                                            case 2:
                                                BoxModel border2 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]));
                                                element.border = border2;
                                                break;
                                            case 3:
                                                BoxModel border3 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], element.lineheight));
                                                element.border = border3;
                                                break;
                                            case 4:
                                                BoxModel border4 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[3]));
                                                element.border = border4;
                                                break;

                                        }

                                    }
                                    else
                                    {
                                        BoxModel border1 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value), RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value));
                                        element.border = border1;
                                    }
                                    break;
                                #endregion border

                                #region border-left-width
                                case "border-left-width":
                                    element.border.Left = RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight);
                                    break;
                                #endregion border-left-width

                                #region border-right-width
                                case "border-right-width":
                                    element.border.Right = RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight);
                                    break;
                                #endregion border-right-width

                                #region border-top-width
                                case "border-top-width":
                                    element.border.Top = RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight);
                                    break;
                                #endregion border-top-width

                                #region border-bottom-width
                                case "border-bottom-width":
                                    element.border.Bottom = RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight);
                                    break;
                                #endregion border-bottom-width

                                #region border-bottom-style
                                case "border-bottom-style":
                                    element.border_bottom_style = value;
                                    break;
                                #endregion border-bottom-style

                                #region padding
                                case "padding":
                                    if (value.Contains(' '))
                                    {
                                        string[] multivalues = value.Split(' ');
                                        int size = multivalues.Length;

                                        switch (size)
                                        {
                                            case 2:
                                                BoxModel padding2 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]));
                                                element.padding = padding2;
                                                break;
                                            case 3:
                                                BoxModel padding3 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], element.lineheight));
                                                element.padding = padding3;
                                                break;
                                            case 4:
                                                BoxModel padding4 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[3]));
                                                element.padding = padding4;
                                                break;

                                        }

                                    }
                                    else
                                    {

                                        BoxModel padding1 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value), RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value));
                                        element.padding = padding1;
                                    }
                                    break;
                                #endregion padding

                                #region text-indent
                                case "text-indent":
                                    element.text_indent = Convert.ToInt32(RTBrailleRendererHelper.ConvertUnitsInt(value));
                                    break;
                                #endregion text-indent

                                #region height
                                case "height":
                                    element.height = RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight);
                                    break;
                                #endregion height

                                #region width
                                case "width":
                                    element.width = RTBrailleRendererHelper.ConvertUnitsHor(value);
                                    break;
                                #endregion width


                                #region display
                                case "display":
                                    if (value == "none")
                                    {
                                        element.hide = true;
                                    }
                                    else
                                    {
                                        element.hide = false;
                                    }
                                    break;
                                #endregion display

                            }
                        }

                    }
                }

                if (!element.hide)
                {
                    M = GeneralDraw(ref M, element, rules); //Call GeneralDraw for first time without parent coordinates      
                }
            }

        }


        /// <summary>
        /// Creates new struct 'HtmlBrailleElement' for child nodes).
        /// </summary>
        /// <param name="M">The matrix of the display</param>
        /// <param name="htmlelement">The htmlelement from the DOM.</param>
        /// <param name="rules">The css rules.</param>
        /// <param name="x">The current x from the parent.</param>
        /// <param name="y">The current y from the parent.</param>
        public void NewElement(ref List<bool[]> M, IDomObject htmlelement, Dictionary<String, List<String>> rules, ref uint x, ref uint y, ref uint x_max, ref uint x_min)
        {


            string selector = RTBrailleRendererHelper.getSelectorFromElement(htmlelement);

            if (!rules.ContainsKey(selector))
            {
                return;
            }
            else
            {
                List<string> declarations = rules[selector];
                HtmlBrailleElement element = new HtmlBrailleElement();
                INodeList nodes = (((CsQuery.Implementation.DomContainer<CsQuery.Implementation.DomElement>)(htmlelement))).ChildNodes;

                element.nodelist = nodes;
                element.selector = selector;
                element.before = null;
                element.after = null;
                element.lineheight = 5;
                element.width = maxwidth;

                foreach (string declaration in declarations)
                {
                    if (declaration.StartsWith("line-height"))
                    {
                        string[] feature_value = declaration.Split(':');
                        string feature = feature_value[0];
                        string value = feature_value[1].TrimStart();
                        element.lineheight = RTBrailleRendererHelper.ConvertUnitsVert(value, 1);
                    }
                }

                foreach (string declaration in declarations)
                {
                    #region before
                    if (declaration.StartsWith("before|"))
                    {
                        string[] partpseudo = declaration.Split('|');
                        string nopseudo = partpseudo[1];
                        string[] feature_value = nopseudo.Split(':');
                        string feature = feature_value[0];
                        string value = feature_value[1].TrimStart();

                        if (feature.Equals("content"))
                        {
                            value = value.Substring(1, value.Length - 2);
                            if (element.selector == "li" && RTBrailleRendererHelper.getSelectorFromElement(htmlelement.ParentNode) == "ol")
                            {
                                element.before = value + RTBrailleRendererHelper.getUnicodeFromNumber(htmlelement.Index + 1);
                            }
                            else
                            {
                                element.before = value;
                            }
                        }
                    }
                    #endregion before

                    #region after
                    else if (declaration.StartsWith("after|"))
                    {
                        string[] partpseudo = declaration.Split('|');
                        string nopseudo = partpseudo[1];
                        string[] feature_value = nopseudo.Split(':');
                        string feature = feature_value[0];
                        string value = feature_value[1].TrimStart();

                        if (feature.Equals("content"))
                        {
                            value = value.Substring(1, value.Length - 2);
                            element.after = value;
                        }
                    }
                    #endregion after

                    else
                    {
                        if (!(declaration == null))
                        {
                            string[] feature_value = declaration.Split(':');
                            string feature = feature_value[0];
                            string value = feature_value[1].TrimStart();
                            switch (feature)
                            {
                                #region margin
                                case "margin":
                                    if (value.Contains(' '))
                                    {
                                        string[] multivalues = value.Split(' ');
                                        int size = multivalues.Length;


                                        switch (size)
                                        {
                                            case 2:
                                                BoxModel margin2 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]));
                                                element.margin = margin2;
                                                break;
                                            case 3:
                                                BoxModel margin3 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], element.lineheight));
                                                element.margin = margin3;
                                                break;
                                            case 4:
                                                BoxModel margin4 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[3]));
                                                element.margin = margin4;
                                                break;

                                        }

                                    }
                                    else
                                    {

                                        BoxModel margin1 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value), RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value));
                                        element.margin = margin1;
                                    }

                                    break;
                                #endregion margin

                                #region border-width
                                case "border-width":
                                    if (value.Contains(' ')) //Angaben müssen der Form "margin: 4px 2px 3px" sein
                                    {
                                        string[] multivalues = value.Split(' ');
                                        int size = multivalues.Length;
                                        uint[] borderpixel = new uint[size];

                                        switch (size)
                                        {
                                            case 2:
                                                BoxModel border2 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]));
                                                element.border = border2;
                                                break;
                                            case 3:
                                                BoxModel border3 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], element.lineheight));
                                                element.border = border3;
                                                break;
                                            case 4:
                                                BoxModel border4 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[3]));
                                                element.border = border4;
                                                break;

                                        }

                                    }
                                    else
                                    {
                                        BoxModel border1 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value), RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value));
                                        element.border = border1;
                                    }
                                    break;
                                #endregion border

                                #region border-left-width
                                case "border-left-width":
                                    element.border.Left = RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight);
                                    break;
                                #endregion border-left-width

                                #region border-right-width
                                case "border-right-width":
                                    element.border.Right = RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight);
                                    break;
                                #endregion border-right-width

                                #region border-top-width
                                case "border-top-width":
                                    element.border.Top = RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight);
                                    break;
                                #endregion border-top-width

                                #region border-bottom-width
                                case "border-bottom-width":
                                    element.border.Bottom = RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight);
                                    break;
                                #endregion border-bottom-width

                                #region border-bottom-style
                                case "border-bottom-style":
                                    element.border_bottom_style = value;
                                    break;
                                #endregion border-bottom-style

                                #region padding
                                case "padding":
                                    if (value.Contains(' '))
                                    {
                                        string[] multivalues = value.Split(' ');
                                        int size = multivalues.Length;

                                        switch (size)
                                        {
                                            case 2:
                                                BoxModel padding2 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]));
                                                element.padding = padding2;
                                                break;
                                            case 3:
                                                BoxModel padding3 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], element.lineheight));
                                                element.padding = padding3;
                                                break;
                                            case 4:
                                                BoxModel padding4 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[3]));
                                                element.padding = padding4;
                                                break;

                                        }

                                    }
                                    else
                                    {

                                        BoxModel padding1 = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value), RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value));
                                        element.padding = padding1;
                                    }
                                    break;
                                #endregion padding

                                #region text-indent
                                case "text-indent":
                                    element.text_indent = Convert.ToInt32(RTBrailleRendererHelper.ConvertUnitsInt(value));
                                    break;
                                #endregion text-indent

                                #region height
                                case "height":
                                    element.height = RTBrailleRendererHelper.ConvertUnitsVert(value, element.lineheight);
                                    break;
                                #endregion height

                                #region width
                                case "width":
                                    element.width = RTBrailleRendererHelper.ConvertUnitsHor(value);
                                    break;
                                #endregion width

                                #region display
                                case "display":
                                    if (value == "none")
                                    {
                                        element.hide = true;
                                    }
                                    else
                                    {
                                        element.hide = false;
                                    }
                                    break;
                                #endregion display

                            }
                        }

                    }
                }

                if (!element.hide)
                {
                    M = GeneralDraw(ref M, element, rules, ref x, ref y, ref x_max, ref x_min); //Call GeneralDraw for a child node with parent coordinates
                }
            }
        }


        /// <summary>
        /// General method to start drawing on the display for the parent node.
        /// </summary>
        /// <param name="M">The matrix of the display.</param>
        /// <param name="element">The HtmlBrailleElement to be drawn.</param>
        /// <param name="rules">The css rules.</param>
        /// <returns></returns>
        public List<bool[]> GeneralDraw(ref List<bool[]> M, HtmlBrailleElement element, Dictionary<String, List<String>> rules)
        {

            //private object height --> check for null object, then cast to uint/int set 0

            #region initializeVariables
            uint x = coords.CurrentX;
            uint y = coords.CurrentY;
            uint x_max;
            if (element.width > maxwidth || element.width == 0)
            {
                x_max = (maxwidth - 1);
            }
            else
            {
                x_max = element.width - 1;
            }

            uint margin_bot = 0;
            uint padding_bot = 0;
            uint x_firstline = 0;
            bool firstline = true;

            uint lineheigt = 5; //standard line-height with 4 px for braille and 1 space
            bool heightLock = false; //maximum height reached
            uint height = 0; //calculated hight wiht BoxModel


            //Border drawn at the end, saving coords
            bool hasBorder = false;
            uint borderstartx = 0;
            uint borderendx = 0;
            uint borderstarty = 0;
            uint borderendy = 0;

            //Underline drawn later
            bool hasUnderlining = false;
            bool isDoubleUnderline = false;
            bool dotBefore = false;
            #endregion initializeVariables

            #region margin
            // if(!(element.margin.hasBox()))
            if (!(element.margin.Equals(null)))
            {
                //Verschieben des aktuellen x/y Wertes und des max x Wertes um die marginpixel
                x = x + element.margin.Left;
                y = y + element.margin.Top;
                x_max = x_max - element.margin.Right;
                margin_bot = element.margin.Bottom;
            }
            #endregion margin

            #region border
            if (!(element.border.Equals(null)))
            {
                //Zwischenspeichern für Zeichnen des Borders am Ende
                hasBorder = true;
                borderstartx = x;
                borderendx = x_max;
                borderstarty = y;

                //Verschieben des aktuellen 
                x = x + element.border.Left;
                x_max = x_max - element.border.Right;
                y = y + element.border.Top;

            }
            #endregion border

            #region padding
            if (!(element.padding.Equals(null)))
            {
                x = x + element.padding.Left;
                x_max = x_max - element.padding.Right;
                y = y + element.padding.Top;
                padding_bot = element.padding.Bottom;

            }
            #endregion padding

            #region text_indent
            if (element.text_indent < 0)
            {
                uint temp = 0;

                temp = Convert.ToUInt32(Math.Abs(element.text_indent));
                if (x - temp >= 0)
                {
                    x_firstline = x - temp;
                }
            }
            else
            {

                x_firstline = x + Convert.ToUInt32(element.text_indent);
            }
            #endregion text_indent

            #region border-bottom-style
            if (!(element.border_bottom_style == null))
            {
                hasUnderlining = true;
                if (element.border_bottom_style == "double")
                    isDoubleUnderline = true;

            }

            #endregion border-bottom-style

            #region line-height
            //
            if (!(element.lineheight == null))
            {
                lineheigt = element.lineheight;
                if ((lineheigt < 5 && hasUnderlining) || (lineheigt < 6 && isDoubleUnderline))
                {
                    lineheigt = 6;
                }
            }
            #endregion line-height

            #region height
            if (!(element.height.Equals(null)))
            {
                //height = coords.CurrentY + element.margin.Top + element.border.Top + element.padding.Top + element.height;
                height = y + element.height;
            }
            #endregion height


            //Create Lines for BoxModel and first row of braille with specified lineheight
            for (int i = 0; i < (y - coords.CurrentY) + lineheigt; i++)
            {
                M.Add(RTBrailleRendererHelper.newLine(maxwidth));
            }



            //coords.CurrentX = x;
            uint x_afterBox = x;

            //Iterate through nodeList
            #region IterateNodes
            for (int nodeNumber = 0; nodeNumber < element.nodelist.Length; nodeNumber++)
            {
                string temptext = null;
                if (firstline && !heightLock)
                {
                    checkNode(element.nodelist[nodeNumber], ref M, rules, ref x_firstline, ref y, ref x_max, ref x_afterBox, ref temptext);
                    element.text = temptext;
                }
                else if (!heightLock)
                {
                    checkNode(element.nodelist[nodeNumber], ref M, rules, ref x, ref y, ref x_max, ref x_afterBox, ref temptext);
                    element.text = temptext;
                }


                if (!String.IsNullOrEmpty(element.text))
                {
                    element.text = SplitString(command, param, element.text);

                    string[] words = element.text.Split(' ');

                    #region before
                    //if (!(element.before.Equals(null)))
                    if (!(element.before == null))
                    {
                        switch (element.selector)
                        {
                            // Insert before every word
                            case "b":
                                for (int i = 0; i < words.Length; i++)
                                {
                                    words[i] = words[i].Insert(0, element.before + "\\x2800");
                                }
                                break;
                            // Insert only at the beginning
                            case "u":
                                words[0] = words[0].Insert(0, element.before + "\\x2800");
                                break;
                            case "li":
                                words[0] = words[0].Insert(0, element.before + "\\x2800");
                                break;
                        }
                    }
                    #endregion before

                    #region after
                    if (!(element.after == null))
                    {
                        switch (element.selector)
                        {
                            // Insert after every word
                            case "b":
                                for (int i = 0; i < words.Length; i++)
                                {
                                    words[i] = words[i].Insert(words[i].Length, element.after);
                                }
                                break;
                            // Insert only at the end of last word
                            case "u":
                                words[words.Length - 1] = words[words.Length - 1].Insert(words[words.Length - 1].Length, element.after);
                                break;
                        }
                    }
                    #endregion after

                    #region drawWords
                    //foreach (string word in words)
                    for (int wordindex = 0; wordindex < words.Length; wordindex++)
                    {
                        //Insert before elements

                        //Check if set height is already reached for next line -> can't be drawn
                        if (!heightLock && !String.IsNullOrEmpty(words[wordindex]))
                        {
                            string[] chars = words[wordindex].Split('\\');
                            // first line? -> text indent!

                            #region firstline
                            if (firstline)
                            {
                                //line break needed? -> add new Lines, set y + 4+lineheight
                                if (x_firstline + chars.Length * 3 > x_max)
                                {
                                    //If current position +1 is bigger then Top BoxModell plus the specified lineheight times (4 Rows for braille sign plus lineheight)

                                    if ((element.height > 0) && (y + lineheigt >= height))   //If set height not reached, continue drawing
                                    {
                                        heightLock = true;
                                        break;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < lineheigt; i++)
                                        {
                                            M.Add(RTBrailleRendererHelper.newLine(maxwidth));
                                        }

                                        //Set y for the next row of braille symbols
                                        y = y + lineheigt;



                                        //continue writing with fresh line
                                        for (int j = 1; j < chars.Length; j++)
                                        {
                                            chars[j] = chars[j].Insert(0, "\\");
                                            bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
                                            M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x, ref y);
                                            if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
                                            {
                                                Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
                                            }
                                            x = x + 3;

                                        }
                                        if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
                                        {
                                            Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
                                        }
                                        x = x + 3;
                                        firstline = false;
                                    }

                                }
                                else

                                //No line break needed yet
                                {
                                    for (int j = 1; j < chars.Length; j++)
                                    {
                                        chars[j] = chars[j].Insert(0, "\\");
                                        bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
                                        M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x_firstline, ref y);
                                        if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
                                        {
                                            Underline(ref M, element.border_bottom_style, ref x_firstline, ref y, ref dotBefore);
                                        }
                                        x_firstline = x_firstline + 3;

                                    }
                                    if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
                                    {
                                        Underline(ref M, element.border_bottom_style, ref x_firstline, ref y, ref dotBefore);
                                    }
                                    x_firstline = x_firstline + 3;


                                }
                            }
                            #endregion firstline

                            //not first line anymore -> no text_indent
                            #region notfirstline
                            else
                            {
                                //line break needed? -> add new Lines, set y + 4+lineheight
                                if (x + chars.Length * 3 > x_max)
                                {
                                    if ((element.height > 0) && (y + lineheigt >= height))   //If set height not reached, continue drawing
                                    {
                                        heightLock = true;
                                        break;
                                    }

                                    //Draw new line normally
                                    else
                                    {
                                        for (int i = 0; i < lineheigt; i++)
                                        {
                                            M.Add(RTBrailleRendererHelper.newLine(maxwidth));
                                        }

                                        //Set y for the next row of braille symbols and reset x to x-coord after the BoxModel applied
                                        y = y + lineheigt;
                                        x = x_afterBox;

                                        for (int j = 1; j < chars.Length; j++)
                                        {
                                            chars[j] = chars[j].Insert(0, "\\");
                                            bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
                                            M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x, ref y);
                                            if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
                                            {
                                                Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
                                            }
                                            x = x + 3;
                                        }

                                        if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
                                        {
                                            Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
                                        }
                                        x = x + 3;
                                        firstline = false;
                                    }
                                }

                                    //No line break needed yet, draw normally
                                else
                                {
                                    if ((element.height > 0) && (y + 1 > height))
                                    {
                                        heightLock = true;
                                        break;
                                    }
                                    else
                                    {

                                        for (int j = 1; j < chars.Length; j++)
                                        {
                                            chars[j] = chars[j].Insert(0, "\\");
                                            bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
                                            M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x, ref y);
                                            if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
                                            {
                                                Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
                                            }
                                            x = x + 3;

                                        }

                                        if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
                                        {
                                            Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
                                        }
                                        x = x + 3;
                                    }

                                }

                            }
                            #endregion notfirstline


                        }

                        //

                    } //End for word in words

                    if (element.selector == "li")
                    {
                        x = x_afterBox;
                        y = y + lineheigt;
                        for (int i = 0; i < lineheigt; i++)
                        {
                            M.Add(RTBrailleRendererHelper.newLine(maxwidth));
                        }
                    }
                    #endregion drawWords
                }
            }
            #endregion IterateNodes


            uint boxBot = padding_bot + element.border.Bottom + margin_bot;
            for (int i = 0; i < boxBot; i++)
            {
                M.Add(RTBrailleRendererHelper.newLine(maxwidth));
            }

            borderendy = y + lineheigt + padding_bot + element.border.Bottom - 1;
            y = y + lineheigt + boxBot;

            // am Ende borderendy setzen
            if (hasBorder)
            {
                M = drawBorder(ref M, element.border, borderstartx, borderendx, borderstarty, borderendy);
            }

            //GANZ am Ende Margin Bot...

            //Vor verlassen globales X und Y setzen
            coords.CurrentY = y;
            return M;

        }


        //TODO: zweite general draw anpassen
        /// <summary>
        /// General method to start drawing on the display for the child nodes.
        /// </summary>
        /// <param name="M">The matrix of the display.</param>
        /// <param name="element">The HtmlBrailleElement to be drawn.</param>
        /// <param name="rules">The css rules.</param>
        /// <param name="parentx">The current x of the parent.</param>
        /// <param name="parenty">The current y of the parent.</param>
        /// <param name="parentx_max">The current x_max of the parent.</param>
        /// <returns></returns>
        public List<bool[]> GeneralDraw(ref List<bool[]> M, HtmlBrailleElement element, Dictionary<String, List<String>> rules, ref uint parentx, ref uint parenty, ref uint parentx_max, ref uint parentx_min)
        {
            //exception
            try
            {

            }
            catch (NullReferenceException ex)
            {
                var Ex2 = new ArgumentException("Css not valid", ex);
                throw Ex2;
            }
            //private object height;

            #region initializeVariables
            uint x = parentx;
            uint y = parenty;
            uint x_max;
            uint x_afterBox = parentx_min;

            if (element.width > parentx_max || element.width == 0)
            {
                x_max = (parentx_max - 1);
            }
            else
            {
                x_max = element.width - 1;
            }

            uint margin_bot = 0;
            uint padding_bot = 0;
            uint x_firstline = x;
            bool firstline = true;

            uint lineheigt = 5; //standard line-height with 4 px for braille and 1 space
            bool heightLock = false;
            uint height = 0; //calculated hight wiht BoxModel


            //Border wird am Ende erst gezeichnet, deshalb Werte zwischenspeichern
            bool hasBorder = false;
            uint borderstartx = 0;
            uint borderendx = 0;
            uint borderstarty = 0;
            uint borderendy = 0;

            bool hasUnderlining = false;
            bool isDoubleUnderline = false;
            bool dotBefore = false;
            #endregion initializeVariables


            #region margin
            // if(!(element.margin.hasBox()))
            if (!(element.margin.Equals(null)))
            {
                //Verschieben des aktuellen x/y Wertes und des max x Wertes um die marginpixel
                x = x + element.margin.Left;
                x_afterBox = x_afterBox + element.margin.Left;
                y = y + element.margin.Top;
                x_max = x_max - element.margin.Right;
                margin_bot = element.margin.Bottom;
            }
            #endregion margin

            #region border
            if (!(element.border.Equals(null)))
            {
                //Zwischenspeichern für Zeichnen des Borders am Ende
                hasBorder = true;
                borderstartx = x;
                borderendx = x_max;
                borderstarty = y;

                //Verschieben des aktuellen 
                x = x + element.border.Left;
                x_afterBox = x_afterBox + element.border.Left;
                x_max = x_max - element.border.Right;
                y = y + element.border.Top;

            }
            #endregion border

            #region padding
            if (!(element.padding.Equals(null)))
            {
                x = x + element.padding.Left;
                x_afterBox = x_afterBox + element.padding.Left;
                x_max = x_max - element.padding.Right;
                y = y + element.padding.Top;
                padding_bot = element.padding.Bottom;

            }
            #endregion padding

            #region text_indent
            if (element.text_indent < 0)
            {
                uint temp = 0;

                temp = Convert.ToUInt32(Math.Abs(element.text_indent));
                if (x - temp >= 0)
                {
                    x_firstline = x - temp;
                }
            }
            else
            {

                x_firstline = x + Convert.ToUInt32(element.text_indent);
            }
            #endregion text_indent

            #region border-bottom-style
            if (!(element.border_bottom_style == null))
            {
                hasUnderlining = true;
                if (element.border_bottom_style == "double")
                    isDoubleUnderline = true;

            }

            #endregion border-bottom-style

            #region line-height
            //
            if (!(element.lineheight == null))
            {
                lineheigt = element.lineheight;
                if ((lineheigt < 5 && hasUnderlining) || (lineheigt < 6 && isDoubleUnderline))
                {
                    lineheigt = 6;
                }
            }
            #endregion line-height

            #region height
            if (!(element.height.Equals(null)))
            {
                height = parenty + element.margin.Top + element.border.Top + element.padding.Top + (element.lineheight) * element.height;
            }
            #endregion height



            //Create Lines for BoxModel and first row of braille with specified lineheight
            for (int i = 0; i < (y - parenty) + lineheigt; i++)
            {
                M.Add(RTBrailleRendererHelper.newLine(maxwidth));
            }




            //Iterate through nodeList
            #region IterateNodes
            for (int nodeNumber = 0; nodeNumber < element.nodelist.Length; nodeNumber++)
            {
                string temptext = null;

                if (firstline && !heightLock)
                {
                    checkNode(element.nodelist[nodeNumber], ref M, rules, ref x_firstline, ref y, ref x_max, ref x_afterBox, ref temptext);
                    element.text = temptext;
                }
                else if (!heightLock)
                {
                    checkNode(element.nodelist[nodeNumber], ref M, rules, ref x, ref y, ref x_max, ref x_afterBox, ref temptext);
                    element.text = temptext;
                }
                if (!String.IsNullOrEmpty(element.text))
                {
                    element.text = element.text = SplitString(command, param, element.text);


                    string[] words = element.text.Split(' ');

                    #region before
                    //if (!(element.before.Equals(null)))
                    if (!(element.before == null))
                    {
                        switch (element.selector)
                        {
                            // Insert before every word
                            case "b":
                                for (int i = 0; i < words.Length; i++)
                                {
                                    words[i] = words[i].Insert(0, element.before + "\\2800");
                                }
                                break;
                            // Insert only at the beginning
                            case "u":
                                words[0] = words[0].Insert(0, element.before + "\\2800");
                                break;
                            case "li":
                                words[0] = words[0].Insert(0, element.before + "\\2800");
                                break;
                        }
                    }
                    #endregion before

                    #region after
                    if (!(element.after == null))
                    {
                        switch (element.selector)
                        {
                            // Insert after every word
                            case "b":
                                for (int i = 0; i < words.Length; i++)
                                {
                                    words[i] = words[i].Insert(words[i].Length, element.after);
                                }
                                break;
                            // Insert only at the end of last word
                            case "u":
                                words[words.Length - 1] = words[words.Length - 1].Insert(words[words.Length - 1].Length, element.after);
                                break;
                        }
                    }
                    #endregion after

                    #region drawWords

                    for (int wordindex = 0; wordindex < words.Length; wordindex++)
                    {
                        //Insert before elements

                        //Check if set height is already reached for next line -> can't be drawn
                        if (!heightLock && !String.IsNullOrEmpty(words[wordindex]))
                        {
                            string[] chars = words[wordindex].Split('\\');
                            // first line? -> text indent!
                            #region firstline
                            if (firstline)
                            {
                                //line break needed? -> add new Lines, set y + 4+lineheight
                                if (x_firstline + chars.Length * 3 > x_max)
                                {
                                    if ((element.height > 0) && (y + lineheigt >= height))   //If set height not reached, continue drawing
                                    {
                                        heightLock = true;
                                        break;
                                    }

                                    //If set height not reached, continue drawing
                                    else
                                    {

                                        for (int i = 0; i < lineheigt; i++)
                                        {
                                            M.Add(RTBrailleRendererHelper.newLine(maxwidth));
                                        }

                                        //Set y for the next row of braille symbols
                                        y = y + lineheigt;


                                        x = x_afterBox;


                                        //continue writing with fresh line
                                        for (int j = 1; j < chars.Length; j++)
                                        {
                                            chars[j] = chars[j].Insert(0, "\\");
                                            bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
                                            M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x, ref y);
                                            if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
                                            {
                                                Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
                                            }
                                            x = x + 3;

                                        }
                                        if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
                                        {
                                            Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
                                        }
                                        x = x + 3;
                                        firstline = false;
                                    }
                                }
                                else

                                //No line break needed yet
                                {


                                    for (int j = 1; j < chars.Length; j++)
                                    {
                                        chars[j] = chars[j].Insert(0, "\\");
                                        bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
                                        M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x_firstline, ref y);
                                        if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
                                        {
                                            Underline(ref M, element.border_bottom_style, ref x_firstline, ref y, ref dotBefore);
                                        }
                                        x_firstline = x_firstline + 3;

                                    }
                                    if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
                                    {
                                        Underline(ref M, element.border_bottom_style, ref x_firstline, ref y, ref dotBefore);
                                    }
                                    x_firstline = x_firstline + 3;

                                }
                            }
                            #endregion firstline

                            //not first line anymore -> no text_indent
                            #region notfirstline
                            else
                            {
                                //line break needed? -> add new Lines, set y + 4+lineheight
                                if (x + chars.Length * 3 > x_max)
                                {
                                    //If current position +1 is bigger then Top BoxModell plus the specified lineheight times (4 Rows for braille sign plus lineheight)
                                    if ((element.height > 0) && (y + 1 > height))
                                    {
                                        heightLock = true;
                                        break;
                                    }

                                    //Draw new line normally
                                    else
                                    {
                                        for (int i = 0; i < lineheigt; i++)
                                        {
                                            M.Add(RTBrailleRendererHelper.newLine(maxwidth));
                                        }

                                        //Set y for the next row of braille symbols and reset x to x-coord after the BoxModel applied
                                        y = y + lineheigt;
                                        x = x_afterBox;

                                        for (int j = 1; j < chars.Length; j++)
                                        {
                                            chars[j] = chars[j].Insert(0, "\\");
                                            bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
                                            M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x, ref y);
                                            if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
                                            {
                                                Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
                                            }
                                            x = x + 3;
                                        }

                                        if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
                                        {
                                            Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
                                        }
                                        x = x + 3;
                                        firstline = false;
                                    }
                                }

                                    //No line break needed yet, draw normally
                                else
                                {


                                    for (int j = 1; j < chars.Length; j++)
                                    {
                                        chars[j] = chars[j].Insert(0, "\\");
                                        bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
                                        M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x, ref y);
                                        if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
                                        {
                                            Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
                                        }
                                        x = x + 3;

                                    }

                                    if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
                                    {
                                        Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
                                    }
                                    x = x + 3;

                                }

                            }
                            #endregion notfirstline

                        }

                        //

                    } //End for word in words
                    if (element.selector == "li")
                    {
                        x = x_afterBox;
                        y = y + lineheigt;
                        for (int i = 0; i < lineheigt; i++)
                        {
                            M.Add(RTBrailleRendererHelper.newLine(maxwidth));
                        }

                    }
                    else
                    {
                        if (firstline)
                            parentx = x_firstline;
                        else
                            parentx = x;

                    }

                    //delete list test

                    #endregion drawWords
                }
            }
            #endregion IterateNodes

            //delete
            parenty = y;

            uint boxBot = padding_bot + element.border.Bottom + margin_bot;
            for (int i = 0; i < boxBot; i++)
            {
                M.Add(RTBrailleRendererHelper.newLine(maxwidth));
            }


            borderendy = y + lineheigt + padding_bot + element.border.Bottom - 1;
            y = y + lineheigt + boxBot;

            // am Ende borderendy setzen
            if (hasBorder)
            {
                M = drawBorder(ref M, element.border, borderstartx, borderendx, borderstarty, borderendy);
            }

            //GANZ am Ende Margin Bot...

            //Vor verlassen globales X und Y setzen
            //coords.CurrentY = y;
            return M;

        }

        /// <summary>
        /// Checks the nodes to look for nested html-tags.
        /// </summary>
        /// <param name="nodes">The list of nodes.</param>
        /// <param name="M">The matrix of the display.</param>
        /// <param name="rules">The css rules.</param>
        /// <param name="x">The current x.</param>
        /// <param name="y">The current y.</param>
        /// <param name="x_max">The current x_max.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private void checkNode(IDomObject node, ref List<bool[]> M, Dictionary<String, List<String>> rules, ref uint x, ref uint y, ref uint x_max, ref uint x_min, ref string text)
        {
            if (node.NodeType == NodeType.TEXT_NODE)
            {
                text = node.ToString();
            }
            else
            {
                NewElement(ref M, node, rules, ref x, ref y, ref x_max, ref x_min);
            }

        }





        //Ende der Klasse RTBrailleRenderer




        































    }



    class Globals
    {
        uint _currentX;
        public uint CurrentX
        {
            get
            {
                return _currentX;
            }
            set
            {
                _currentX = value;
            }
        }

        uint _currentY;
        public uint CurrentY
        {
            get
            {
                return _currentY;
            }
            set
            {
                _currentY = value;
            }
        }
    }




}
