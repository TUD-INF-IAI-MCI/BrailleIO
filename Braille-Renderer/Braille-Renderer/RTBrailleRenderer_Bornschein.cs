using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery;
using System.IO;

namespace tud.mci.tangram.Braille_Renderer
{
    public partial class RTBrailleRenderer
    {

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                          JB                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public RTBrailleRenderer()
            : this(pathToLiblouis, pathToTables)
        { }


        /// <summary>
        /// Initializes a new instance of the <see cref="RTBrailleRenderer"/> class.
        /// </summary>
        /// <param name="htmlfile">The htmlfile.</param>
        /// <param name="cssfile">The cssfile.</param>
        /// <param name="_pathToLiblouis">The path to liblouis.</param>
        /// <param name="_pathToTables">The path to liblouis tables.</param>
        /// <param name="maxwidth">The maximum width of the display</param>
        public RTBrailleRenderer(string _pathToLiblouis, string _pathToTables)
        {
            pathToLiblouis = _pathToLiblouis;
            pathToTables = _pathToTables;
            coords = new Globals();
            command = pathToLiblouis + @"lou_translate";
        }

        public List<bool[]> RenderHTMLDoc(string htmlfile, string cssfile, uint maxwidth, string tables)
        {
            this.htmlfile = htmlfile;
            this.cssfile = cssfile;
            this.maxwidth = maxwidth;
            this.tables = tables;
            reset();

            param = "--forward \"" + pathToTables + tables.Replace(",", "," + pathToTables) + "\"";
            param = param.TrimEnd();


            Dictionary<String, List<String>> rules = RTBCssParser.CssToDictionary(cssfile);

            /*************************** JB **********************/
            //Dictionary<String, Dictionary<String, Object>> rules = RTBCssParser.CssToDictionary(cssfile);
            CQ dom = htmlparse(htmlfile);

            return IterateThroughDom(M, dom, rules);
        }

        bool reset()
        {
            M.Clear();
            coords = new Globals();
            return true;
        }

        /// <summary>
        /// parses the specified htmlfile to a DOM.
        /// </summary>
        /// <param name="htmlfile">The htmlfile.</param>
        /// <param name="pathToLiblouis">The path to liblouis.</param>
        /// <param name="pathToTables">The path to tables.</param>
        /// <returns>The DOM of html file</returns>
        CQ htmlparse(string htmlfile)
        {
            string htmldok;

            if (File.Exists(htmlfile))
            {
                htmldok = File.ReadAllText(htmlfile);
            }
            else htmldok = String.IsNullOrEmpty(htmlfile) ? "" : htmlfile;

            htmldok = htmldok.Replace("\r\n<ul>", "<ul>");
            htmldok = htmldok.Replace("<ul>\r\n", "<ul>");
            htmldok = htmldok.Replace("<ol>\r\n", "<ol>");
            htmldok = htmldok.Replace("</li>\r\n", "</li>");
            htmldok = htmldok.Replace("</ul>\r\n", "</ul>");
            htmldok = htmldok.Replace("</ol>\r\n", "</ol>");
            htmldok = htmldok.Replace(">\r\n<", "><");
            htmldok = htmldok.Replace("\r\n\r\n<", "</p><");
            htmldok = htmldok.Replace("\r\n\r\n", "</p><p>");
            htmldok = htmldok.Replace("/p>\r\n<", "/p><");
            htmldok = htmldok.Replace(">\r\n", "><p>");
            htmldok = htmldok.Replace("\r\n<", "</p><");
            htmldok = htmldok.Replace("<p><p>", "<p>");
            htmldok = htmldok.Replace("</p></p>", "</p>");

            CQ dom = CQ.CreateDocument(htmldok);

            return dom;
        }

        private const int SPLIT_STRING_MAX_LENGTH = 60;
        private string SplitString(string command, string param, string text)
        {
            string output = null;
            string temp = null;
            int splitCount = (text.Length / SPLIT_STRING_MAX_LENGTH) + 1;
            string[] textarray = new string[splitCount];

            for (int i = 0; i < splitCount; i++)
            {
                if (i == splitCount - 1)
                {
                    temp = text.Substring(i * SPLIT_STRING_MAX_LENGTH);
                    textarray[i] = RTBrailleRendererHelper.cmdCall(command, param, temp);
                }
                else
                {
                    temp = text.Substring(i * SPLIT_STRING_MAX_LENGTH, SPLIT_STRING_MAX_LENGTH);
                    textarray[i] = RTBrailleRendererHelper.cmdCall(command, param, temp);
                }

            }

            for (int j = 0; j < splitCount; j++)
            {
                output = output + textarray[j];
            }

            return output;

        }


