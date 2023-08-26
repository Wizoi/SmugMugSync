using System.Xml;
using System.Xml.Linq;

namespace SmugMugCoreSync.Utility;

public class XmlSystem
{
    public virtual void OutputXmlToFile(string targetFilePath, WrappedXElement rootNode)
    {
        using (var xmlWriter = XmlWriter.Create(targetFilePath, new XmlWriterSettings() { Indent = true }))
        {
            rootNode.XElement.WriteTo(xmlWriter);
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

