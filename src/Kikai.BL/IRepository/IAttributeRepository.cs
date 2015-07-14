using Kikai.BL.DTO;
using Kikai.BL.DTO.ApiObjects;
using System;
using System.Collections.Generic;

namespace Kikai.BL.IRepository
{
    public interface IAttributeRepository
    {
        IEnumerable<AttributeApiObject> SelectPublishedAttributes();

        IEnumerable<AttributeObject> SelectRequiredAttributes();

        IEnumerable<FilteredAttributeObject> GetFilteredAttributes(string attribute_id, int? published, int page, int recordPerPage);
        
        void ProccessAttribute(string AttriubteId);
        
        AttributeSettingObject CheckSetting(AttributeObject Attribute);

        AttributeDetailsApiObject GetAttributeDetails(object id);

        AttributeObject SelectByID(object id);
    }
}