        ///// <summary>
        ///// Prepares the element with respect to the css declarations.
        ///// </summary>
        ///// <author>Jens Bornschein</author>
        ///// <param name="htmlelement">The htmlelement.</param>
        ///// <param name="rules">The css rules.</param>
        ///// <returns>an HTML Struct modeling the css data</returns>
        //HtmlBrailleElement prepareElement(IDomObject htmlelement, Dictionary<String, Dictionary<String, Object>> rules)
        //{
        //    HtmlBrailleElement element = new HtmlBrailleElement();
        //    string selector = RTBrailleRendererHelper.getSelectorFromElement(htmlelement);
        //    Dictionary<String, Object> declarations;
        //    if (!rules.ContainsKey(selector)) { declarations = new Dictionary<String, Object>(); }
        //    else { declarations = rules[selector]; }
        //    if (String.IsNullOrWhiteSpace(selector) || (declarations.ContainsKey("display") && declarations["display"].Equals("none")))
        //    {
        //        element.hide = true;
        //        return element;
        //    }

        //    INodeList nodes = (((CsQuery.Implementation.DomContainer<CsQuery.Implementation.DomElement>)(htmlelement))).ChildNodes;
        //    element.nodelist = nodes;
        //    element.selector = selector;
        //    element.before = null;
        //    element.after = null;
        //    element.lineheight = DEFAULT_LINE_HEIGHT;
        //    element.width = maxwidth;


        //    element.margin = declarations.ContainsKey("margin") && declarations["margin"] is BoxModel ? (BoxModel)declarations["margin"] : new BoxModel(0);
        //    element.border = declarations.ContainsKey("border-width") && declarations["border-width"] is BoxModel ? (BoxModel)declarations["border-width"] : new BoxModel(0);
        //    element.border_bottom_style = declarations.ContainsKey("border-bottom-style") ? declarations["border-bottom-style"].ToString() : String.Empty;
        //    element.padding = declarations.ContainsKey("padding") && declarations["padding"] is BoxModel ? (BoxModel)declarations["padding"] : new BoxModel(0);
        //    element.text_indent = declarations.ContainsKey("text-indent") ? Convert.ToInt32(declarations["text-indent"].ToString()) : 0;
        //    //TODO: check this;
        //    if (declarations.ContainsKey("height")) element.height = Convert.ToUInt16(declarations["height"].ToString());
        //    if (declarations.ContainsKey("width")) element.width = Convert.ToUInt16(declarations["width"].ToString());
        //    if (declarations.ContainsKey("line-height")) element.lineheight = Convert.ToUInt16(declarations["line-height"].ToString());

        //    if (declarations.ContainsKey("before")) element.before = declarations["before"].ToString();
        //    if (declarations.ContainsKey("after")) element.before = declarations["after"].ToString();

        //    return element;
        //}


        /// <summary>
        /// Iterates through the DOM. Each entry is parent node.
        /// </summary>
        /// <param name="M">The matrix of the display.</param>
        /// <param name="dom">The DOM.</param>
        /// <param name="rules">The css rules.</param>
        List<bool[]> IterateThroughDom(List<bool[]> M, CQ dom, Dictionary<String, List<String>> rules)
        {
            //get Body node
            var body = dom.Document.Body;

            coords.CurrentX = 0;

            uint x = coords.CurrentX;
            uint y = coords.CurrentY;

            uint mw = maxwidth;
            uint minx = 0;


            foreach (IDomObject child in body.ChildNodes)
            {
                if (child != null)
                {
                    //NewElement(ref M, child, rules, ref x, ref y, ref mw, ref minx);
                    NewElement(ref M, child, rules);
                }
            }
            //RTBrailleRendererHelper.PaintBoolMatrixToImage(M.ToArray(), "testimage.bmp");
            return M;
        }

        ///// <summary>
        ///// Iterates through the DOM. Each entry is parent node.
        ///// </summary>
        ///// <param name="M">The matrix of the display.</param>
        ///// <param name="dom">The DOM.</param>
        ///// <param name="rules">The css rules.</param>
        //List<bool[]> IterateThroughDom(List<bool[]> M, CQ dom, Dictionary<String, Dictionary<String, Object>> rules)
        //{
        //    //get Body node
        //    var body = dom.Document.Body;

        //    coords.CurrentX = 0;

        //    uint x = coords.CurrentX;
        //    uint y = coords.CurrentY;

        //    uint mw = maxwidth;
        //    uint minx = 0;


        //    foreach (IDomObject child in body.ChildNodes)
        //    {
        //        if (child != null)
        //        {
        //            //NewElement(ref M, child, rules, ref x, ref y, ref mw, ref minx);
        //            NewElement(ref M, child, rules);
        //        }
        //    }
        //    //RTBrailleRendererHelper.PaintBoolMatrixToImage(M.ToArray(), "testimage.bmp");
        //    return M;
        //}

