using Kikai.BL.Concrete;
using Kikai.BL.DTO;
using Kikai.BL.DTO.WebAdminObjects;
using Kikai.Domain.Common;
using Kikai.Logging.Utils;
using Kikai.WebAdmin.Common;
using Kikai.WebAdmin.Extensions;
using Kikai.WebAdmin.Utils;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace Kikai.WebAdmin.Controllers
{
    [CustomAuthorize(UserPermission.OFFER_CAN_EDIT_OFFERS, UserPermission.OFFER_ADMIN)]
    public class RespondentAttributeController : Controller
    {
        private MessagesUtil msgUtil;
        public RespondentAttributeController()
        {
            msgUtil = new MessagesUtil();
        }

        //
        // GET: /RespondentAttribute/

        /// <summary>
        /// This function will add a respondent attribute to an offer
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool AddRespondentAttribute(string result)
        {
            RespondentAttributeObject attribute = JsonConvert.DeserializeObject<RespondentAttributeObject>(result);
            bool inserted = false;
            try
            {
                new RespondentAttributeRepository().Insert(attribute);
                inserted = true;
            }
            catch (Exception e)
            {
                LoggerFactory.GetLogger().Error(String.Format(msgUtil.GetMessage(MessageKey.LOG_ADDRESPONDENTATTRIBUTE_EXCEPTION), attribute.OfferId, attribute.Ident, attribute.Values), e);
            }
            return inserted;
        }

        public string AttributeInfo(string attributeId)
        {
            AttributeInfoObject attribute = new AttributeInfoObject();
            try
            {
                attribute = new AttributeRepository().SelectByIDWithInfo(attributeId);
            }
            catch (Exception e)
            {
                LoggerFactory.GetLogger().Error(String.Format(msgUtil.GetMessage(MessageKey.LOG_GETATTRIBUTEINFO_EXCEPTION), attribute.Id), e);
            }
            return JsonConvert.SerializeObject(attribute);
        }

        public bool DeleteAttribute(int attributeId)
        {
            bool deleted = false;
            try
            {
                new RespondentAttributeRepository().Delete(attributeId);
                deleted = true;
            }
            catch (Exception e)
            {
                LoggerFactory.GetLogger().Error(string.Format(msgUtil.GetMessage(MessageKey.LOG_DELETERESPONDENTATTRIBUTE_EXCEPTION), attributeId), e);
            }
            return deleted;
        }
    }
}
