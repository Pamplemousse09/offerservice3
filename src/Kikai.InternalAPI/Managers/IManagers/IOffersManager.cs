using Kikai.InternalAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Kikai.InternalAPI.Managers.IManagers
{
    public interface IOffersManager
    {
        OffersStudyIdDataObject GetOffersBySudyId(HttpRequestMessage Request, string StudyId, string ApiUser);
    }
}