        ///// <summary>
        ///// Creates new struct 'HtmlBrailleElement' for child nodes).
        ///// </summary>
        ///// <param name="M">The matrix of the display</param>
        ///// <param name="htmlelement">The htmlelement from the DOM.</param>
        ///// <param name="rules">The css rules.</param>
        ///// <param name="x">The current x from the parent.</param>
        ///// <param name="y">The current y from the parent.</param>
        //void NewElement(ref List<bool[]> M, IDomObject htmlelement, Dictionary<String, Dictionary<String, Object>> rules, ref uint x, ref uint y, ref uint x_max, ref uint x_min)
        //{
        //    HtmlBrailleElement element = prepareElement(htmlelement, rules);
        //    if (!element.hide)
        //    {
        //        M = GeneralDraw(ref M, element, rules, ref x, ref y, ref x_max, ref x_min); //Call GeneralDraw for a child node with parent coordinates
        //    }
        //}


        ///// <summary>
        ///// Checks the nodes to look for nested html-tags.
        ///// </summary>
        ///// <param name="nodes">The list of nodes.</param>
        ///// <param name="M">The matrix of the display.</param>
        ///// <param name="rules">The css rules.</param>
        ///// <param name="x">The current x.</param>
        ///// <param name="y">The current y.</param>
        ///// <param name="x_max">The current x_max.</param>
        ///// <returns></returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        //private void checkNode(IDomObject node, ref List<bool[]> M, Dictionary<String, Dictionary<String, Object>> rules, ref uint x, ref uint y, ref uint x_max, ref uint x_min, ref string text)
        //{
        //    if (node.NodeType == NodeType.TEXT_NODE)
        //    {
        //        text = node.ToString();
        //    }
        //    else
        //    {
        //        NewElement(ref M, node, rules, ref x, ref y, ref x_max, ref x_min);
        //    }

        //}


        ///*******************************************************************************************
        // * 
        // *                           DELETE MABY WHEN ADAPT TO USE ONLY ONE FUNCTION
        // * 
        // * ******************************************************************************************/



        ///// <summary>
        ///// Creates new struct 'HtmlBrailleElement' for the first time (parent node).
        ///// </summary>
        ///// <param name="M">The matrix of the display.</param>
        ///// <param name="htmlelement">The htmlelement from the DOM.</param>
        ///// <param name="rules">The css rules.</param>
        //void NewElement(ref List<bool[]> M, IDomObject htmlelement, Dictionary<String, Dictionary<String, Object>> rules)
        //{
        //    HtmlBrailleElement element = prepareElement(htmlelement, rules);

        //    if (!element.hide)
        //    {
        //        M = GeneralDraw(ref M, element, rules); //Call GeneralDraw for first time without parent coordinates      
        //    }


        //}

        ///// <summary>
        ///// General method to start drawing on the display for the parent node.
        ///// </summary>
        ///// <param name="M">The matrix of the display.</param>
        ///// <param name="element">The HtmlBrailleElement to be drawn.</param>
        ///// <param name="rules">The css rules.</param>
        ///// <returns></returns>
        //public List<bool[]> GeneralDraw(ref List<bool[]> M, HtmlBrailleElement element, Dictionary<String, Dictionary<String, Object>> rules)
        //{

        //    //private object height --> check for null object, then cast to uint/int set 0

        //    #region initializeVariables
        //    uint x = coords.CurrentX;
        //    uint y = coords.CurrentY;
        //    uint x_max;
        //    if (element.width > maxwidth || element.width == 0)
        //    {
        //        x_max = (maxwidth - 1);
        //    }
        //    else
        //    {
        //        x_max = element.width - 1;
        //    }

        //    uint margin_bot = 0;
        //    uint padding_bot = 0;
        //    uint x_firstline = 0;
        //    bool firstline = true;

        //    uint lineheigt = DEFAULT_LINE_HEIGHT; //standard line-height with 4 px for braille and 1 space
        //    bool heightLock = false; //maximum height reached
        //    uint height = 0; //calculated hight with BoxModel


        //    //Border drawn at the end, saving coords
        //    bool hasBorder = false;
        //    uint borderstartx = 0;
        //    uint borderendx = 0;
        //    uint borderstarty = 0;
        //    uint borderendy = 0;

        //    //Underline drawn later
        //    bool hasUnderlining = false;
        //    bool isDoubleUnderline = false;
        //    bool dotBefore = false;
        //    #endregion initializeVariables

        //    #region margin
        //    // if(!(element.margin.hasBox()))
        //    if (!(element.margin.Equals(null)))
        //    {
        //        //Verschieben des aktuellen x/y Wertes und des max x Wertes um die marginpixel
        //        x = x + element.margin.Left;
        //        y = y + element.margin.Top;
        //        x_max = x_max - element.margin.Right;
        //        margin_bot = element.margin.Bottom;
        //    }
        //    #endregion margin

