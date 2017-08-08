using System.Collections.Generic;
using Gestures.InOut;

using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using Gestures.Recognition.GestureData;

namespace Gestures.InOut
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    /// <seealso cref="Gestures.InOut.ITemplateReaderWriter" />
    public class TemplateReaderWriter : ITemplateReaderWriter
    {
        public virtual String GetXmlTemplateFromGesture(GestureTemplate gesture)
        {
            XmlDocument doc = new XmlDocument();
            StringWriter sw = new StringWriter();
            XmlSerializer xmlSer = new XmlSerializer(typeof(GestureTemplate));

            xmlSer.Serialize(sw, gesture);
            System.Text.StringBuilder sb = sw.GetStringBuilder();

            return sb.ToString();
        }
        public virtual String LoadXmlTemplate(String xmlFilePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);
            return doc.InnerXml;
        }
        public virtual void StoreTemplate(String xmlFile, String xmlFilePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            
            if (File.Exists(xmlFilePath))
            {
                File.Delete(xmlFilePath);
            }
            doc.Save(xmlFilePath);
        }
        public virtual GestureTemplate GetGestureFromXmlTemplate(String xml)
        {
            XmlSerializer xmlSer = new XmlSerializer(typeof(GestureTemplate));
            StringReader sr = new StringReader(xml);
            GestureTemplate gesture = xmlSer.Deserialize(sr) as GestureTemplate;
            return gesture;
        }
    
    }
}