using Kikai.Logging.DTO;
using Kikai.Logging.Utils.IUtils;
using System;
using System.Xml;
using System.Runtime.Caching;
using System.Configuration;

namespace Kikai.Logging.Utils
{
    public class ErrorUtil : IErrorUtil
    {
        private XmlDocument XmlMessages;

        /// <summary>
        /// Function that returns an error code with it's appropriate values
        /// </summary>
        /// <param name="key"></param>
        /// <returns>ErrorObject</returns>
        public ErrorObject GetError(int ErrorId)
        {
            ErrorObject ErrorObject = new ErrorObject();
            ObjectCache cache = MemoryCache.Default;
            XmlMessages = cache["MessagesXML"] as XmlDocument;
            try
            {
                if (XmlMessages == null)
                {
                    XmlMessages = new XmlDocument();
                    var fileName = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["ResourcePathForErrorMsgs"];
                    XmlMessages.Load(fileName);
                    cache["MessagesXML"] = XmlMessages;
                }
                string nodePath = String.Format("/errors/error[@id='{0}']", ErrorId);
                XmlNode errorNode = XmlMessages.DocumentElement.SelectNodes(nodePath).Item(0);
                ErrorObject = new ErrorObject(errorNode.Attributes["id"].Value, errorNode.Attributes["Key"].Value, errorNode.Attributes["Code"].Value, errorNode.Attributes["Message"].Value);
            }
            catch
            {
                throw;
            }
            return ErrorObject;
        }
    }
}
