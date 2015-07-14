using Kikai.WebApi.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Kikai.WebApi.Managers.IManagers
{
    public interface IAttributesManager
    {
        CodebookDataObject GetPublishedAttributes(HttpRequestMessage Request, string ApiUser);
        AttributeDataObject GetAttribute(HttpRequestMessage Request, string ApiUser, string AttributeId);
    }
}