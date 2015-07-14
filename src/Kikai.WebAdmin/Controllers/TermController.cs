using Kikai.BL.Concrete;
using Kikai.BL.DTO;
using Kikai.WebAdmin.UIModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Kikai.WebAdmin.Extensions;
using Kikai.Logging.Utils;
using Kikai.WebAdmin.Utils;
using Kikai.WebAdmin.Common;
using Kikai.Domain.Common;

namespace Kikai.WebAdmin.Controllers
{
    [CustomAuthorizeAttribute]
    public class TermController : Controller
    {
        private MessagesUtil msgUtil;

        public TermController()
        {
            msgUtil = new MessagesUtil();
        }

        [HttpPost]
        [CustomAuthorize(UserPermission.OFFER_CAN_EDIT_OFFERS, UserPermission.OFFER_ADMIN)]
        public void SetCPI(TermObject term)
        {
            try
            {
                term.Last_Updated_By = User.Identity.Name;
                new TermRepository().SetNewCPI(term);
            }
            catch (Exception e)
            {
                LoggerFactory.GetLogger().Error(String.Format(msgUtil.GetMessage(MessageKey.LOG_SETCPI_EXCEPTION), term.OfferId, term.CPI), e);
            }
        }

        public ActionResult GetTermsHistory(Guid OfferId)
        {
            try
            {
                IEnumerable<TermsModel> terms = new TermRepository().GetTermFromOfferId(OfferId).FromEntity();
                return PartialView("_TermsHistoryResult", terms);
            }
            catch (Exception e)
            {
                LoggerFactory.GetLogger().Error(String.Format(msgUtil.GetMessage(MessageKey.LOG_GETTERMSHISTORY_EXCEPTION), OfferId), e);
                return null;
            }
        }
    }
}