        //    #region border
        //    if (!(element.border.Equals(null)))
        //    {
        //        //Zwischenspeichern für Zeichnen des Borders am Ende
        //        hasBorder = true;
        //        borderstartx = x;
        //        borderendx = x_max;
        //        borderstarty = y;

        //        //Verschieben des aktuellen 
        //        x = x + element.border.Left;
        //        x_max = x_max - element.border.Right;
        //        y = y + element.border.Top;

        //    }
        //    #endregion border

        //    #region padding
        //    if (!(element.padding.Equals(null)))
        //    {
        //        x = x + element.padding.Left;
        //        x_max = x_max - element.padding.Right;
        //        y = y + element.padding.Top;
        //        padding_bot = element.padding.Bottom;

        //    }
        //    #endregion padding

        //    #region text_indent
        //    if (element.text_indent < 0)
        //    {
        //        uint temp = 0;

        //        temp = Convert.ToUInt32(Math.Abs(element.text_indent));
        //        if (x - temp >= 0)
        //        {
        //            x_firstline = x - temp;
        //        }
        //    }
        //    else
        //    {

        //        x_firstline = x + Convert.ToUInt32(element.text_indent);
        //    }
        //    #endregion text_indent

        //    #region border-bottom-style
        //    if (!(element.border_bottom_style == null))
        //    {
        //        hasUnderlining = true;
        //        if (element.border_bottom_style == "double")
        //            isDoubleUnderline = true;

        //    }

        //    #endregion border-bottom-style

        //    #region line-height
        //    //
        //    if (!(element.lineheight == null))
        //    {
        //        lineheigt = element.lineheight;
        //        if ((lineheigt < 5 && hasUnderlining) || (lineheigt < 6 && isDoubleUnderline))
        //        {
        //            lineheigt = 6;
        //        }
        //    }
        //    #endregion line-height

        //    #region height
        //    if (!(element.height.Equals(null)))
        //    {
        //        //height = coords.CurrentY + element.margin.Top + element.border.Top + element.padding.Top + element.height;
        //        height = y + element.height;
        //    }
        //    #endregion height


        //    //Create Lines for BoxModel and first row of braille with specified lineheight
        //    for (int i = 0; i < (y - coords.CurrentY) + lineheigt; i++)
        //    {
        //        M.Add(RTBrailleRendererHelper.newLine(maxwidth));
        //    }



        //    //coords.CurrentX = x;
        //    uint x_afterBox = x;

        //    //Iterate through nodeList
        //    #region IterateNodes
        //    for (int nodeNumber = 0; nodeNumber < element.nodelist.Length; nodeNumber++)
        //    {
        //        string temptext = null;
        //        if (firstline && !heightLock)
        //        {
        //            checkNode(element.nodelist[nodeNumber], ref M, rules, ref x_firstline, ref y, ref x_max, ref x_afterBox, ref temptext);
        //            element.text = temptext;
        //        }
        //        else if (!heightLock)
        //        {
        //            checkNode(element.nodelist[nodeNumber], ref M, rules, ref x, ref y, ref x_max, ref x_afterBox, ref temptext);
        //            element.text = temptext;
        //        }


        //        if (!String.IsNullOrEmpty(element.text))
        //        {
        //            element.text = SplitString(command, param, element.text);

        //            string[] words = element.text.Split(' ');

        //            #region before
        //            //if (!(element.before.Equals(null)))
        //            if (!(element.before == null))
        //            {
        //                switch (element.selector)
        //                {
        //                    // Insert before every word
        //                    case "b":
        //                        for (int i = 0; i < words.Length; i++)
        //                        {
        //                            words[i] = words[i].Insert(0, element.before + "\\x2800");
        //                        }
        //                        break;
        //                    // Insert only at the beginning
        //                    case "u":
        //                        words[0] = words[0].Insert(0, element.before + "\\x2800");
        //                        break;
        //                    case "li":
        //                        words[0] = words[0].Insert(0, element.before + "\\x2800");
        //                        break;
        //                }
        //            }
        //            #endregion before

        //            #region after
        //            if (!(element.after == null))
        //            {
        //                switch (element.selector)
        //                {
        //                    // Insert after every word
        //                    case "b":
        //                        for (int i = 0; i < words.Length; i++)
        //                        {
        //                            words[i] = words[i].Insert(words[i].Length, element.after);
        //                        }
        //                        break;
        //                    // Insert only at the end of last word
        //                    case "u":
        //                        words[words.Length - 1] = words[words.Length - 1].Insert(words[words.Length - 1].Length, element.after);
        //                        break;
        //                }
        //            }
        //            #endregion after

