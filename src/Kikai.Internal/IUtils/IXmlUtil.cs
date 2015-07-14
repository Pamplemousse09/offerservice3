using System.Xml;
using System.Xml.Linq;

namespace Kikai.Internal.IUtils
{
    public interface IXmlUtil
    {
        XDocument ParseXmlDocument(string result);

        XmlDocument LoadXmlDocument(string result);
    }
}
