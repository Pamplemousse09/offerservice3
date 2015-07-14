using Kikai.BL.DTO.ApiObjects;
using Kikai.BL.DTO.WebAdminObjects;
using Kikai.BL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using Kikai.BL.DTO;

namespace Kikai.BL.Concrete
{
    public class AttributeRepository : IGenericRepository<AttributeObject>, IAttributeRepository
    {
        public IEnumerable<AttributeObject> SelectAll()
        {
            return new DataUtils().GetList<AttributeObject>("EXEC Attribute_GetAll").ToList();
        }

        public AttributeObject SelectByID(object id)
        {
            AttributeObject attribute = null;
            try
            {
                var AttributeOptionRepository = new AttributeOptionRepository();
                var AttributeSettingRepository = new AttributeSettingRepository();
                attribute = new DataUtils().GetList<AttributeObject>("EXEC Attribute_GetById @Id=@P1", id).ToList().SingleOrDefault();
                if (attribute != null)
                {
                    attribute.AttributeSetting = AttributeSettingRepository.SelectByID(attribute.Id);
                    attribute.AttributeSetting = CheckSetting(attribute);
                    attribute.AttributeOptions = AttributeOptionRepository.SelectListByID(attribute.Id).ToList();
                }
            }
            catch
            {
                throw;
            }

            return attribute;
        }

        public AttributeInfoObject SelectByIDWithInfo(object id)
        {
            AttributeInfoObject attribute = null;
            try
            {
                var AttributeOptionRepository = new AttributeOptionRepository();
                var AttributeSettingRepository = new AttributeSettingRepository();
                attribute = new DataUtils().GetList<AttributeInfoObject>("EXEC Attribute_GetById @Id=@P1", id).ToList().SingleOrDefault();
            }
            catch
            {
                throw;
            }

            return attribute;
        }

        /// <summary>
        /// This function selects all the published attributes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AttributeApiObject> SelectPublishedAttributes()
        {
            return new DataUtils().GetList<AttributeApiObject>("EXEC Attribute_GetPublishedAttributes");
        }

        /// <summary>
        /// This function selects all the required attributes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AttributeObject> SelectRequiredAttributes()
        {
            return new DataUtils().GetList<AttributeObject>("EXEC Attribute_GetRequired").ToList();
        }

        public void Insert(AttributeObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC Attribute_Add @Id=@P1,
                                    @Name=@P2,
                                    @ShortName=@P3, 
                                    @Label=@P4, 
                                    @Type=@P5",
                                    obj.Id,
                                    obj.ShortName,
                                    obj.Label,
                                    obj.Type);
        }

        public void Update(AttributeObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC Attribute_Update @Id=@P1,
                                    @Name=@P2,
                                    @ShortName=@P3,
                                    @Label=@P4,
                                    @Type=@P5",
                                    obj.Id,
                                    obj.Name,
                                    obj.ShortName,
                                    obj.Label,
                                    obj.Type);
        }

        public void Delete(object id)
        {
            new DataUtils().ExcecuteCommand(@"EXEC Attribute_Delete @Id=@P1", id);
        }

        /// <summary>
        /// Function that returns a filtered list of attributes
        /// </summary>
        /// <param name="attribute_id"></param>
        /// <param name="published"></param>
        /// <param name="page"></param>
        /// <param name="recordPerPage"></param>
        /// <returns></returns>
        public IEnumerable<FilteredAttributeObject> GetFilteredAttributes(string attribute_id, int? published, int page, int recordPerPage)
        {
            return new DataUtils().GetList<FilteredAttributeObject>(@"exec Attribute_GetFilteredAttributes @AttributeId=@P1,@Status=@P2,@Page=@P3,@RecordsPerPage = @P4", attribute_id, published, page, recordPerPage).ToList();
        }

        public void ProccessAttribute(string AttributeId)
        {
            var attribute = SelectByID(AttributeId);
            if (attribute.AttributeSetting.Publish == false && attribute.AttributeSetting.Last_Updated_By.ToLower() == "system")
            {
                AttributeSettingRepository attributeSettingRepository = new AttributeSettingRepository();
                attribute.AttributeSetting.Publish = true;
                attributeSettingRepository.Update(attribute.AttributeSetting);
            }
        }

        /// <summary>
        /// Function that checks if an attribute already has a record in attribute settings table, if not it will create a new record an return the attribue setting object
        /// </summary>
        /// <param name="Attribute"></param>
        /// <returns>AttributeSettingObject</returns>
        public AttributeSettingObject CheckSetting(AttributeObject Attribute)
        {
            AttributeSettingObject attributeSetting = new AttributeSettingObject();
            if (Attribute.AttributeSetting == null)
            {
                AttributeSettingRepository aprepo = new AttributeSettingRepository();
                attributeSetting.AttributeId = Attribute.Id;
                aprepo.Insert(attributeSetting);
                attributeSetting = aprepo.SelectByID(Attribute.Id);
            }
            else
            {
                attributeSetting = Attribute.AttributeSetting;
            }
            return attributeSetting;
        }

        /// <summary>
        /// A function that will return the attribute with it's option to display them in the providers api
        /// </summary>
        /// <param name="id"></param>
        /// <returns>AttributeDetailsApiObject</returns>
        public AttributeDetailsApiObject GetAttributeDetails(object id)
        {
            AttributeDetailsApiObject attributeDetails = null;
            try
            {
                AttributeObject attribute = SelectByID(id);
                if (attribute != null)
                {
                    attributeDetails = new AttributeDetailsApiObject();
                    attributeDetails.Id = attribute.Id;
                    attributeDetails.Label = attribute.Label;
                    attributeDetails.Type = attribute.Type;
                    attributeDetails.AttributeOptions = attribute.AttributeOptions;
                }
            }
            catch
            {
                throw;
            }
            return attributeDetails;
        }
    }
}
