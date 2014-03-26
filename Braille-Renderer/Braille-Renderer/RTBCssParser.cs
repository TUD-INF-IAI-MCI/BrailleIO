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
                    string defname = name_features[0];
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
                            if(rules.ContainsKey(defname)){
                                rules[defname].AddRange(featureList);
                            }
                            else{
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

    }
}
