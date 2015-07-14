using Kikai.BL.DTO;
using Kikai.Logging.DTO;
using Kikai.WebAdmin.UIModels;
using System;
using System.Collections.Generic;

namespace Kikai.WebAdmin.Extensions
{
    public static class Methods
    {
        #region TermsModel  and Entity Conversion
        public static TermsModel FromEntity(this TermObject term)
        {
            TermsModel termModel = new TermsModel();

            termModel.Id = term.Id;
            termModel.OfferId = term.OfferId;
            termModel.Start = term.Start;
            termModel.Expiration = term.Expiration;
            termModel.Active = term.Active;
            termModel.User = term.Last_Updated_By;
            termModel.CPI = term.CPI;

            return termModel;
        }

        public static IEnumerable<TermsModel> FromEntity(this IEnumerable<TermObject> terms)
        {
            List<TermsModel> termModelsList = new List<TermsModel>();
            foreach (var term in terms)
            {
                termModelsList.Add(term.FromEntity());
            }
            return termModelsList;
        }
        #endregion TermsModel  and Entity Conversion


        public static LogObject Error_ToLogObject(string RequestId, string User, string Type, string Name, Dictionary<string, string> Parameters, List<ErrorObject> WebServiceResponse = null)
        {
            LogObject logObject = new LogObject();
            Dictionary<string, string> response = new Dictionary<string, string>();
            logObject.TimeStamp = DateTime.Now;
            logObject.RequestId = RequestId;
            logObject.User = User;
            logObject.Type = Type;
            logObject.Name = Name;
            logObject.Parameters = Parameters;
            if (WebServiceResponse.Count == 0)
            {
                response.Add("REQUEST_SUCCESSFUL", "The request was successful.");
            }
            else
            {
                foreach (var error in WebServiceResponse)
                {
                    if (!response.ContainsKey(error.ErrorKey))
                        response.Add(error.ErrorKey, error.Message);
                }
            }
            logObject.Response = response;
            return logObject;
        }
    }
}
