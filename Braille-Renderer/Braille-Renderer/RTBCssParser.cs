using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace tud.mci.tangram.Braille_Renderer
{
    static class RTBCssParser
    {

        public static Dictionary<String, List<String>> CssToDictionary(String cssfile)
        {
            Dictionary<String, List<String>> rules = new Dictionary<String, List<String>>();
            string cssstring = File.ReadAllText(cssfile);
            cssstring = cssstring.Replace("\r\n", "");
            char delimiter = '}';
            string[] cssdefinitions = cssstring.Split(delimiter);
            //cssdefinitions contains one or more rules for a hhtml selector (h1, b, etc)

            foreach (string cssdefinition in cssdefinitions)
            {

                if (!(cssdefinition.Equals("")))
                {
                    string[] name_features = cssdefinition.Split('{');
                    string defname = name_features[0].ToLowerInvariant();
                    string[] features = name_features[1].Split(';');
                    List<string> featureList = features.ToList();
                    featureList.Remove("");

                    if (defname.Contains(","))
                    {
                        string[] multidef = defname.Split(',');
                        foreach (string singledef in multidef)
                        {
                            if (singledef.Contains(':'))
                            {
                                string[] pseudo = singledef.Split(':');
                                List<String> newFeatureList = pseudoElementaufl(pseudo[0], pseudo[1], featureList);
                                rules.Add(pseudo[0], newFeatureList);
                            }
                            else
                            {
                                if (rules.ContainsKey(singledef))
                                {
                                    rules[singledef].AddRange(featureList);
                                }
                                else
                                {
                                    rules.Add(singledef, featureList);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (defname.Contains(':'))
                        {
                            string[] pseudo = defname.Split(':');
                            string name = pseudo[0];
                            List<String> newFeatureList;
                            newFeatureList = pseudoElementaufl(name, pseudo[1], featureList);
                            if (rules.ContainsKey(name))
                            {
                                rules[name].AddRange(newFeatureList);
                            }
                            else
                            {
                                rules.Add(pseudo[0], newFeatureList);
                            }
                        }
                        else
                        {
                            if (rules.ContainsKey(defname))
                            {
                                rules[defname].AddRange(featureList);
                            }
                            else
                            {
                                rules.Add(defname, featureList);
                            }
                        }
                    }
                }
            }
            return rules;

        }

        public static List<String> pseudoElementaufl(string defname, string pseudoelement, List<String> featureList)
        {
            List<String> newFeatureList = new List<String>();
            foreach (string feature in featureList)
            {

                if (feature.StartsWith("content:"))
                {
                    newFeatureList.Add(feature.Insert(0, pseudoelement + "|"));
                }
                else
                {
                    newFeatureList.Add(feature);
                }
            }
            return newFeatureList;
        }


        /******************************* JB ********************************/

        //public static Dictionary<String, Dictionary<String, Object>> CssToDictionary(String cssfile)
        //{
        //    Dictionary<String, Dictionary<String, Object>> cRules = new Dictionary<String, Dictionary<String, Object>>();
            
        //    string cssstring = "";
        //    if (File.Exists(cssfile)) { cssstring = File.ReadAllText(cssfile); }
        //    else { System.Diagnostics.Debug.WriteLine("ERROR: CSS FILE COULD NOT BEEN LOADED: " + cssfile); cssstring = cssfile; }
        //    cssstring = cssstring.Replace("\r\n", "");

        //    char delimiter = '}';
        //    string[] cssdefinitions = cssstring.Split(delimiter);

        //    foreach (string cssdefinition in cssdefinitions)
        //    {
        //        if (!String.IsNullOrWhiteSpace(cssdefinition))
        //        {
        //            string[] name_features = cssdefinition.Split('{');
        //            string defname = name_features[0].ToUpperInvariant();
        //            string[] features = name_features[1].Split(';');
        //            List<string> featureList = features.ToList();
        //            featureList.Remove("");

        //            Dictionary<String, Object> featureObjectList = getFeatureObjectList(featureList);

        //            if (defname.Contains(","))
        //            {
        //                string[] multidef = defname.Split(',');
        //                foreach (string singledef in multidef)
        //                {
        //                    if (singledef.Contains(':'))
        //                    {
        //                        string[] pseudo = singledef.Split(':');
        //                        Dictionary<String, Object> newFeatureObjectList = pseudoElementaufl(pseudo[0], pseudo[1], featureObjectList);
        //                        cRules.Add(pseudo[0], newFeatureObjectList);
        //                    }
        //                    else
        //                    {
        //                        if (cRules.ContainsKey(singledef))
        //                        {
        //                            cRules[singledef] = MergeLeft(cRules[singledef], featureObjectList);
        //                        }
        //                        else
        //                        {
        //                            cRules.Add(singledef, featureObjectList);
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (defname.Contains(':'))
        //                {
        //                    string[] pseudo = defname.Split(':');
        //                    string name = pseudo[0];
        //                    Dictionary<String, Object> newFeatureObjectList = pseudoElementaufl(pseudo[0], pseudo[1], featureObjectList);

        //                    if (cRules.ContainsKey(name))
        //                    {
        //                        cRules[name] = MergeLeft(cRules[name], newFeatureObjectList);
        //                    }
        //                    else
        //                    {
        //                        cRules.Add(pseudo[0], newFeatureObjectList);
        //                    }
        //                }
        //                else
        //                {
        //                    if (cRules.ContainsKey(defname))
        //                    {
        //                        cRules[defname] = MergeLeft(cRules[defname], featureObjectList);
        //                    }
        //                    else
        //                    {
        //                        cRules.Add(defname, featureObjectList);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return cRules;
        //}

        //private static Dictionary<string, object> getFeatureObjectList(List<string> featureList)
        //{
        //    Dictionary<String, Object> featureObjectList = new Dictionary<String, Object>();
        //    if (featureList != null && featureList.Count > 0)
        //    {
        //        uint lineheight = RTBrailleRenderer.DEFAULT_LINE_HEIGHT;

        //        #region Lineheight

        //        foreach (var feature in featureList)
        //        {
        //            if (!String.IsNullOrWhiteSpace(feature) && feature.StartsWith("line-height"))
        //            {
        //                string[] feature_value = feature.Split(':');
        //                if (feature_value.Length > 1)
        //                {
        //                    string value = feature_value[1];
        //                    if (!String.IsNullOrWhiteSpace(value))
        //                    {
        //                        lineheight = RTBrailleRendererHelper.ConvertUnitsVert(value, lineheight);
        //                        featureObjectList.Add("line-height", lineheight);
        //                        if (lineheight == 0)
        //                        {
        //                            featureObjectList.Add("display", "none");
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        #endregion

        //        foreach (var feature in featureList)
        //        {
        //            if (!String.IsNullOrWhiteSpace(feature))
        //            {
        //                string[] feature_value = feature.Split(':');
        //                string name = feature_value[0].ToLowerInvariant();
        //                string value = feature_value[1].Trim();

        //                string[] multivalues = value.Split(' ');
        //                int size = multivalues.Length;
        //                object val = null;

        //                switch (name)
        //                {
        //                    #region margin
        //                    case "margin":
        //                        BoxModel margin;
        //                        switch (size)
        //                        {
        //                            case 1:
        //                                margin = new BoxModel(
        //                                    RTBrailleRendererHelper.ConvertUnitsVert(value, lineheight),
        //                                    RTBrailleRendererHelper.ConvertUnitsHor(value),
        //                                    RTBrailleRendererHelper.ConvertUnitsVert(value, lineheight),
        //                                    RTBrailleRendererHelper.ConvertUnitsHor(value));
        //                                break;
        //                            case 2:
        //                                margin = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]));
        //                                break;
        //                            case 3:
        //                                margin = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], lineheight));
        //                                break;
        //                            case 4:
        //                                margin = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[3]));
        //                                break;
        //                            default:
        //                                margin = new BoxModel(0);
        //                                break;
        //                        }
        //                        val = margin;
        //                        break;
        //                    #endregion margin

        //                    #region border-width
        //                    case "border-width":
        //                        uint[] borderpixel = new uint[size];
        //                        BoxModel border = new BoxModel(0); ;
        //                        switch (size)
        //                        {
        //                            case 1:
        //                                border = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(value, lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value), RTBrailleRendererHelper.ConvertUnitsVert(value, lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value));
        //                                break;
        //                            case 2:
        //                                border = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]));
        //                                break;
        //                            case 3:
        //                                border = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], lineheight));
        //                                break;
        //                            case 4:
        //                                border = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[3]));
        //                                break;
        //                            default:
        //                                break;
        //                        }
        //                        val = border;
        //                        break;
        //                    #endregion border

        //                    #region border-left-width
        //                    case "border-left-width":
        //                        border.Left = RTBrailleRendererHelper.ConvertUnitsVert(value, lineheight);
        //                        break;
        //                    #endregion border-left-width

        //                    #region border-right-width
        //                    case "border-right-width":
        //                        border.Right = RTBrailleRendererHelper.ConvertUnitsVert(value, lineheight);
        //                        break;
        //                    #endregion border-right-width

        //                    #region border-top-width
        //                    case "border-top-width":
        //                        border.Top = RTBrailleRendererHelper.ConvertUnitsVert(value, lineheight);
        //                        break;
        //                    #endregion border-top-width

        //                    #region border-bottom-width
        //                    case "border-bottom-width":
        //                        border.Bottom = RTBrailleRendererHelper.ConvertUnitsVert(value, lineheight);
        //                        break;
        //                    #endregion border-bottom-width

        //                    #region border-bottom-style
        //                    case "border-bottom-style":
        //                        val = value;
        //                        break;
        //                    #endregion border-bottom-style

        //                    #region padding
        //                    case "padding":
        //                        BoxModel padding;
        //                        switch (size)
        //                        {
        //                            case 1:
        //                                padding = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(value, lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value), RTBrailleRendererHelper.ConvertUnitsVert(value, lineheight), RTBrailleRendererHelper.ConvertUnitsHor(value));
        //                                break;
        //                            case 2:
        //                                padding = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]));
        //                                break;
        //                            case 3:
        //                                padding = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], lineheight));
        //                                break;
        //                            case 4:
        //                                padding = new BoxModel(RTBrailleRendererHelper.ConvertUnitsVert(multivalues[0], lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[1]), RTBrailleRendererHelper.ConvertUnitsVert(multivalues[2], lineheight), RTBrailleRendererHelper.ConvertUnitsHor(multivalues[3]));
        //                                break;
        //                            default:
        //                                padding = new BoxModel(0);
        //                                break;
        //                        }
        //                        val = padding;
        //                        break;
        //                    #endregion padding

        //                    #region text-indent
        //                    case "text-indent":
        //                        val = Convert.ToInt32(RTBrailleRendererHelper.ConvertUnitsInt(value));
        //                        break;
        //                    #endregion text-indent

        //                    #region height
        //                    case "height":
        //                        val = RTBrailleRendererHelper.ConvertUnitsVert(value, lineheight);
        //                        if (val.Equals(0)) { featureObjectList.Add("display", "none"); }
        //                        break;
        //                    #endregion height

        //                    #region width
        //                    case "width":
        //                        val = RTBrailleRendererHelper.ConvertUnitsHor(value);
        //                        break;
        //                    #endregion width

        //                    #region display
        //                    case "display":
        //                        if (value == "none")
        //                        {
        //                            val = true;
        //                        }
        //                        else
        //                        {
        //                            val = false;
        //                        }
        //                        break;
        //                    #endregion display

        //                    case "line-height": break;
                                
        //                    default:
        //                        val = value.Trim();
        //                        break;
        //                }

        //                if (val != null) {
        //                    if (featureObjectList.ContainsKey(name)) featureObjectList.Remove(name);
        //                    featureObjectList.Add(name, val);
        //                }
        //                else
        //                {
        //                    //System.Diagnostics.Debug.WriteLine(name + " does not have value");
        //                }
        //            }
        //        }


        //    }
        //    return featureObjectList;
        //}

        //public static Dictionary<String, Object> pseudoElementaufl(string defname, string pseudoelement, Dictionary<String, Object> featureList)
        //{
        //    Dictionary<String, Object> newFeatureList = new Dictionary<String, Object>();
            
        //    foreach (String feature in featureList.Keys)
        //    {
        //        if (feature.Equals("content"))
        //        {
        //            newFeatureList.Add(pseudoelement.ToLowerInvariant(), featureList[feature]);
        //        }
        //        else
        //        {
        //            newFeatureList.Add(feature, featureList[feature]);
        //        }
        //    }
        //    return newFeatureList;
        //}

        // Works in C#3/VS2008:
        // Returns a new dictionary of this ... others merged leftward.
        // Keeps the type of 'this', which must be default-instantiable.
        // Example: 
        //   result = map.MergeLeft(other1, other2, ...)
        public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
            where T : IDictionary<K, V>, new()
        {
            T newMap = new T();
            foreach (IDictionary<K, V> src in
                (new List<IDictionary<K, V>> { me }).Concat(others))
            {
                // ^-- echk. Not quite there type-system.
                foreach (KeyValuePair<K, V> p in src)
                {
                    newMap[p.Key] = p.Value;
                }
            }
            return newMap;
        }
    }
}
