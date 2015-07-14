using Kikai.Internal.IUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Kikai.Internal.Utils
{
    public class XmlUtil : IXmlUtil
    {
        /// <summary>
        /// Function used to parse the XML document
        /// </summary>
        /// <param name="openSampleAttributesResponse"></param>
        /// <returns></returns>
        public XDocument ParseXmlDocument(string result)
        {
            XDocument doc = XDocument.Parse(result);
            return doc;
        }

        /// <summary>
        /// Function used to load the XML document
        /// </summary>
        /// <param name="liveMatchIntenalPIDResponse"></param>
        /// <returns></returns>
        public XmlDocument LoadXmlDocument(string result)
        {
            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(result);
            return xmldoc;
        }


        /// <summary>
        /// This function is used to extract the string value given an XElement
        /// it return empty string if input parameter is null
        /// </summary>
        /// <param name="e"></param>
        /// <returns>string value of the XElement if defined else empty string</returns>
        public static string GetSafeStringNodeValue(XElement e)
        {
            return (e == null) ? "" : (string)e;
        }

        /// <summary>
        /// This function is used to extract the int value given an XElement
        /// it return 0 string if input parameter is null
        /// </summary>
        /// <param name="e"></param>
        /// <returns>int value of the XElement if defined else 0</returns>
        public static int GetSafeIntegerNodeValue(XElement e)
        {
            return (e == null) ? 0 : Convert.ToInt32((double)e);
        }

        /// <summary>
        /// This function is used to extract the float value given an XElement
        /// it return 0 string if input parameter is null
        /// </summary>
        /// <param name="e"></param>
        /// <returns>int value of the XElement if defined else 0</returns>
        public static float GetSafeFloatNodeValue(XElement e)
        {
            return (e == null) ? 0 : (float)e;
        }

        /// <summary>
        /// This function is used to convert the date string to DateTime.
        /// When the string is an invalid date return current datetime.
        /// The exception case is to handle, the SAM service call invalid date strings.
        /// </summary>
        /// <param name="dateString"></param>
        /// <returns>DateTime object for input string</returns>
        public static DateTime? GetSafeDateTime(string dateString)
        {
            DateTime? datetime = DateTime.Now;
            datetime = (dateString == null) ? (DateTime?)null : Convert.ToDateTime(dateString);
            return datetime;
        }
    }
}
