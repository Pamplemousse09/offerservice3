using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kikai.WebAPI.Test.Init
{
    public class InitCache
    {
        #region Properties
        XmlDocument XmlMessages;
        #endregion

        #region Constructor
        public InitCache()
        {
            XmlMessages = new XmlDocument();
            ObjectCache cache = MemoryCache.Default;
            var fileName = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["ResourcePathForErrorMsgs"];
            XmlMessages.Load(fileName);
            cache["MessagesXML"] = XmlMessages;
        }
        #endregion
    }
}
