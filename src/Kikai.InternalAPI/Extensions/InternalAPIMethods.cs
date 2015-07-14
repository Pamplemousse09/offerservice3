using System.Linq;
using System.Net.Http;
namespace Kikai.InternalAPI.Extensions
{
    public class InternalAPIMethods
    {
        #region Methods

        /// <summary>
        /// Function that returns the action name requests from the URL passed
        /// </summary>
        /// <param name="input"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public string FilterControllerAction(HttpRequestMessage Request)
        {
            var actionName = Request.RequestUri.Segments.Last().Replace("/","");
            return actionName.ToLower();
        }

        #endregion
    }
}