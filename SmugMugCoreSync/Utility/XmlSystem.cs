using System.Xml;
using System.Xml.Linq;

namespace SmugMugCoreSync.Utility;

public class XmlSystem
{
    public virtual void OutputXmlToFile(string targetFilePath, XElement rootNode)
    {
        using (var xmlWriter = XmlWriter.Create(targetFilePath, new XmlWriterSettings() { Indent = true }))
        {
            rootNode.WriteTo(xmlWriter);
        }
    }

    public class WrappedXElement
    {
        public WrappedXElement(XElement xe)
        {
            this.XElement = xe; 
        }

        public XElement XElement { get; private set;}
    }
}