        //            #region drawWords
        //            //foreach (string word in words)
        //            for (int wordindex = 0; wordindex < words.Length; wordindex++)
        //            {
        //                //Insert before elements

        //                //Check if set height is already reached for next line -> can't be drawn
        //                if (!heightLock && !String.IsNullOrEmpty(words[wordindex]))
        //                {
        //                    string[] chars = words[wordindex].Split('\\');
        //                    // first line? -> text indent!

        //                    #region firstline
        //                    if (firstline)
        //                    {
        //                        //line break needed? -> add new Lines, set y + 4+lineheight
        //                        if (x_firstline + (chars.Length - 1) * 3 > x_max)
        //                        {
        //                            //If current position +1 is bigger then Top BoxModell plus the specified lineheight times (4 Rows for braille sign plus lineheight)

        //                            if ((element.height > 0) && (y + lineheigt >= height))   //If set height not reached, continue drawing
        //                            {
        //                                heightLock = true;
        //                                break;
        //                            }
        //                            else
        //                            {
        //                                for (int i = 0; i < lineheigt; i++)
        //                                {
        //                                    M.Add(RTBrailleRendererHelper.newLine(maxwidth));
        //                                }

        //                                //Set y for the next row of braille symbols
        //                                y = y + lineheigt;



        //                                //continue writing with fresh line
        //                                for (int j = 1; j < chars.Length; j++)
        //                                {
        //                                    chars[j] = chars[j].Insert(0, "\\");
        //                                    bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
        //                                    M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x, ref y);
        //                                    if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
        //                                    {
        //                                        Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
        //                                    }
        //                                    x = x + 3;

        //                                }
        //                                if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
        //                                {
        //                                    Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
        //                                }
        //                                x = x + 3;
        //                                firstline = false;
        //                            }

        //                        }
        //                        else

        //                        //No line break needed yet
        //                        {
        //                            for (int j = 1; j < chars.Length; j++)
        //                            {
        //                                chars[j] = chars[j].Insert(0, "\\");
        //                                bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
        //                                M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x_firstline, ref y);
        //                                if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
        //                                {
        //                                    Underline(ref M, element.border_bottom_style, ref x_firstline, ref y, ref dotBefore);
        //                                }
        //                                x_firstline = x_firstline + 3;

        //                            }
        //                            if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
        //                            {
        //                                Underline(ref M, element.border_bottom_style, ref x_firstline, ref y, ref dotBefore);
        //                            }
        //                            x_firstline = x_firstline + 3;


        //                        }
        //                    }
        //                    #endregion firstline

        //                    //not first line anymore -> no text_indent
        //                    #region notfirstline
        //                    else
        //                    {
        //                        //line break needed? -> add new Lines, set y + 4+lineheight
        //                        if (x + (chars.Length - 1) * 3 > x_max)
        //                        {
        //                            if ((element.height > 0) && (y + lineheigt >= height))   //If set height not reached, continue drawing
        //                            {
        //                                heightLock = true;
        //                                break;
        //                            }

        //                            //Draw new line normally
        //                            else
        //                            {
        //                                for (int i = 0; i < lineheigt; i++)
        //                                {
        //                                    M.Add(RTBrailleRendererHelper.newLine(maxwidth));
        //                                }

        //                                //Set y for the next row of braille symbols and reset x to x-coord after the BoxModel applied
        //                                y = y + lineheigt;
        //                                x = x_afterBox;

        //                                for (int j = 1; j < chars.Length; j++)
        //                                {
        //                                    chars[j] = chars[j].Insert(0, "\\");
        //                                    bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
        //                                    M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x, ref y);
        //                                    if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
        //                                    {
        //                                        Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
        //                                    }
        //                                    x = x + 3;
        //                                }

        //                                if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
        //                                {
        //                                    Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
        //                                }
        //                                x = x + 3;
        //                                firstline = false;
        //                            }
        //                        }

        //                            //No line break needed yet, draw normally
        //                        else
        //                        {
        //                            if ((element.height > 0) && (y + 1 > height))
        //                            {
        //                                heightLock = true;
        //                                break;
        //                            }
        //                            else
        //                            {

        //                                for (int j = 1; j < chars.Length; j++)
        //                                {
        //                                    chars[j] = chars[j].Insert(0, "\\");
        //                                    bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
        //                                    M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x, ref y);
        //                                    if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
        //                                    {
        //                                        Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
        //                                    }
        //                                    x = x + 3;

        //                                }

        //                                if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
        //                                {
        //                                    Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
        //                                }
        //                                x = x + 3;
        //                            }

        //                        }

        //                    }
        //                    #endregion notfirstline


        //                }

        //                //

        //            } //End for word in words

