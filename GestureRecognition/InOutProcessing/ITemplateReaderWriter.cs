using System;
using Gestures.Recognition.GestureData;
namespace Gestures.InOut
{
    public interface ITemplateReaderWriter
    {
        GestureTemplate GetGestureFromXmlTemplate(string xml);
        string GetXmlTemplateFromGesture(GestureTemplate gesture);
        string LoadXmlTemplate(string xmlFilePath);
        void StoreTemplate(string xmlFile, string xmlFilePath);
    }
}
