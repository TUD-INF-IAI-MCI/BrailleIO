
using System;
using System.Collections.Generic;
using Gestures.Recognition;
using Gestures.Recognition.GestureData;
using Gestures.Recognition.Interfaces;
using Gestures.Geometrie.Vertex;
using Gestures.InOut;
using System.IO;

namespace Gestures.InOut
{  
    public class StandardPaths
    {
        public static String standardXmlTemplatePath = System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar+ "GestureTemplates";
        public static String dollar1TemplatePath = System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "XmlTemplates" + System.IO.Path.DirectorySeparatorChar;
        public static String dollar1TempPath = standardXmlTemplatePath + System.IO.Path.DirectorySeparatorChar+ "TempXmlTemplates" + System.IO.Path.DirectorySeparatorChar;
    }

    /// <summary>
    /// Provides methods for loading and storing gesture templates.
    /// </summary>
		/// <remarks> </remarks>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    public class StandardTemplateLoader : ILoadGestureTemplates
    {
        #region Fields
        protected IDictionary<String, GestureClass> templates;
        private ITemplateReaderWriter templateReaderWriter = new TemplateReaderWriter();
        #endregion

        #region Constructors
        public StandardTemplateLoader()
        {
            templates = new Dictionary<String, GestureClass>();
        }
        #endregion

        #region ILoadTemplateGestures Members

        public virtual void LoadTemplates(string xmlTemplatePath)
        {
            templates.Clear();
            if (xmlTemplatePath == null || xmlTemplatePath == String.Empty)
            {
                xmlTemplatePath = StandardPaths.standardXmlTemplatePath;
            }
            if (System.IO.Directory.Exists(xmlTemplatePath))
            {
                var fileList = new List<String>();
                foreach (String file in System.IO.Directory.GetFiles(xmlTemplatePath, "*.xml",
                    System.IO.SearchOption.AllDirectories))
                {
                    fileList.Add(file);
                }
                fileList.Sort(new Comparison<string>(delegate(string str1, string str2)
                {
                    if (str1 == null && str2 == null)
                    { return 0; }
                    else if (str1 == null)
                    { return -1; }
                    else if (str2 == null)
                    {return 1;}
                    return System.IO.File.GetCreationTime(str1) < System.IO.File.GetCreationTime(str2) ? -1 : 1;
                }));
            
                foreach (String file in fileList)
                {
                    GestureTemplate gestureTemplate =
                        templateReaderWriter.GetGestureFromXmlTemplate(
                        templateReaderWriter.LoadXmlTemplate(file));

                    if (gestureTemplate != null)
                    {
                        var name = Path.GetFileNameWithoutExtension(file);
                        var num = name.Substring(name.LastIndexOf("_"));
                        int templateNum = 0;
                        bool store = false;

                        if (String.IsNullOrWhiteSpace(gestureTemplate.ClassName))
                        {                            
                            var cname = name.Substring(0, name.LastIndexOf("_"));
                            gestureTemplate.ClassName = cname;
                            store = true;
                        }
                        if (!String.IsNullOrWhiteSpace(num))
                        {
                            bool succ = Int32.TryParse(num, out templateNum);
                            if (succ && templateNum != gestureTemplate.TemplateNumber)
                            {
                                gestureTemplate.TemplateNumber = templateNum;
                                store = true;
                            }
                        }
                        if(store)
                        {
                            try { this.StoreTemplate(Path.GetDirectoryName(file), Path.GetFileName(file), gestureTemplate); }
                            catch { }
                        }

                        AddTemplate(gestureTemplate);
                    }
                }
            }

            else
            {
                System.IO.Directory.CreateDirectory(xmlTemplatePath);

            }
        }

        public virtual GestureClass[] GetGestureClasses()
        {
            GestureClass[] result = new GestureClass[templates.Count];
            int i = 0;
            foreach (KeyValuePair<string, GestureClass> template in templates)
            {
                result[i++] = template.Value;
            }
            return result;
        }

        public virtual void AddTemplate(GestureTemplate gestureTemplate)
        {
            #region standard loading procedure
            if (gestureTemplate.ClassName == null) 
            { 
                //gestureTemplate.ClassName = templates.Count.ToString(); 
                throw new ArgumentException("gesture template doesn't contain class name");
            }
            if (!templates.ContainsKey(gestureTemplate.ClassName))
            {
                GestureClass gestureClass = new GestureClass();
                gestureClass.ClassName = gestureTemplate.ClassName;
               // gestureTemplate.TemplateNumber = 0;
                gestureClass.Add(gestureTemplate);
                templates.Add(gestureClass.ClassName, gestureClass);
            }
            else
            {
                //TODO: this avoids adding of templates under same class name as templates
                //that differ in token count -> remove in final version or parameterize
//                templates[gestureTemplate.ClassName][0].Count
                //&& templates[gestureTemplate.ClassName].Count<11
                //if (3 == gestureTemplate.Count )
                {
                 //   gestureTemplate.TemplateNumber = templates[gestureTemplate.ClassName].Count;
                    templates[gestureTemplate.ClassName].Add(gestureTemplate);
                }
                //else
                //{
                //    Console.WriteLine("Template contained wrong number of tokens");
                //    System.Diagnostics.Process.GetCurrentProcess().Kill();
                //}
            }
            #endregion
        }

        public virtual void StoreTemplates(string templatePath)
        {
            foreach (GestureClass gestureClass in templates.Values)
            {
                for (int i = 0; i < gestureClass.Count; i++)
                {
                    StoreTemplate(templatePath,gestureClass.ClassName+"_"+i.ToString()+".xml", gestureClass[i]);
                }
            }
        }

        public virtual void StoreTemplate(string templatePath, String templateFileName, GestureTemplate template)
        {
            if (templatePath == null || templatePath == String.Empty)
            {
                templatePath = StandardPaths.standardXmlTemplatePath;
            }
            if (!System.IO.Directory.Exists(templatePath))
            {
                System.IO.Directory.CreateDirectory(templatePath);
            }            
            templateReaderWriter.StoreTemplate(
                templateReaderWriter.GetXmlTemplateFromGesture(template),
                templatePath + System.IO.Path.DirectorySeparatorChar + templateFileName);
        }

        public virtual void StoreTemplate(string templatePath, GestureTemplate template)
        {
            int i = 0;
            String fileName = template.ClassName + "_" + i.ToString() + ".xml";
            String file = templatePath + System.IO.Path.DirectorySeparatorChar + fileName;
            while (System.IO.File.Exists(file))
            {
                i++;
                fileName = template.ClassName + "_" + i.ToString() + ".xml";
                file = templatePath + System.IO.Path.DirectorySeparatorChar + fileName;
            }
            StoreTemplate(templatePath, fileName, template);
        }

        public virtual void RemoveTemplates(string className)
        {
            if (this.templates.ContainsKey(className))
            {
                this.templates.Remove(className);
            }
        }
        #endregion

        public virtual void LoadTemplates(GestureClass[] templates)
        {
            this.templates.Clear();
            if (templates != null)
            {
                foreach (var gestureClass in templates)
                {
                    for (int i=0;i<gestureClass.Count;i++)
                    {
                        AddTemplate(gestureClass[i]);
                    }
                }
            }
        }
    }
}
