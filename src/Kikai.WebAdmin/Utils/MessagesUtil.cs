using Kikai.WebAdmin.IUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;
using System.Xml;

namespace Kikai.WebAdmin.Utils
{
    public class MessagesUtil : IMessagesUtil
    {
        public string GetMessage(string ErrorKey)
        {
            XmlNode errorNode = null;
            ObjectCache cache = MemoryCache.Default;
            XmlDocument XmlMessages = cache["UIXML"] as XmlDocument;
            try
            {
                if (XmlMessages == null)
                {
                    XmlMessages = new XmlDocument();
                    var fileName = AppDomain.CurrentDomain.BaseDirectory + "bin\\Resource\\UIstrings.xml";
                    XmlMessages.Load(fileName);
                    cache["UIXML"] = XmlMessages;
                }
                string nodePath = String.Format("/messages/message[@id='{0}']", ErrorKey);
                errorNode = XmlMessages.DocumentElement.SelectNodes(nodePath).Item(0);
            }
            catch
            {
                throw;
            }
            return errorNode.Attributes["Message"].Value;
        }
    }
}