using Kikai.BL.DTO;
using Kikai.BL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kikai.BL.Concrete
{
    public class AttributeOptionRepository : IGenericRepository<AttributeOptionObject>, IAttributeOptionRepository
    {

        public IEnumerable<AttributeOptionObject> SelectAll()
        {
            return new DataUtils().GetList<AttributeOptionObject>("EXEC AttributeOption_GetAll").ToList();
        }

        /// <summary>
        /// Method that will return first attribute option based on attribute Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AttributeOptionObject SelectByID(object id)
        {
            return new DataUtils().GetList<AttributeOptionObject>("EXEC AttributeOption_GetById @AttributeId=@P1", id).ToList().FirstOrDefault();
        }

        /// <summary>
        /// Function that will return a list of attribute options based on attribute Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<AttributeOptionObject> SelectListByID(object id)
        {
            return new DataUtils().GetList<AttributeOptionObject>("EXEC AttributeOption_GetById @AttributeId=@P1", id).ToList();
        }

        public void Insert(AttributeOptionObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC AttributeOption_Add @AttributeId=@P1,
                                    @Code=@P2, 
                                    @Description=@P3",
                                    obj.AttributeId,
                                    obj.Code,
                                    obj.Description);
        }

        public void Update(AttributeOptionObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC AttributeOption_Update @AttributeId=@P1,
                                    @Code=@P2,
                                    @Description=@P3",
                                    obj.AttributeId,
                                    obj.Code,
                                    obj.Description);
        }

        public void Delete(string AttributeId, int Code)
        {
            new DataUtils().ExcecuteCommand(@"EXEC AttributeSetting_Delete @AttributeId=@P1, @Code=@P2", AttributeId, Code);
        }


        public void Delete(object id)
        {
            throw new NotImplementedException();
        }
    }
}
