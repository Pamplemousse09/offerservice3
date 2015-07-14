using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kikai.WebAdmin.HtmlHelper
{
    public class CustomUrlHelper
    {
        public static string GetUrl()
        {
            var requestContext = HttpContext.Current.Request.RequestContext;
            var baseurl = new System.Web.Mvc.UrlHelper(requestContext).Action("Index", "Home");
            baseurl = baseurl.Replace("/Home/Index/", "");
            baseurl = baseurl.TrimEnd('/');
            return baseurl;
        }
    }
}