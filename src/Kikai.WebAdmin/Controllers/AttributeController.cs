using Kikai.BL.Concrete;
using Kikai.BL.DTO;
using Kikai.Logging.Utils;
using Kikai.WebAdmin.Common;
using Kikai.WebAdmin.DTO;
using Kikai.WebAdmin.Extensions;
using Kikai.WebAdmin.UIModels;
using Kikai.WebAdmin.Utils;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Kikai.WebAdmin.Controllers
{
    [CustomAuthorizeAttribute]
    public class AttributeController : Controller
    {
        private MessagesUtil msgUtil;

        public AttributeController()
        {
            msgUtil = new MessagesUtil();
        }

        //
        // GET: /Attribute/
        public ActionResult Index()
        {
            Session.Remove("ProviderName");
            Session.Remove("StudyId");
            Session.Remove("OfferTitle");
            Session.Remove("OfferStatus");
            Session.Remove("OfferType");
            Session.Remove("Page");
            Session.Remove("Provider_PageSize");
            Session.Remove("OfferPageSize");
            return View();
        }

        /// <summary>
        /// When clicking on the search filters this controller is processed 
        /// </summary>
        /// <param name="searchObject"></param>
        /// <returns>the list of attributes according the search parameters</returns>
        public ActionResult AttributeResult(AttributeSearchObject searchObject)
        {
            //Initializing the UI model
            var attrList = new AttributeModel();
            attrList.PageSize = searchObject.PageSize;
            attrList.Page = searchObject.Page;

            try
            {
                //Searching for the list of desired attributes
                attrList.Attributes = new AttributeRepository().GetFilteredAttributes(searchObject.AttributeId, searchObject.Published, searchObject.Page - 1, searchObject.PageSize).ToList();
            }
            catch(Exception e)
            {
                LoggerFactory.GetLogger().Error(string.Format(msgUtil.GetMessage(MessageKey.LOG_GETATTRIBUTES_EXCEPTION), searchObject.AttributeId, searchObject.Published, searchObject.Page, searchObject.PageSize), e);
                return null;
            }

            //Setting the session variables to save the search query in case the user refreshes the page
            SetSearchSessionVariables(searchObject);

            return PartialView("_AttributeResult", attrList);
        }

        /// <summary>
        /// Publishing or Unpublishing an attribute
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns>The user that updated this attribute</returns>
        public string PutAttribute(string id, bool status)
        {
            //Initializing the AttributeSetting object in order to send it to BL to update the database record
            AttributeSettingObject attrSet = new AttributeSettingObject();
            try
            {
                attrSet.AttributeId = id;
                attrSet.Publish = status;
                attrSet.Last_Updated_By = User.Identity.Name;
                new AttributeSettingRepository().Publish(attrSet);
            }
            catch (Exception e)
            {
                LoggerFactory.GetLogger().Error(string.Format(msgUtil.GetMessage(MessageKey.LOG_PUTATTRIBUTES_EXCEPTION), id, status, User.Identity.Name), e);
                return null;
            }
            return attrSet.Last_Updated_By;
        }

        private void SetSearchSessionVariables(AttributeSearchObject searchObject)
        {
            Session["AttributeId"] = searchObject.AttributeId;
            Session["Published"] = searchObject.Published;
            Session["Page"] = searchObject.Page;
            Session["AttributePageSize"] = searchObject.PageSize;
        }
    }
}