        //            if (element.selector == "li")
        //            {
        //                x = x_afterBox;
        //                y = y + lineheigt;
        //                for (int i = 0; i < lineheigt; i++)
        //                {
        //                    M.Add(RTBrailleRendererHelper.newLine(maxwidth));
        //                }
        //            }
        //            #endregion drawWords
        //        }
        //    }
        //    #endregion IterateNodes


        //    uint boxBot = padding_bot + element.border.Bottom + margin_bot;
        //    for (int i = 0; i < boxBot; i++)
        //    {
        //        M.Add(RTBrailleRendererHelper.newLine(maxwidth));
        //    }

        //    borderendy = y + lineheigt + padding_bot + element.border.Bottom - 1;
        //    y = y + lineheigt + boxBot;

        //    // am Ende borderendy setzen
        //    if (hasBorder)
        //    {
        //        M = drawBorder(ref M, element.border, borderstartx, borderendx, borderstarty, borderendy);
        //    }

        //    //GANZ am Ende Margin Bot...

        //    //Vor verlassen globales X und Y setzen
        //    coords.CurrentY = y;
        //    return M;

        //}
        ////TODO: zweite general draw anpassen
        ///// <summary>
        ///// General method to start drawing on the display for the child nodes.
        ///// </summary>
        ///// <param name="M">The matrix of the display.</param>
        ///// <param name="element">The HtmlBrailleElement to be drawn.</param>
        ///// <param name="rules">The css rules.</param>
        ///// <param name="parentx">The current x of the parent.</param>
        ///// <param name="parenty">The current y of the parent.</param>
        ///// <param name="parentx_max">The current x_max of the parent.</param>
        ///// <returns></returns>
        //public List<bool[]> GeneralDraw(ref List<bool[]> M, HtmlBrailleElement element, Dictionary<String, Dictionary<String, Object>> rules, ref uint parentx, ref uint parenty, ref uint parentx_max, ref uint parentx_min)
        //{

        //    #region initializeVariables
        //    uint x = parentx;
        //    uint y = parenty;
        //    uint x_max;
        //    uint x_afterBox = parentx_min;

        //    if (element.width > parentx_max || element.width == 0)
        //    {
        //        x_max = (parentx_max - 1);
        //    }
        //    else
        //    {
        //        x_max = element.width - 1;
        //    }

        //    uint margin_bot = 0;
        //    uint padding_bot = 0;
        //    uint x_firstline = x;
        //    bool firstline = true;

        //    uint lineheigt = DEFAULT_LINE_HEIGHT; //standard line-height with 4 px for braille and 1 space
        //    bool heightLock = false;
        //    uint height = 0; //calculated hight with BoxModel


        //    //Border wird am Ende erst gezeichnet, deshalb Werte zwischenspeichern
        //    bool hasBorder = false;
        //    uint borderstartx = 0;
        //    uint borderendx = 0;
        //    uint borderstarty = 0;
        //    uint borderendy = 0;

        //    bool hasUnderlining = false;
        //    bool isDoubleUnderline = false;
        //    bool dotBefore = false;
        //    #endregion initializeVariables


        //    #region margin
        //    // if(!(element.margin.hasBox()))
        //    if (!(element.margin.Equals(null)))
        //    {
        //        //Verschieben des aktuellen x/y Wertes und des max x Wertes um die marginpixel
        //        x = x + element.margin.Left;
        //        x_afterBox = x_afterBox + element.margin.Left;
        //        y = y + element.margin.Top;
        //        x_max = x_max - element.margin.Right;
        //        margin_bot = element.margin.Bottom;
        //    }
        //    #endregion margin

        //    #region border
        //    if (!(element.border.Equals(null)))
        //    {
        //        //Zwischenspeichern für Zeichnen des Borders am Ende
        //        hasBorder = true;
        //        borderstartx = x;
        //        borderendx = x_max;
        //        borderstarty = y;

        //        //Verschieben des aktuellen 
        //        x = x + element.border.Left;
        //        x_afterBox = x_afterBox + element.border.Left;
        //        x_max = x_max - element.border.Right;
        //        y = y + element.border.Top;

        //    }
        //    #endregion border

        //    #region padding
        //    if (!(element.padding.Equals(null)))
        //    {
        //        x = x + element.padding.Left;
        //        x_afterBox = x_afterBox + element.padding.Left;
        //        x_max = x_max - element.padding.Right;
        //        y = y + element.padding.Top;
        //        padding_bot = element.padding.Bottom;

        //    }
        //    #endregion padding

        //    #region text_indent
        //    if (element.text_indent < 0)
        //    {
        //        uint temp = 0;

        //        temp = Convert.ToUInt32(Math.Abs(element.text_indent));
        //        if (x - temp >= 0)
        //        {
        //            x_firstline = x - temp;
        //        }
        //    }
        //    else
        //    {
        //        x_firstline = x + Convert.ToUInt32(element.text_indent);
        //    }
        //    #endregion text_indent

