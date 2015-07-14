using System.Collections.Generic;
using System.Linq;
using Kikai.BL.DTO;
using Kikai.BL.IRepository;

namespace Kikai.BL.Concrete
{
    public class AttributeSettingRepository : IGenericRepository<AttributeSettingObject>, IAttributeSettingRepository
    {

        public IEnumerable<AttributeSettingObject> SelectAll()
        {
            return new DataUtils().GetList<AttributeSettingObject>("EXEC AttributeSetting_GetAll").ToList();
        }

        public AttributeSettingObject SelectByID(object id)
        {
            return new DataUtils().GetList<AttributeSettingObject>("EXEC AttributeSetting_GetById @AttributeId=@P1", id).ToList().SingleOrDefault();
        }

        public void Insert(AttributeSettingObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC AttributeSetting_Add @AttributeId=@P1,
                                    @Creation_Date=@P2, 
                                    @Created_By=@P3,
                                    @Last_Updated_By=@P4, 
                                    @Publish=@P5, 
                                    @Required=@P6",
                                    obj.AttributeId,
                                    obj.Creation_Date,
                                    obj.Created_By,
                                    obj.Last_Updated_By,
                                    obj.Publish,
                                    obj.Required);
        }

        public void Update(AttributeSettingObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC AttributeSetting_Update @AttributeId=@P1,
                                    @Creation_Date=@P2,
                                    @Created_By=@P3,
                                    @Last_Updated_By=@P4,
                                    @Publish=@P5,
                                    @Required=@P6",
                                    obj.AttributeId,
                                    obj.Creation_Date,
                                    obj.Created_By,
                                    obj.Last_Updated_By,
                                    obj.Publish,
                                    obj.Required);
        }

        public void Delete(object id)
        {
            new DataUtils().ExcecuteCommand(@"EXEC AttributeSetting_Delete @AttributeId=@P1", id);
        }


        public void Publish(AttributeSettingObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC AttributeSetting_Publish @AttributeId=@P1,
                                    @Status=@P2,
                                    @User=@P3",
                                    obj.AttributeId,
                                    obj.Publish,
                                    obj.Last_Updated_By);
        }
    }
}