        //    #region border-bottom-style
        //    if (!(element.border_bottom_style == null))
        //    {
        //        hasUnderlining = true;
        //        if (element.border_bottom_style == "double")
        //            isDoubleUnderline = true;

        //    }

        //    #endregion border-bottom-style

        //    #region line-height
        //    lineheigt = element.lineheight;
        //    if ((lineheigt < 5 && hasUnderlining) || (lineheigt < 6 && isDoubleUnderline))
        //    {
        //        lineheigt = 6;
        //    }
        //    #endregion line-height

        //    #region height
        //    //TODO: check this
        //    if (!(element.height.Equals(null)))
        //    {
        //        height = parenty + element.margin.Top + element.border.Top + element.padding.Top + (element.lineheight) * element.height;
        //    }
        //    #endregion height



        //    //TODO: check this
        //    //Create Lines for BoxModel and first row of braille with specified lineheight
        //    for (int i = 0; i < (y - parenty) + lineheigt; i++)
        //    {
        //        M.Add(RTBrailleRendererHelper.newLine(maxwidth));
        //    }


        //    //Iterate through nodeList
        //    #region IterateNodes
        //    for (int nodeNumber = 0; nodeNumber < element.nodelist.Length; nodeNumber++)
        //    {
        //        string temptext = null;

        //        if (firstline && !heightLock)
        //        {
        //            checkNode(element.nodelist[nodeNumber], ref M, rules, ref x_firstline, ref y, ref x_max, ref x_afterBox, ref temptext);
        //            element.text = temptext;
        //        }
        //        else if (!heightLock)
        //        {
        //            checkNode(element.nodelist[nodeNumber], ref M, rules, ref x, ref y, ref x_max, ref x_afterBox, ref temptext);
        //            element.text = temptext;
        //        }
        //        if (!String.IsNullOrEmpty(element.text))
        //        {
        //            element.text = element.text = SplitString(command, param, element.text);


        //            string[] words = element.text.Split(' ');

        //            #region before
        //            //if (!(element.before.Equals(null)))
        //            if (!(element.before == null))
        //            {
        //                switch (element.selector)
        //                {
        //                    //TODO: why?
        //                    // Insert before every word
        //                    case "b":
        //                        for (int i = 0; i < words.Length; i++)
        //                        {
        //                            words[i] = words[i].Insert(0, element.before + "\\2800");
        //                        }
        //                        break;
        //                    // Insert only at the beginning
        //                    case "u":
        //                        words[0] = words[0].Insert(0, element.before + "\\2800");
        //                        break;
        //                    case "li":
        //                        words[0] = words[0].Insert(0, element.before + "\\2800");
        //                        break;
        //                }
        //            }
        //            #endregion before

        //            #region after
        //            if (!(element.after == null))
        //            {
        //                switch (element.selector)
        //                {
        //                    // Insert after every word
        //                    //TODO: really?
        //                    case "b":
        //                        for (int i = 0; i < words.Length; i++)
        //                        {
        //                            words[i] = words[i].Insert(words[i].Length, element.after);
        //                        }
        //                        break;
        //                    // Insert only at the end of last word
        //                    case "u":
        //                        words[words.Length - 1] = words[words.Length - 1].Insert(words[words.Length - 1].Length, element.after);
        //                        break;
        //                }
        //            }
        //            #endregion after

        //            #region drawWords

        //            for (int wordindex = 0; wordindex < words.Length; wordindex++)
        //            {
        //                //Insert before elements

        //                //Check if set height is already reached for next line -> can't be drawn
        //                if (!heightLock && !String.IsNullOrEmpty(words[wordindex]))
        //                {
        //                    string[] chars = words[wordindex].Split('\\');
        //                    // first line? -> text indent!
        //                    #region firstline
        //                    if (firstline)
        //                    {
        //                        //line break needed? -> add new Lines, set y + 4+lineheight
        //                        if (x_firstline + (chars.Length - 1) * 3 > x_max)
        //                        {
        //                            if ((element.height > 0) && (y + lineheigt >= height))   //If set height not reached, continue drawing
        //                            {
        //                                heightLock = true;
        //                                break;
        //                            }

        //                            //If set height not reached, continue drawing
        //                            else
        //                            {

        //                                for (int i = 0; i < lineheigt; i++)
        //                                {
        //                                    M.Add(RTBrailleRendererHelper.newLine(maxwidth));
        //                                }

        //                                //Set y for the next row of braille symbols
        //                                y = y + lineheigt;


        //                                x = x_afterBox;


        //                                //continue writing with fresh line
        //                                for (int j = 1; j < chars.Length; j++)
        //                                {
        //                                    chars[j] = chars[j].Insert(0, "\\");
        //                                    bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
        //                                    M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x, ref y);
        //                                    if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
        //                                    {
        //                                        Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
        //                                    }
        //                                    x = x + 3;

        //                                }
        //                                if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
        //                                {
        //                                    Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
        //                                }
        //                                x = x + 3;
        //                                firstline = false;
        //                            }
        //                        }
        //                        else

        //                        //No line break needed yet
        //                        {


        //                            for (int j = 1; j < chars.Length; j++)
        //                            {
        //                                chars[j] = chars[j].Insert(0, "\\");
        //                                bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
        //                                M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x_firstline, ref y);
        //                                if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
        //                                {
        //                                    Underline(ref M, element.border_bottom_style, ref x_firstline, ref y, ref dotBefore);
        //                                }
        //                                x_firstline = x_firstline + 3;

        //                            }
        //                            if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
        //                            {
        //                                Underline(ref M, element.border_bottom_style, ref x_firstline, ref y, ref dotBefore);
        //                            }
        //                            x_firstline = x_firstline + 3;

        //                        }
        //                    }
        //                    #endregion firstline

        //                    //not first line anymore -> no text_indent
        //                    #region notfirstline
        //                    else
        //                    {
        //                        //line break needed? -> add new Lines, set y + 4+lineheight
        //                        if (x + (chars.Length - 1) * 3 > x_max)
        //                        {
        //                            //If current position +1 is bigger then Top BoxModell plus the specified lineheight times (4 Rows for braille sign plus lineheight)
        //                            if ((element.height > 0) && (y + 1 > height))
        //                            {
        //                                heightLock = true;
        //                                break;
        //                            }

        //                            //Draw new line normally
        //                            else
        //                            {
        //                                for (int i = 0; i < lineheigt; i++)
        //                                {
        //                                    M.Add(RTBrailleRendererHelper.newLine(maxwidth));
        //                                }

        //                                //Set y for the next row of braille symbols and reset x to x-coord after the BoxModel applied
        //                                y = y + lineheigt;
        //                                x = x_afterBox;

        //                                for (int j = 1; j < chars.Length; j++)
        //                                {
        //                                    chars[j] = chars[j].Insert(0, "\\");
        //                                    bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
        //                                    M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x, ref y);
        //                                    if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
        //                                    {
        //                                        Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
        //                                    }
        //                                    x = x + 3;
        //                                }

        //                                if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
        //                                {
        //                                    Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
        //                                }
        //                                x = x + 3;
        //                                firstline = false;
        //                            }
        //                        }

        //                            //No line break needed yet, draw normally
        //                        else
        //                        {


        //                            for (int j = 1; j < chars.Length; j++)
        //                            {
        //                                chars[j] = chars[j].Insert(0, "\\");
        //                                bool[,] onechar = RTBrailleRendererHelper.unicode_to_matrix(chars[j]);
        //                                M = RTBrailleRendererHelper.putcharinmatrix(ref M, onechar, ref x, ref y);
        //                                if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)))
        //                                {
        //                                    Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
        //                                }
        //                                x = x + 3;

        //                            }

        //                            if (hasUnderlining && (((lineheigt - 4 > 0) && !isDoubleUnderline) || (lineheigt - 4 > 1)) && (wordindex < words.Length - 1))
        //                            {
        //                                Underline(ref M, element.border_bottom_style, ref x, ref y, ref dotBefore);
        //                            }
        //                            x = x + 3;

        //                        }

        //                    }
        //                    #endregion notfirstline

        //                }

        //                //

        //            } //End for word in words
        //            if (element.selector == "li")
        //            {
        //                x = x_afterBox;
        //                y = y + lineheigt;
        //                for (int i = 0; i < lineheigt; i++)
        //                {
        //                    M.Add(RTBrailleRendererHelper.newLine(maxwidth));
        //                }

        //            }
        //            else
        //            {
        //                if (firstline)
        //                    parentx = x_firstline;
        //                else
        //                    parentx = x;

        //            }

        //            //delete list test

        //            #endregion drawWords
        //        }
        //    }
        //    #endregion IterateNodes

        //    //delete
        //    parenty = y;

        //    uint boxBot = padding_bot + element.border.Bottom + margin_bot;
        //    for (int i = 0; i < boxBot; i++)
        //    {
        //        M.Add(RTBrailleRendererHelper.newLine(maxwidth));
        //    }


        //    borderendy = y + lineheigt + padding_bot + element.border.Bottom - 1;
        //    y = y + lineheigt + boxBot;

        //    // am Ende borderendy setzen
        //    if (hasBorder)
        //    {
        //        M = drawBorder(ref M, element.border, borderstartx, borderendx, borderstarty, borderendy);
        //    }

        //    //GANZ am Ende Margin Bot...

        //    //Vor verlassen globales X und Y setzen
        //    //coords.CurrentY = y;
        //    return M;

        //}


    }
}
